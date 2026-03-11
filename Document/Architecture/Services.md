# 서비스 레이어 명세 (Service Layer Specification)

프로젝트의 핵심 비즈니스 로직을 담당하는 주요 서비스들에 대한 설명입니다.

## 🛠️ 핵심 서비스 목록

### 1. `MesoMarketService`
- **목적**: 메소마켓 거래 장부의 데이터 관리.
- **주요 기능**:
    - `AddTradeAsync`: 거래 내역 저장.
    - `GetTradesAsync`: 사용자별 거래 목록 조회(최신순).
    - `GetStatisticsAsync`: 평균 매수/매도 단가 및 총량 계산 루틴 제공.
    - `DeleteTradeAsync`: 특정 거래 삭제.

### 2. `AccountBookService` (SQLite 통합 버전)
- **목적**: 범용 가계부 데이터의 SQLite 영속성 관리.
- **특징**: 기존 Google Sheets 연동 방식에서 SQLite로 전환되어 성능이 비약적으로 향상됨.
- **주요 기능**:
    - `AddEntryAsync`: 가계부 항목 추가.
    - `GetEntriesAsync`: 날짜별/사용자별 항목 필터링 조회.
    - `SyncFromNexonMesoMarketAsync`: 넥슨 API를 통해 거래 이력을 가계부로 덤프.

### 3. `NexonApiService`
- **목적**: 넥슨 오픈 API와의 통신 규격 관리.
- **주요 기능**:
    - API Key 관리 및 헤더 주입.
    - 메소마켓 시세 내역(`history`) 요청 및 데이터 파싱.

### 4. `MarketDataService` (Singleton)
- **목적**: 애플리케이션 전반에서 공유되는 시세 데이터를 메모리에 캐싱.
- **특징**: 네트워크 요청 최소화를 위해 싱글톤으로 등록되어 관리됨.

---

## 💉 의존성 주입 (Dependency Injection)

`Program.cs`에서 다음과 같이 서비스가 등록되어 있습니다.

```csharp
// Application Services
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddScoped<AccountBookService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<NexonApiService>();
builder.Services.AddScoped<MesoMarketService>();
```
