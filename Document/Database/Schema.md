# 데이터베이스 스키마 (Database Schema)

MapleStory Market Graph는 **SQLite**를 기본 데이터베이스로 사용하며, **Entity Framework Core**를 통해 관리됩니다.

## 📂 데이터베이스 파일
- **파일명**: `mesomarket.db`
- **위치**: 프로젝트 루트 디렉토리

---

## 📊 주요 테이블 명세

### 1. `Users`
사용자 계정 정보를 관리합니다.

| 컬럼명 | 타입 | 설명 | 제약 조건 |
| :--- | :--- | :--- | :--- |
| **Id** | INTEGER | 사용자 고유 ID | Primary Key |
| **Email** | TEXT | 사용자 이메일 | Unique |
| **DisplayName** | TEXT | 화면 표시 이름 | - |
| **GoogleSubjectId** | TEXT | 구글 계정 고유 ID | Unique |

### 2. `MesoMarketTrades`
메소마켓 거래 내역을 저장합니다.

| 컬럼명 | 타입 | 설명 | 비고 |
| :--- | :--- | :--- | :--- |
| **Id** | INTEGER | 거래 고유 ID | PK |
| **UserId** | TEXT | 사용자 ID | FK (Users) |
| **Timestamp** | DATETIME | 거래 시간 | - |
| **TradeType** | TEXT | 매수 / 매도 | - |
| **MesoAmount** | REAL | 거래 메소 수량 | - |
| **PointAmount** | REAL | 메이플포인트 수량 | - |
| **UnitPrice** | REAL | 1억당 단가 | - |
| **Memo** | TEXT | 메모 | - |

### 3. `AccountBookEntries`
통합 가계부의 수입/지출 내역을 관리합니다.

| 컬럼명 | 타입 | 설명 | 비고 |
| :--- | :--- | :--- | :--- |
| **Id** | INTEGER | 항목 고유 ID | PK |
| **UserId** | TEXT | 사용자 ID | Indexed |
| **Timestamp** | DATETIME | 발생 일시 | - |
| **Type** | TEXT | 수입 / 지출 | - |
| **Category** | TEXT | 분류 (메소마켓/사냥 등) | - |
| **Title** | TEXT | 제목 | - |
| **Amount** | REAL | 금액 | - |
| **Unit** | TEXT | 단위 (Meso/Point) | - |
| **IsUp** | INTEGER | 상승/하락 여부 | 0(False) / 1(True) |
| **Memo** | TEXT | 상세 메모 | - |

---

## 🛠️ DB 관리 명령 (EF Core CLI)

1. **새로운 마이그레이션 추가**
   ```bash
   dotnet ef migrations add <MigrationName>
   ```

2. **데이터베이스 업데이트 (스키마 적용)**
   ```bash
   dotnet ef database update
   ```

3. **마이그레이션 내역 조회**
   ```bash
   sqlite3 mesomarket.db "SELECT * FROM __EFMigrationsHistory;"
   ```
