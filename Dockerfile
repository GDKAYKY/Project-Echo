FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Project-Echo.csproj", "./"]
RUN dotnet restore
COPY . .
# Copy docs to the correct location
COPY docs/ /src/Project-Echo/docs/
RUN dotnet build "Project-Echo.csproj" -c Release -o /app/build

FROM build AS publish
# Ensure docs directory is included in publish
RUN dotnet publish "Project-Echo.csproj" -c Release -o /app/publish /p:IncludeContentInSingleFile=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy documentation files from the build stage
COPY --from=build /src/Project-Echo/docs /app/docs
# Ensure docs directory exists and has correct permissions
RUN chmod -R 755 /app/docs
ENTRYPOINT ["dotnet", "Project-Echo.dll"] 