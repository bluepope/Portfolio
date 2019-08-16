FROM bluepope/centos7-aspnetcore AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.2-sdk AS build
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