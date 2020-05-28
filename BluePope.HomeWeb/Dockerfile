#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM bluepope/centos8-dotnet3.1-chrome83

WORKDIR /app
EXPOSE 5000
EXPOSE 5001

COPY BluePope.HomeWeb/bin/Release/netcoreapp3.1 .
COPY BluePope.HomeWeb/wwwroot wwwroot/

ENTRYPOINT ["dotnet", "HomeWeb.dll"]