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
1. jquery.download 다운로드 구현

2. signalR
  - 접속자 관리 기능 등

3. 로그인 로그아웃
  - oauth2를 이용한 sns 로그인 

4. 개별 접근권한 관리
  - controller 에서 role 로 제어? 
  - claim 이용은 안해봤는데 차이가?

5. async await 를 사용한 비동기 처리 및 비동기 테스트 개발

6. telegramBot을 통한 알림메세지 발송

7. reCAPTCHA 인증

8. datetime picker, select2 등의 모듈 적용

9. vue 용 component 적용
 - 현재 js 의 vue 파일은 제가 임시로 만든 것들이며 제대로 정리하여 typescript 로 재작성하고 싶습니다.

 10. 편집기 (summernote) 를 이용한 게시물 등록 및 xss 공격에 대한 처리

# 완료된 것
1. 쿠키기반 로그인 로그아웃
2. signalR 채팅, 별도 관리자 일괄 전송
3. vue 를 이용한 기본 table 조회 추가 삭제 저장 관리
4. ajax json 및 form append 를 이용한 저장 및 파일 업로드 관리