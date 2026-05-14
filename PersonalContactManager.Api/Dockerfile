FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PersonalContactManager.Api.sln .
COPY PersonalContactManager.Api/PersonalContactManager.Api.csproj PersonalContactManager.Api/
COPY PersonalContactManager.Application/PersonalContactManager.Application.csproj PersonalContactManager.Application/
COPY PersonalContactManager.Domain/PersonalContactManager.Domain.csproj PersonalContactManager.Domain/
COPY PersonalContactManager.Infrastructure/PersonalContactManager.Infrastructure.csproj PersonalContactManager.Infrastructure/
COPY PersonalContactManager.Tests/PersonalContactManager.Tests.csproj PersonalContactManager.Tests/
RUN dotnet restore

COPY . .
RUN dotnet publish PersonalContactManager.Api/PersonalContactManager.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PersonalContactManager.Api.dll"]
