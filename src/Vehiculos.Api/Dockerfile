FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Vehiculos.Api.csproj", "./"]
RUN dotnet restore "./Vehiculos.Api.csproj" /p:TargetFramework=net8.0
COPY . .
RUN dotnet publish "./Vehiculos.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:TargetFramework=net8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Vehiculos.Api.dll"]
