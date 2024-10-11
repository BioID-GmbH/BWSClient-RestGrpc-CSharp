#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8071

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BioID.RestGrpcForwarder/BioID.RestGrpcForwarder.csproj", "BioID.RestGrpcForwarder/"]
RUN dotnet restore "./BioID.RestGrpcForwarder/BioID.RestGrpcForwarder.csproj"
COPY . .
WORKDIR "/src/BioID.RestGrpcForwarder"
RUN dotnet build "./BioID.RestGrpcForwarder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BioID.RestGrpcForwarder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BioID.RestGrpcForwarder.dll"]