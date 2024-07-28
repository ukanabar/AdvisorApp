# Use the official .NET image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AdvisorApp/AdvisorApp.csproj", "AdvisorApp/"]
RUN dotnet restore "AdvisorApp/AdvisorApp.csproj"
COPY . .
WORKDIR "/src/AdvisorApp"
RUN dotnet build "AdvisorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdvisorApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdvisorApp.dll"]
