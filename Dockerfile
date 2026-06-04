ARG REGISTRY=mcr.microsoft.com

# Runtime Shell Target (Uses light aspnet image)
FROM ${REGISTRY}/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# The Heavy Construction Arena (FIXED: Uses sdk image to compile!)
FROM ${REGISTRY}/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy multi-project solution structure mapping layers
COPY *.slnx ./
COPY ["FreakyFashion/FreakyFashion.csproj", "FreakyFashion/"]
COPY ["DomainLayer/DomainLayer.csproj", "DomainLayer/"]
COPY ["ApplicationLayer/ApplicationLayer.csproj", "ApplicationLayer/"]
COPY ["RepositoriesDependencyInjectionProject/RepositoriesDependencyInjectionProject.csproj", "RepositoriesDependencyInjectionProject/"]
COPY ["InfrastructureLayer/InfrastructureLayer.csproj", "InfrastructureLayer/"]

# COPY ["FreakyFashionClient/FreakyFashionClient.csproj", "FreakyFashionClient/"]

# Restore the mapped projects
RUN dotnet restore

# Copy remaining source code files and compile
COPY . .
WORKDIR "/src/FreakyFashion"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/FreakyFashion"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false --no-dependencies

# Assembling final secure container box
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreakyFashion.dll"]