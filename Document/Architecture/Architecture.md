# 시스템 아키텍처 (System Architecture)

본 문서는 MapleStory Market Graph의 전반적인 시스템 구조와 기술적 설계 결정을 설명합니다.

## 🧱 계층 구조 (Layered Architecture)

프로젝트는 유지보수성과 확장성을 위해 다음과 같은 계층 구조를 따릅니다.

### 1. Presentation Layer (Blazor Components)
- **위치**: `/Components/Pages`, `/Components/Layout`
- **역할**: 사용자 인터페이스(UI) 렌더링 및 사용자 이벤트 처리.
- **특징**: Blazor Interactive Server 모드를 사용하여 실시간 상태 업데이트 제공.

### 2. Service Layer (Business Logic)
- **위치**: `/Services`
- **역할**: 데이터 가공, 외부 API(넥슨) 통신, DB 접근 추상화.
- **주요 서비스**:
    - `AccountBookService`: 가계부 데이터 CRUD (SQLite 사용).
    - `MesoMarketService`: 메소마켓 거래 데이터 집계 및 관리.
    - `MarketDataService`: (Singleton) 캐시된 시장 데이터 관리.

### 3. Data Access Layer (EF Core)
- **위치**: `/Data`
- **역할**: `ApplicationDbContext`를 통해 SQLite 데이터베이스와 통신.
- **ORM**: Entity Framework Core 9.0 (Code-First 방식).

---

## 🔐 인증 아키텍처 (Authentication)

### JWT 기반 인증
- 유저 로그인 시 서버에서 JWT(JSON Web Token) 발급.
- 브라우저의 `localStorage`에 저장 및 `AuthenticationStateProvider`를 통한 전역 상태 관리.
- `/Services/JwtAuthenticationStateProvider.cs`에서 토큰 검증 및 사용자 컨텍스트 유지.

### Google OAuth 2.0
- 구글 계정을 통한 간편 로그인 지원.
- 클라이언트 측 구글 인증 후 토큰을 서버로 전달하여 처리.

---

## 🌐 외부 API 연동 (Integration)

- **Nexon Open API**: 메이플스토리 공식 데이터를 가져오기 위해 전용 `NexonApiService`를 사용합니다.
- **가계부 동기화**: 사용자 승인 하에 넥슨 API를 통해 메소마켓 거래 이력을 가계부로 자동 동기화하는 로직이 포함되어 있습니다.

---

## 💡 주요 설계 결정 (Design Decisions)

1. **SQLite 채택**: 가벼운 데이터베이스 사용으로 배포 비용 절감 및 개발 속도 향상.
2. **Interactive Server Mode**: 복잡한 대시보드 기능을 실시간으로 구현하기 위해 채택.
3. **Glassmorphism UI**: 프리미엄 앱 느낌을 주기 위해 CSS 변수를 활용한 고대비 다크 모드 디자인 시스템 구축.
