FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CleanArchTemplate.Presentation.Api/CleanArchTemplate.Presentation.Api.csproj", "CleanArchTemplate.Presentation.Api/"]
COPY ["CleanArchTemplate.Application/CleanArchTemplate.Application.csproj", "CleanArchTemplate.Application/"]
COPY ["CleanArchTemplate.Domain/CleanArchTemplate.Domain.csproj", "CleanArchTemplate.Domain/"]
COPY ["CleanArchTemplate.Infrastructure/CleanArchTemplate.Infrastructure.csproj", "CleanArchTemplate.Infrastructure/"]
COPY ["CleanArchTemplate.SharedKernel/CleanArchTemplate.SharedKernel.csproj", "CleanArchTemplate.SharedKernel/"]

RUN dotnet restore "CleanArchTemplate.Presentation.Api/CleanArchTemplate.Presentation.Api.csproj"

COPY . .

WORKDIR "/src/CleanArchTemplate.Presentation.Api"

RUN dotnet publish \
    "./CleanArchTemplate.Presentation.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CleanArchTemplate.Presentation.Api.dll"]