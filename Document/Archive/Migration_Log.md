# 프로젝트 구현 기록 및 마이그레이션 로그 (Archive)

본 문서는 프로젝트 진행 과정에서의 주요 의사결정, 리팩토링 및 아키텍처 변경 이력을 기록합니다.

---

## 📅 2026-03-12: 가계부 데이터 저장소 SQLite 통합

### 작업 개요
- 기존에 구글 시트(Google Sheets) API를 사용하여 저장하던 가계부 데이터를 로컬 SQLite 데이터베이스로 통합했습니다.

### 변경 이유
- **성능**: 구글 API 호출 시 발생하는 대기 시간 제거.
- **안정성**: 외부 서비스 의존성을 낮추고 데이터 무결성 확보.
- **단일화**: `MesoMarketTrades`와 동일한 EF Core 패턴을 사용하여 코드 복잡도 감소.

### 영향 범위
- `AccountBookService.cs`: Google API 의존성 완전 제거 및 EF Core 기반으로 재작성.
- `ApplicationDbContext.cs`: `AccountBookEntries` DbSet 추가.
- `appsettings.json`: GoogleSheets 관련 설정 항목 제거.

---

## 📅 2026-03-11: 메소마켓 거래장부 UI 리팩토링 및 손익 계산 로직 고도화

### 작업 개요
- 메소마켓 거래장부의 통계 섹션을 프리미엄 다크 모드 UI(Glassmorphism)로 전면 리팩토링했습니다.
- 실현 손익(Realized P/L) 계산 로직을 추가하고 시각화했습니다.

### 주요 반영 사항
- **UI**: 단일 카드 레이아웃, 고대비 폰트, 수직 구분선 적용.
- **Calculation**: `(평균 매도 단가 - 평균 매수 단가) * (매도 수량 / 1억)` 공식을 통해 메이플포인트(MP) 수익 산출.
- **Bug Fix**: 실현 손익이 1억 배 과다 계산되던 단위 버그 및 네비게이션 중복 활성화 버그 수정.

---

## 📂 이전 문서 보관
- [초기 가계부 구현 계획서 (Google Sheets 버전)](../Archive/Implementation_Sheets_Initial.md) - *현재 비활성화*
- [인증 시스템 구축 가이드](../Archive/Auth_System_Guide.md)
