FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./*.props ./

COPY ["src/OzonService/OzonService.csproj", "src/OzonService/"]
COPY ["src/Application/OzonService.Application.Models/OzonService.Application.Models.csproj", "src/Application/OzonService.Application.Models/"]
COPY ["src/Presentation/OzonService.Presentation.Kafka/OzonService.Presentation.Kafka.csproj", "src/Presentation/OzonService.Presentation.Kafka/"]
COPY ["src/Application/OzonService.Application.Contracts/OzonService.Application.Contracts.csproj", "src/Application/OzonService.Application.Contracts/"]
COPY ["src/Application/OzonService.Application.Abstractions/OzonService.Application.Abstractions.csproj", "src/Application/OzonService.Application.Abstractions/"]
COPY ["src/Application/OzonService.Application/OzonService.Application.csproj", "src/Application/OzonService.Application/"]
COPY ["src/Infrastructure/OzonService.Infrastructure.Persistence/OzonService.Infrastructure.Persistence.csproj", "src/Infrastructure/OzonService.Infrastructure.Persistence/"]

RUN dotnet restore "src/OzonService/OzonService.csproj"

COPY . .
WORKDIR "/src/src/OzonService"
RUN dotnet build "OzonService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OzonService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OzonService.dll"]

