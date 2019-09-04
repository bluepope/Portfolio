#FROM bluepope/centos7-aspnetcore AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
RUN apt update -y
RUN apt install -y libc6-dev libgdiplus libx11-dev wget
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN apt install -y ./google-chrome-stable_current_amd64.deb
RUN rm google-chrome-stable_current_amd64.deb
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["CoreMvcWeb/CoreMvcWeb.csproj", "CoreMvcWeb/"]
COPY ["CoreLib/CoreLib.csproj", "CoreLib/"]
RUN dotnet restore "CoreMvcWeb/CoreMvcWeb.csproj"
COPY . .
WORKDIR "/src/CoreMvcWeb"
RUN dotnet build "CoreMvcWeb.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CoreMvcWeb.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CoreMvcWeb.dll"]