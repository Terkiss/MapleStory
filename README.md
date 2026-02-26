# 🍁 MapleStory Market Graph (DotMaple)

.NET 9 Blazor 기반 고성능 시장 데이터 시각화 대시보드입니다.

## 🚀 프로젝트 소개
메이플스토리 시장 데이터를 효율적으로 시각화하기 위해 구축된 고성능 대시보드로, 다음과 같은 특징을 가집니다:
- **.NET 9 Blazor Web App (Interactive Server)** 환경 지원
- **TeruTeruPandas** 라이브러리를 활용한 데이터 엔진 연동
- **Glassmorphism** 스타일을 적용한 모던 UI/UX
- SVG/Canvas 기반의 **고성능 캔들 차트** 및 미니맵 제공

## 🛠 주요 기술 스택
- **Framework:** .NET 9 Blazor Web App
- **Backend/Data:** C# (MarketDataService, TeruTeruPandas.dll 참조)
- **Frontend:** HTML, Vanilla CSS (Glassmorphism 테마)
- **Visualization:** 전용 고성능 캔들 차트 컴포넌트

## 📌 주요 기능
1. **시장 데이터 분석 엔진 연동:** 인메모리 데이터를 초기화하여 다양한 타임프레임(5분~24시간)의 데이터를 실시간으로 로드 및 브라우징합니다.
2. **반응형 대시보드 UI:** Header, Main Chart, Mini-map, Sidebar 등으로 구성된 체계적인 레이아웃.
3. **인터랙티브 차트:** 타임프레임 전환, 실시간 업데이트 시뮬레이션 등을 지원합니다.

## ⚙️ 환경 구성
- `/data`: 정적 자원 및 캐시 등 경로
- `/dll`: `TeruTeruPandas.dll` 등 외부 엔진 라이브러리 참조

## 📝 개발 진행 상황
- [x] 프로젝트 초기화 및 레이아웃 구성
- [x] 프론트엔드 UI 컴포넌트(차트 포함) 개발 완료
- [x] 데이터 통합 및 엔진 연동 (기본 구현)
- [ ] OHLV 및 MA(이동평균) 계산 로직 연동
- [ ] JIT 최적화 및 엔진 성능 검증 진행 중
