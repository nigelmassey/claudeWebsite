FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY claudeWebsite.Shared/ ./claudeWebsite.Shared/
COPY claudeWebsite.Api/ ./claudeWebsite.Api/
WORKDIR /src/claudeWebsite.Api
RUN dotnet publish claudeWebsite.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "claudeWebsite.Api.dll"]
