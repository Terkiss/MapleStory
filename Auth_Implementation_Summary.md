# 메이플스토리 오픈 API 연동 및 자체 인증 시스템 작업 요약서

본 문서는 Gemini CLI가 초기 작성한 코드를 기반으로, 누락되거나 불완전했던 UI 및 인증 연동 로직을 완성하고 최종 반영한 작업 내역을 요약합니다. 이 문서를 통해 지금까지의 작업 방향과 변경점(복구 요소)을 파악할 수 있습니다.

## 1. Google 소셜 로그인 기능 정상화
*   **문제점:** `Login.razor` 및 `Register.razor`에 구글 로그인 버튼 껍데기만 존재하고 실제 연동 자바스크립트는 누락되어 있었습니다 ("Google 로그인은 현재 구현 중입니다").
*   **해결책:** 
    *   `App.razor`의 `<head>` 영역에 **Google Identity Services (GSI)** 공식 라이브러리(`https://accounts.google.com/gsi/client`) 스크립트를 주입했습니다.
    *   초기화 및 랜더링을 담당하는 `initializeGoogleLogin` 전역 JS 함수를 구현했습니다.
    *   `Login.razor`, `Register.razor`에서 `@inject IJSRuntime`을 활용해 GSI 컨테이너를 그리고, 로그인 성공 시 생성되는 Google Credential Token을 C# 백엔드(`GoogleAuthService`)로 쏴주는 브릿지 메서드(`[JSInvokable] HandleGoogleCredential`)를 추가하여 완벽히 연동시켰습니다.

## 2. 블레이저 라우팅 렌더링 모드 수정 (동적 UI 업데이트)
*   **문제점:** 로그인을 성공하여 로컬 스토리지에 JWT 토큰이 저장되어도, 새로고침을 누르기 전까지는 우측 상단 메인 헤더의 로그인 마크가 닉네임 유저 프로필로 전환되지 않는(UI 갱신 버그) 현상이 있었습니다.
*   **해결책:** `App.razor` 파일의 `<Routes />` 컴포넌트를 정적 렌더링에서 **실시간 서버 상호작용 방식(`@rendermode="new InteractiveServerRenderMode(prerender: false)"`)**으로 변경하여, 상태(Auth State) 변화 시 헤더 컴포넌트(`MainLayout.razor`)가 즉각 반응하고 렌더링되도록 수정했습니다.

## 3. UI 및 디자인 통일 (MesoMarket 테마 적용)
*   **문제점:** 기존 회원가입 및 로그인 페이지의 인풋(Input)과 버튼 디자인이 다소 투박하고, 메인 차트 페이지(`Home.razor`)의 다크/프리미엄 컨셉과 이질감이 있었습니다.
*   **해결책:** 기존 테마 색상(var(--maple-gold), 완전한 검은색 배경)으로 스타일을 교체하고, `border-radius: 12px`, 클릭 시 골드빛 글로우 아웃라인 이펙트를 세팅하여 어색함 없이 고급스러운 일체감을 주도록 디자인 코드를 변경했습니다. 또한 `MainLayout.razor` 헤더에 유저 아이콘 바로 옆 **로그아웃 버튼**을 추가했습니다.

## 4. 로컬 데이터베이스 (SQLite) 구축
*   **내역:** EF Core CLI 툴(`dotnet ef`)을 활용하여, 설계된 `User` 모델을 기반으로 `InitialAuthCreate` 마이그레이션 스키마를 생성하고 `database update` 명령어를 통해 프로젝트 루트에 실제 `mesomarket.db` 파일을 성공적으로 구성했습니다.
