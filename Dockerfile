FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o dist

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /APP
COPY --from=build-env /App/dist .
ENTRYPOINT [ "dotnet", "Api.dll" ]