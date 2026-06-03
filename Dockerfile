# Declare the arguments: '--build-arg REGISTRY=mcr.microsoft.com' from your YAML file
ARG REGISTRY=mcr.microsoft.com

# Base Runtime Stage (Uses aspnet image)
FROM ${REGISTRY}/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Build Stage (Uses sdk image to compile code)
FROM ${REGISTRY}/dotnet/aspnet:10.0 AS build
WORKDIR /src

COPY  *.slnx ./
COPY ["FreakyFashionAPI/FreakyFashion.csproj", "./"]
COPY ["FreakyFashionAPI/DomainLayer.csproj", "./"]
COPY ["FreakyFashionAPI/ApplicationLayer.csproj", "./"]
COPY ["FreakyFashionAPI/RepositoriesDependencyInjectionProject.csproj", "./"]
COPY ["FreakyFashionAPI/InfrastructureLayer.csproj", "./"]

RUN dotnet restore "FreakyFashion.csproj"

# Copy remaining source code and build
COPY FreakyFashionAPI/ .
#WORKDIR "/src/FreakyFashion"
RUN dotnet build "FreakyFashion.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "FreakyFashion.csproj" -c Release -o /app/publish /p:UseAppHost=false
#RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreakyFashion.dll"]