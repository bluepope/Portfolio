# Core Mvc 포트폴리오용 작업

# 최초실행시 오류가 발생하는 경우 root 폴더에 생성된 usersettings.json를 확인해주세요

# 완료된 것
- [x] 쿠키기반 로그인 로그아웃 - DB연동은 해야함
- [x] signalR - 채팅, 관리자 전체 전송 기능
- [x] 개별 접근권한 관리 - Claim 및 Role을 이용한 권한관리
- [x] async await 를 사용한 비동기 처리 및 비동기 테스트 개발
- [x] telegramBot - Echo까지 해놓음
- [x] 편집기 (summernote) 를 이용한 게시물 등록
- [x] vue 를 이용한 기본 grid 형태 조회 추가 삭제 저장 관리 샘플
- [x] ajax json 및 form append 를 이용한 저장 및 파일 업로드 관리 
- [x] formData를 이용한 전송 progress 표시 기능 - typescript
- [x] barcode 생성 기능
- [x] selenium chrome 을 이용한 web 크롤링
- [x] http client 를 이용한 다운로드 (단순 비동기, progress bar 형태 필요할 듯)
- [x] HttpClient 비동기 형태로 업로드 / 다운로드 progress 처리 기능
- [x] docker 관리 - docker toolbox 로 기본적인 테스트는 하였으나 wsl2 docker 나올때까지 일단 대기
- [x] Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 추가하고 디버그시에만 실시간 cshtml 컴파일 반영 처리 (Project 파일 및 Statup 파일)
- [x] xss 공격에 대한 처리 - Ganss.XSS.HtmlSanitizer 적용
# 개발할 것
- [ ] oauth2를 이용한 가입 / 로그인 - google, facebook 정도만
- [ ] .vue 형태로 component 개발 - datetime picker, select2 --> typescript로 개발 해봐야함
- [ ] jenkins 연동 ci 테스트 -> 자동화된 docker 배포 가능한가?
- [ ] google reCAPTCHA v3 인증 --> ajax 로그인시 반영이 어렵네..
- [ ] docker 적용시 ssl 적용 문제

# 별도 프로젝트로 제외
- [ ] api area 개발 후 wpf or winform 과 연동

#usersettings.json 예시
{
  "ConnectionString": {
    "MySql": "Server=xx;Port=xx;Database=xx;Uid=xx;Pwd=xx;",
  },
  "TelegramBot": {
    "BotToken": "",
    "Socks5Host": null,
    "Socks5Port": 0,
    "WebHookUrl": ""
  },
  "ReCaptcha": {
    "SiteKey": "",
    "PrivateKey": ""
  }
}

#iis 실행을 원하시는 경우
 - Program.cs 에서 UseKestrel 관련 구문 삭제해주세요