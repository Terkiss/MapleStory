# 🍁 MapleStory Market Graph (DotMaple)

.NET 9 Blazor 기반 고성능 시장 데이터 시각화 및 통합 자산 관리 대시보드입니다.

## 🚀 프로젝트 소개
메이플스토리 시장 데이터를 효율적으로 시각화하고 개인 자산을 체계적으로 관리하기 위해 구축된 프리미엄 대시보드입니다.
- **.NET 9 Blazor Web App (Interactive Server)** 기반의 풍부한 사용자 경험
- **SQLite & EF Core 9.0** 기반의 고성능 로컬 데이터 스토리지 (기존 구글 시트 대체)
- **Glassmorphism** 스타일과 메이플스토리 테마가 결합된 프리미엄 UI/UX
- SVG/Canvas 기반의 **고성능 캔들 차트** 및 실시간 통계 데스크

## � 프로젝트 문서 (Documentation)
상세한 기술 사양 및 아키텍처 정보는 [/Document](./Document/README.md) 폴더를 참조하세요.
- [시스템 아키텍처](./Document/Architecture/Architecture.md)
- [데이터베이스 스키마](./Document/Database/Schema.md)
- [서비스 레이어 명세](./Document/Architecture/Services.md)
- [구현 및 마이그레이션 이력](./Document/Archive/Migration_Log.md)

## �🛠 주요 기술 스택
- **Framework:** .NET 9 Blazor Web App
- **Database:** SQLite (Entity Framework Core 9.0)
- **Data Engine:** C# (MarketDataService, TeruTeruPandas.dll)
- **Authentication:** JWT, Google OAuth 2.0
- **UI/UX:** Vanilla CSS, Bootstrap Icons, Glassmorphism Design

## 📌 주요 기능
### 1. 메소마켓 거래장부 (Meso Market Ledger)
- **프리미엄 요약 카드:** 내역을 기반으로 평균 매수/매도 단가 및 실현 손익(Realized P/L)을 실시간 집계.
- **실시간 손익 계산:** `(매도단가 - 매수단가) * 수량` 공식을 통한 정확한 메이플포인트(MP) 수익 시각화.
- **인터랙티브 캘린더:** 날짜별 거래 내역 필터링 및 관리 기능 제공.

### 2. 통합 가계부 (Account Book)
- **SQLite 통합:** 기존 구글 시트 방식에서 로컬 DB로 전환하여 데이터 접근 속도 및 안정성 극대화.
- **스마트 통계:** 수입/지출 내역 분석을 통한 실시간 잔액(Net Balance) 및 월별 추이 시각화.
- **Nexon API 동기화:** 넥슨 오픈 API를 통해 거래 내역을 자동으로 가계부에 반영하는 기능 지원.

### 3. 고성능 차트 및 분석
- **인메모리 분석 엔진:** `TeruTeruPandas` 라이브러리를 통한 고속 데이터 처리 및 차트 렌더링.
- **인터랙티브 조작:** 실시간 확대/축소(Zoom), 타임프레임 전환 지원.

## 📝 최근 업데이트 현황
- [x] 가계부 데이터 저장소 SQLite 통합 완료 (성능 최적화)
- [x] 메소마켓 거래장부 프리미엄 UI 리팩토링 완료
- [x] 실현 손익(Realized P/L) 계산 로직 고도화 및 버그 수정
- [x] 프로젝트 통합 기술 문서(Document/) 구축 완료
- [x] 네비게이션 활성화 로직 및 UI 버그 수정

---
© 2026 MapleStory Market Graph Project. All rights reserved.
