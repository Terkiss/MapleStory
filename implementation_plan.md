# 넥슨 API 연동 및 자체 인증 시스템 구현 계획서 (최종 통합본)

본 문서는 MesoMarket 시스템에 넥슨 게임 데이터(메이플스토리)를 연동하기 위한 인증 및 인가 아키텍처 구현 계획입니다. 
넥슨 공식 Open ID 서비스 이용의 한계를 극복하고, 확장성 있는 사용자 관리를 위해 **"SQLite 기반 자체/소셜 로그인 체계 + 사용자 개인 API Key 등록 방식"**을 채택했습니다.

## 1. 배경 및 연동 전략 (Strategy)

### 공식 넥슨 로그인(Open ID)의 한계
*   **권한 문제:** 넥슨 Open ID 기능은 현재 '메이플스토리 파트너스 제작자'로 승인된 애플리케이션에만 제공됩니다.
*   **기술적 제약:** OAuth 2.0 표준을 따르므로 승인되지 않은 비공식 리다이렉트 URI나 시크릿 키 검증 우회가 불가능합니다.
*   **해결책:** 공식 파트너 승인을 기다리는 동안 시스템 개발 및 서비스를 차질 없이 진행하기 위해, 우회 전략을 적용합니다.

### 채택된 우회 전략: 사용자별 API Key 할당 방식
1.  **MesoMarket 자체 회원가입/로그인:** 구글 시트 같은 외부 스토리지 대신 서버 내장형 **SQLite 데이터베이스**를 사용하여 사용자를 안전하게 자체 가입시키거나 구글 계정으로 로그인시킵니다.
2.  **API Key 개별 발급 및 등록:** 로그인한 유저는 "마이페이지"에서 본인이 직접 넥슨 개발자 포털을 통해 발급받은 API Key(개발 및 서비스 단계 무관)를 시스템에 등록합니다.
3.  **검증 후 저장:** 시스템은 입력받은 Key로 넥슨 서버에 가벼운 테스트 요청을 보내 유효성을 검증한 뒤 유저 DB 레코드에 안전하게 저장합니다.
4.  **권한 대행:** 이후 시스템 내 모든 넥슨 데이터 조회 요청(AccountBookService 등) 수행 시, 현재 로그인한 유저의 DB 컬럼에 저장된 `NexonApiKey`를 사용하여 넥슨 API를 호출합니다.

## 2. 데이터베이스 및 엔티티 설계 (Database & Entity Design)

효율적이고 관리가 편리한 `SQLite`와 `Entity Framework Core (EF Core)`를 도입합니다.

### [NEW] Models/User.cs (사용자 모델)
자체 이메일 가입 유저와 구글 소셜 가입 유저를 통합 처리하는 구조입니다.
*   `Id` (Guid 혹은 int PK): 시스템 고유 식별자
*   `Name` (string): 사용자 이름 (또는 닉네임)
*   `Email` (string): 로컬 가입 시 로그인 아이디, 구글 가입 시 구글 이메일 (유니크 제약조건 적용)
*   `PasswordHash` (string?): **로컬 가입자의 경우에만 존재**하는 해싱된 비밀번호. (구글 가입자는 NULL)
*   `GoogleSubjectId` (string?): **구글 연동 가입자의 경우에만 존재**하는 구글 고유 식별자. (로컬 가입자는 NULL)
*   `AuthType` (Enum): 가입 경로 구분용 (예: `Local`, `Google`)
*   `NexonApiKey` (string?): 사용자가 성공적으로 등록한 넥슨 1-to-1 개인 API Key.
*   `CreatedAt` (DateTime): 가입 일시

### [NEW] Data/ApplicationDbContext.cs
EF Core Context 클래스. `DbSet<User> Users`를 포함하며, `OnModelCreating`에서 `Email`에 대한 Unique 인덱스를 구성합니다.

## 3. 백엔드 서비스 구성 (Backend Services)

핵심 비즈니스 로직을 분리하여 구현합니다. JWT(Json Web Token) 기반의 Stateless 인증을 적용합니다.

### [NEW] Services/AuthService.cs (자체 인증)
1.  `RegisterLocalUserAsync(name, email, password, passwordConfirm)`:
    *   비밀번호 입력 및 확인(Confirm) 값 일치 여부 체크.
    *   이메일 중복 시도 체크.
    *   비밀번호를 `BCrypt` 또는 내장 해싱 알고리즘으로 암호화하여 DB에 Insert (`AuthType = Local`).
2.  `LoginLocalAsync(email, password)`: 이메일과 비밀번호 검증 후 JWT 토큰 발급.

### [NEW] Services/GoogleAuthService.cs (소셜 인증)
1.  `AuthenticateWithGoogleAsync(string googleIdToken)`:
    *   프론트엔드에서 구글 로그인 창을 띄운 뒤 넘겨받은 `ID Token`의 서명(Signature)과 유효성을 검증 라이브러리(`Google.Apis.Auth`)로 검사.
    *   검증된 정보에서 `Email`, `Name`, `Subject(ID)` 추출.
    *   DB에 `GoogleSubjectId`나 `Email`이 없으면 신규 가입 처리 (`AuthType = Google`).
    *   완료 후 로컬 시스템과 넥슨 API를 사용할 수 있는 우리만의 JWT 토큰 발급.

### [NEW] Services/NexonApiService.cs (API Key 헬퍼)
사용자로부터 받은 넥슨 API Key가 진짜인지, 혹은 만료되지 않았는지 확인하는 유틸리티 서비스.
1.  `VerifyApiKeyAsync(string apiKey)`:
    *   전달된 API Key를 HTTP Request Header (`x-nxopen-api-key`)에 세팅.
    *   가장 응답이 빠르고 파라미터가 적은 넥슨 API(예를 들어 캐릭터 식별자(ocid)를 조회하는 테스트용 호출 등)를 발송.
    *   결과가 `HTTP 200 OK` 이면 `true`, `400/401` 이면 `false` 반환.

## 4. API 엔드포인트 라우팅 (API Controllers)

클라이언트(프론트엔드/SPA)가 호출할 접점입니다.

### [NEW] Controllers/AuthController.cs
*   `[HttpPost("register/local")]`: 입력 폼 데이터(이름, 이메일, 비밀번호 두 번)를 받아 `AuthService.RegisterLocalUserAsync` 처리.
*   `[HttpPost("login/local")]`: 자체 로그인 처리용 엔드포인트.
*   `[HttpPost("login/google")]`: 구글 연동 로그인 처리용 엔드포인트.

### [NEW] Controllers/UserController.cs
현재 접속 중인 세션(JWT Bearer Token 등으로 인증된 상태: `[Authorize]`)에서 유저 본인의 정보를 수정하는 역할.
*   `[HttpPost("me/nexon-key")]`: 
    *   Payload: `{ "ApiKey": "test_xxx..." }`
    *   `NexonApiService.VerifyApiKeyAsync(key)` 검증 로직 통과 시, 현재 접속 유저의 클레임(Claim)에 있는 User ID를 이용해 DB에서 `User` 객체를 조회.
    *   해당 유저의 `NexonApiKey` 필드 업데이트 후 DB 저장. (유효하지 않은 키라면 400 Bad Request 에러 및 메시지 반환)

## 5. 앱 환경 설정 (Configurations)

### [MODIFY] appsettings.json
DB 및 외부 연동 정보 구성.
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=mesomarket.db" // SQLite 기반 오프라인 DB 파일
  },
  "Authentication": {
    "Jwt": {
      "Key": "YOUR_SUPER_SECRET_KEY_FOR_LOCAL_JWT...", 
      "Issuer": "MesoMarket"
    },
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID" // 구글 API 콘솔에서 발급받은 클라이언트 ID
    }
  },
  "NexonApi": {
    "BaseUrl": "https://open.api.nexon.com"
  }
}
```

## 6. 프론트엔드 UI 화면 구현 계획 (Frontend Flow)

백엔드 API를 호출하고 사용자에게 보여질 화면(UI) 구성 요소입니다.

### 6-1. 회원가입 화면 (Register Page)
*   **자체 가입 폼:** 
    *   `이름 (Name)`, `이메일 (Email)`, `비밀번호 (Password)`, `비밀번호 확인 (Password Confirm)` 입력 필드 제공.
    *   제출 버튼 클릭 시 프론트엔드 단에서 비밀번호 일치 여부 1차 검증 후 `/register/local` API 호출.
*   **소셜 연동 버튼:** 폼 최상단 또는 최하단에 형태의 눈에 띄는 "Google 계정으로 시작하기" 버튼 배치.

### 6-2. 로그인 화면 (Login Page)
*   **자체 로그인 폼:** `이메일`과 `비밀번호` 입력 필드 및 "로그인" 버튼.
*   **소셜 로그인 버튼:** 회원가입 화면과 동일한 "Google로 로그인" 버튼 배치.
*   성공 시 발급받은 JWT 토큰을 브라우저의 `localStorage` 또는 `HttpOnly Cookie`에 저장하고 접속 상태(Header 등) 갱신.

### 6-3. 마이페이지 화면 (My Page - 넥슨 연동)
사용자가 로그인한 후 진입하는 개인 설정 영역입니다.
*   **현재 연동 상태 표시:** 유저 DB에 `NexonApiKey`가 있는지 여부에 따라 "연동 완료 (캐릭터명)" 혹은 "미연동" 상태를 배지로 표시.
*   **API Key 입력 폼:**
    *   "넥슨 API Key 등록" 입력 텍스트 박스 제공.
    *   우측 상단에 툴팁이나 모달 창을 통해 **"넥슨 개발자 센터에서 내 API Key 발급받는 방법 가이드"** (링크 및 스크린샷 1~2장) 제공.
    *   "연동하기" 버튼 클릭 시 상태(Loading 핑글핑글)를 보여주고 `/me/nexon-key` 호출. 성공 시 화면 즉시 리로드하여 연동 상태 업데이트.

## 7. 검증 계획 (Verification Plan)

### 애플리케이션 빌드 및 DB 마이그레이션 확인
*   `dotnet ef migrations add InitialCreate` 명령어 실행 시 오류가 없는지 확인.
*   `dotnet ef database update` 실행 후 솔루션 폴더 내에 `mesomarket.db` (SQLite 파일)가 성공적으로 생성되는지 체크.

### API 및 UI 동작 수동 검증 (End-To-End)
1.  **자체(Local) 가입 및 예외:** UI에서 비밀번호와 비밀번호 확인 값이 불일치할 경우 폼 아래 경고 메시지가 뜨는지 확인. 유효한 데이터 전송 시 200/201 응답과 함께 DB에 데이터 적재 후 로그인 창으로 전환 확인.
2.  **구글 소셜 로그인:** 구글 버튼을 눌러 소셜 가입/로그인이 한 큐에 이루어지고 메인 화면으로 리다이렉트 되는지 확인.
3.  **마이페이지 API 연동 성공 여부:** 
    *   마이페이지에서 무작위 문자열("1234") 전송 시 넥슨 연동 실패 경고창(Alert/Toast) 표출.
    *   진짜 넥슨 API 키 입력 시 "연동 성공!" 메시지와 함께 마이페이지 UI 상태가 "연동 완료"로 바뀌는지 체크. 
