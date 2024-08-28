FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7005

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReceiverWebAPI/ReceiverWebAPI.csproj", "ReceiverWebAPI/"]
RUN dotnet restore "./ReceiverWebAPI/ReceiverWebAPI.csproj"
COPY . .
WORKDIR "/src/ReceiverWebAPI"
RUN dotnet build "./ReceiverWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReceiverWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReceiverWebAPI.dll"]
