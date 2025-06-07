FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Project-Echo.csproj", "."]
RUN dotnet restore "Project-Echo.csproj"
COPY . .
RUN dotnet build "Project-Echo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project-Echo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project-Echo.dll"] 