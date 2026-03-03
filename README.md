# 🍁 MapleStory Market Graph (DotMaple)

.NET 9 Blazor 기반 고성능 시장 데이터 시각화 및 통합 자산 관리 대시보드입니다.

## 🚀 프로젝트 소개
메이플스토리 시장 데이터를 효율적으로 시각화하고 개인 자산을 체계적으로 관리하기 위해 구축된 프리미엄 대시보드입니다.
- **.NET 9 Blazor Web App (Interactive Server)** 기반의 풍부한 사용자 경험
- **TeruTeruPandas** 라이브러리를 활용한 고속 데이터 분석 엔진 연동
- **Glassmorphism** 스타일과 메이플스토리 테마가 결합된 프리미엄 UI/UX
- SVG/Canvas 기반의 **고성능 캔들 차트** 및 실시간 통계 데스크

## 🛠 주요 기술 스택
- **Framework:** .NET 9 Blazor Web App
- **Database:** Google Sheets API v4 (가계부 연동)
- **Data Engine:** C# (MarketDataService, TeruTeruPandas.dll)
- **UI/UX:** HTML, Vanilla CSS (Custom Key-color System)
- **Icons:** Custom Maple Icons (Meso, Point, Orange Mushroom)

## 📌 주요 기능
### 1. 통합 가계부 (Account Book)
- **구글 시트 연동:** 별도의 DB 설치 없이 구글 시트를 데이터베이스로 활용하여 실시간 저장 및 동기화 수행.
- **스마트 통계:** 수입/지출 내역을 분석하여 실시간 잔액(Net Balance) 및 월별 추이를 시각화.
- **맞춤형 아이콘:** Meso, Maple Point 등 각 화폐 단위별 전용 아이콘 적용으로 직관적인 UX 제공.
- **인터랙티브 차트:** `TeruTeruPandas`로 집계된 실제 데이터를 바탕으로 한 바 차트 및 파이 차트 제공 (마우스 호버 툴팁 지원).

### 2. 시장 데이터 분석
- **인메모리 분석 엔진:** 5분부터 24시간까지 다양한 타임프레임의 데이터를 고속으로 스위칭 및 브라우징.
- **기술적 지표:** OHLV 데이터와 이동평균선(SMA) 등 분석 로직 통합.

### 3. 고성능 차트 솔루션
- **인터랙티브 조작:** 실시간 확대/축소(Zoom), 타임프레임 전환, 실시간 시뮬레이션 지원.
- **확장성:** 전용 가격 단위(`Unit`) 바인딩으로 억 단위 이상의 고액 지표도 정확하게 시각화.

## ⚙️ 환경 구성
- `/data`: 시장 데이터 CSV 및 정적 자원
- `/dll`: `TeruTeruPandas.dll` 엔진 라이브러리
- `service_account.json`: 구글 시트 연동을 위한 인증 파일 (보안상 .gitignore 등록 권장)

## 📝 개발 진행 상황
- [x] 프로젝트 초기화 및 메이플 테마 레이아웃 구성
- [x] 고성능 캔들 차트 및 인터랙티브 UI 개발
- [x] 가계부 기능 (구글 시트 연동 및 분석 엔진 결합) 완료
- [x] Meso/Point 전용 아이콘 및 프리미엄 로고 적용
- [x] 시장 데이터 경로 최적화 및 안정화
- [ ] 마켓 분석 자동화 로직 고도화 진행 중
