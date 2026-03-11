# MapleStory Market Graph 프로젝트 문서 (Project Documentation)

이 디렉토리는 MapleStory Market Graph 프로젝트의 아키텍처, 데이터베이스 설계, 서비스 명세 및 개발 지침을 포함하는 공식 문서 저장소입니다. 본 문서를 통해 팀원들이 프로젝트의 구조를 빠르게 파용하고 유지보수할 수 있도록 돕습니다.

## 📂 문서 구조 (Directory Structure)

### 1. [시스템 아키텍처 (Architecture)](./Architecture/Architecture.md)
- 전체적인 소프트웨어 스택 및 계층 구조 설명
- 인증(JWT/Google) 및 클라이언트-서버 통신 방식

### 2. [데이터베이스 설계 (Database)](./Database/Schema.md)
- SQLite 테이블 스키마 정의
- Entity Framework Core 마이그레이션 관리 방법

### 3. [비즈니스 로직 및 서비스 (Services)](./Architecture/Services.md)
- `MesoMarketService`: 메소마켓 거래 데이터 처리
- `AccountBookService`: 통합 가계부(SQLite 기반) 처리
- `AuthService` & `NexonApiService`: 외부 연동 및 인증 처리

### 4. [기록관 (Archive)](./Archive/Migration_Log.md)
- 과거 구현 계획(Implementation Plans) 및 작업 이력
- 주요 리팩토링 및 아키텍처 변경 로그

---

## 🚀 프로젝트 개요 (Overview)

MapleStory Market Graph는 메이플스토리의 시장 데이터를 분석하고 개인의 자산을 체계적으로 관리하는 Blazor 기반 웹 애플리케이션입니다.

- **핵심 가치**: 데이터 시각화, 프리미엄 UI/UX, 분산된 데이터의 통합 관리
- **주요 기능**:
    - **메소마켓 거래장부**: 실시간 손익 계산 및 거래 내역 시각화
    - **통합 가계부**: 일상적인 수입/지출 관리 (SQLite 기반)
    - **Nexon API 연동**: 공식 데이터를 기반으로 한 자동화된 정보 수집

---

## ⚙️ 기술 스택 (Tech Stack)

- **Backend/Frontend**: .NET 9 Blazor (Interactive Server Render Mode)
- **Database**: SQLite (Entity Framework Core 9.0)
- **Authentication**: JWT (JSON Web Token), Google OAuth 2.0
- **UI/Components**: Vanilla CSS, Bootstrap Icons, Blazor Generic Components
- **Tools**: .NET CLI, Entity Framework Tools
