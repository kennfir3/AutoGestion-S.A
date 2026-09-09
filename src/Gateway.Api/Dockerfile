FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Gateway.Api.csproj", "./"]
RUN dotnet restore "./Gateway.Api.csproj" /p:TargetFramework=net8.0
COPY . .
RUN dotnet publish "./Gateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:TargetFramework=net8.0

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
