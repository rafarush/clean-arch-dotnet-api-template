FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5125

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CleanArchTemplate.Api/CleanArchTemplate.Api.csproj", "CleanArchTemplate.Api/"]
RUN dotnet restore "CleanArchTemplate.Api/CleanArchTemplate.Api.csproj"
COPY . .
WORKDIR "/src/CleanArchTemplate.Api"
RUN dotnet build "./CleanArchTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CleanArchTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanArchTemplate.Api.dll"]
