FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build

# Copy global project files
COPY ["aspnet-core/*.props", \
    "aspnet-core/*.json", \
    "aspnet-core/*.config", \
    "aspnet-core/*.targets", \
    "aspnet-core/*.runsettings", \
    "aspnet-core/*.sln", \
    "aspnet-core/"]

# Copy csproj
COPY ["aspnet-core/src/Sg.ClockifyIt.Application/*.csproj", "aspnet-core/src/Sg.ClockifyIt.Application/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.Application.Contracts/*.csproj", "aspnet-core/src/Sg.ClockifyIt.Application.Contracts/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.DbMigrator/*.csproj", "aspnet-core/src/Sg.ClockifyIt.DbMigrator/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.Domain/*.csproj", "aspnet-core/src/Sg.ClockifyIt.Domain/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.Domain.Shared/*.csproj", "aspnet-core/src/Sg.ClockifyIt.Domain.Shared/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.EntityFrameworkCore/*.csproj", "aspnet-core/src/Sg.ClockifyIt.EntityFrameworkCore/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.HttpApi/*.csproj", "aspnet-core/src/Sg.ClockifyIt.HttpApi/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.HttpApi.Client/*.csproj", "aspnet-core/src/Sg.ClockifyIt.HttpApi.Client/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.HttpApi.Host/*.csproj", "aspnet-core/src/Sg.ClockifyIt.HttpApi.Host/"]
COPY ["aspnet-core/src/Sg.ClockifyIt.Integration.Redmine/*.csproj", "aspnet-core/src/Sg.ClockifyIt.Integration.Redmine/"]
COPY ["aspnet-core/test/Sg.ClockifyIt.Application.Tests/*.csproj", "aspnet-core/test/Sg.ClockifyIt.Application.Tests/"]
COPY ["aspnet-core/test/Sg.ClockifyIt.Domain.Tests/*.csproj", "aspnet-core/test/Sg.ClockifyIt.Domain.Tests/"]
COPY ["aspnet-core/test/Sg.ClockifyIt.EntityFrameworkCore.Tests/*.csproj", "aspnet-core/test/Sg.ClockifyIt.EntityFrameworkCore.Tests/"]
COPY ["aspnet-core/test/Sg.ClockifyIt.HttpApi.Client.ConsoleTestApp/*.csproj", "aspnet-core/test/Sg.ClockifyIt.HttpApi.Client.ConsoleTestApp/"]
COPY ["aspnet-core/test/Sg.ClockifyIt.TestBase/*.csproj", "aspnet-core/test/Sg.ClockifyIt.TestBase/"]

# Restore as distinct layer
WORKDIR "/aspnet-core"
RUN dotnet restore

# Copy project(s)
COPY ["aspnet-core/", "."]

WORKDIR "/aspnet-core/src/Sg.ClockifyIt.HttpApi.Host"
RUN dotnet build --no-restore -c Release

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sg.ClockifyIt.HttpApi.Host.dll"]