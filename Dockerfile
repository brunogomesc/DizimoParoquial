# Base image for runtime (used in final stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Optional: define UID if needed
# ARG APP_UID=1000
# USER ${APP_UID}

# Image used for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GIT_SHA=ec95009fc54734c1dc1e1192012b38c13e763822
LABEL git_sha=$GIT_SHA

WORKDIR /src

# Copy and restore dependencies
COPY ["DizimoParoquial/DizimoParoquial.csproj", "DizimoParoquial/"]
RUN dotnet restore "./DizimoParoquial/DizimoParoquial.csproj"

# Copy the entire source
COPY . .

WORKDIR "/src/DizimoParoquial"

# Build with limited memory usage (1 thread to reduce memory use)
RUN dotnet build "./DizimoParoquial.csproj" -c $BUILD_CONFIGURATION -m:1 -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DizimoParoquial.csproj" -c $BUILD_CONFIGURATION -m:1 -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Optional: if you want to run as non-root
# USER ${APP_UID}

ENTRYPOINT ["dotnet", "DizimoParoquial.dll"]
