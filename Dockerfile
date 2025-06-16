FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Project-Echo.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build "Project-Echo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project-Echo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ensure docs directory exists and has correct permissions
RUN mkdir -p /app/docs && chmod -R 755 /app/docs
ENTRYPOINT ["dotnet", "Project-Echo.dll"] 