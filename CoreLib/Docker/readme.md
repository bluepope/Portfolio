#도커 기존 컨테이너에서 새로운 이미지 생성하기 *오래걸림
#commit 기존컨테이너이름 새이미지이름 
sudo docker commit centos centos7-aspnetcore

#private repo 
docker login -u username -p passwd docker.private.net
docker tag centos docker.private.net/centos
docker push docker.private.net/centos
