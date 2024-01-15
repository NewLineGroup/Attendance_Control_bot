FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["AttendanceControlBot/AttendanceControlBot.csproj", "AttendanceControlBot/"]
RUN dotnet restore "AttendanceControlBot/AttendanceControlBot.csproj"
COPY . .
WORKDIR "/src/AttendanceControlBot"
RUN dotnet build "AttendanceControlBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AttendanceControlBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AttendanceControlBot.dll"]
