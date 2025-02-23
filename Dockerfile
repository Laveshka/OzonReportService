FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./*.props ./

COPY ["src/OzonReportService/OzonReportService.csproj", "src/OzonReportService/"]
COPY ["src/Application/OzonReportService.Application.Models/OzonReportService.Application.Models.csproj", "src/Application/OzonReportService.Application.Models/"]
COPY ["src/Presentation/OzonReportService.Presentation.Kafka/OzonReportService.Presentation.Kafka.csproj", "src/Presentation/OzonReportService.Presentation.Kafka/"]
COPY ["src/Application/OzonReportService.Application.Contracts/OzonReportService.Application.Contracts.csproj", "src/Application/OzonReportService.Application.Contracts/"]
COPY ["src/Application/OzonReportService.Application.Abstractions/OzonReportService.Application.Abstractions.csproj", "src/Application/OzonReportService.Application.Abstractions/"]
COPY ["src/Application/OzonReportService.Application/OzonReportService.Application.csproj", "src/Application/OzonReportService.Application/"]
COPY ["src/Infrastructure/OzonReportService.Infrastructure.Persistence/OzonReportService.Infrastructure.Persistence.csproj", "src/Infrastructure/OzonReportService.Infrastructure.Persistence/"]

RUN dotnet restore "src/OzonReportService/OzonReportService.csproj"

COPY . .
WORKDIR "/src/src/OzonReportService"
RUN dotnet build "OzonReportService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OzonReportService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OzonReportService.dll"]

