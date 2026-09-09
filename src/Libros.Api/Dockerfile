FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Libros.Api.csproj", "./"]
RUN dotnet restore "./Libros.Api.csproj" /p:TargetFramework=net8.0
COPY . .
RUN dotnet publish "./Libros.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:TargetFramework=net8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Libros.Api.dll"]
