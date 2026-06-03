# Declare the arguments: '--build-arg REGISTRY=mcr.microsoft.com' from your YAML file
ARG REGISTRY

# Base Runtime Stage (Uses aspnet image)
FROM ${REGISTRY}/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build Stage (Uses sdk image to compile code)
FROM ${REGISTRY}/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY ["FreakyFashion/FreakyFashion.csproj", "FreakyFashion/"]
COPY ["FreakyFashion/DomainLayer.csproj", "FreakyFashion/"]
COPY ["FreakyFashion/ApplicationLayer.csproj", "FreakyFashion/"]
COPY ["FreakyFashion/RepositoriesDependencyInjectionProject.csproj", "FreakyFashion/"]
COPY ["FreakyFashion/InfrastructureLayer.csproj", "FreakyFashion/"]

RUN dotnet restore "FreakyFashion/FreakyFashion.csproj"

# Copy remaining source code and build
COPY . .
#WORKDIR "/src/FreakyFashion"
RUN dotnet build "FreakyFashion/FreakyFashion.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "FreakyFashion/FreakyFashion.csproj" -c Release -o /app/publish /p:UseAppHost=false
#RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreakyFashion.dll"]