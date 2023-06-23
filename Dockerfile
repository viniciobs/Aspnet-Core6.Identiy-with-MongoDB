FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Infra/Infra.csproj", "Infra/"]
RUN dotnet restore "Infra/Infra.csproj"
COPY ["src/Infra/.", "Infra/"]
RUN dotnet build "/src/Infra/Infra.csproj" -c Release -o /app/build

COPY ["src/Identity/Identity.csproj", "Identity/"]
RUN dotnet restore "Identity/Identity.csproj"
COPY ["src/Identity/.", "Identity/"]
RUN dotnet build "/src/Identity/Identity.csproj" -c Release -o /app/build

COPY ["src/Ioc/Ioc.csproj", "Ioc/"]
RUN dotnet restore "Ioc/Ioc.csproj"
COPY ["src/Ioc/.", "Ioc/"]
RUN dotnet build "/src/Ioc/Ioc.csproj" -c Release -o /app/build

COPY ["src/Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj" 
COPY ["src/Api/.", "Api/"]
RUN dotnet build "/src/Api/Api.csproj" -c Release -o /app/build

WORKDIR "/src/Api"

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Api.dll" ]