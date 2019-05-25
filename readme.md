# Core Mvc 포트폴리오용 작업

# CoreMvcWeb 폴더의 dbconfig.json 파일에 connection string 을 필수로 넣어주세요
# 최초실행시 자동 생성됩니다.

# 현재 테스트는 Home DB 의 COL1 COL2 를 불러오는 DB 작업입니다. -- 추후 삭제
Home DB 생성

CREATE TEST1 (
	COL1 INT,
	COL2 LONGTEXT
)

추후 DB 스크립트 공유방법이 필요할 것으로 보입니다


# 작업할 것
1. 로그인 로그아웃
  - 쿠키기반, identity, redis 연동? 등
  - oauth2를 이용한 sns 로그인 

2. 개별 접근권한 관리
  - controller 에서 role 로 제어? 
  - claim 이용은 안해봤는데 차이가?

3. ajax json 을 통한 통신, 파일 업로드, 다운로드 구현

4. signalR
  - 채팅 구현
  - 접속자 관리 및 메세지 푸시

5. async await 를 사용한 비동기 처리 및 비동기 메소드 개발

6. telegramBot을 통한 알림메세지 발송

7. reCAPTCHA 인증