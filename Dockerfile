#FROM ://microsoft.com AS builder
#WORKDIR /src
FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ://microsoft.com AS build
WORKDIR /src

COPY *.slnx ./
COPY ["FreakyFashion/FreakyFashion.csproj", "FreakyFashion/."]
COPY ["FreakyFashion/DomainLayer.csproj", "FreakyFashion/."]
COPY ["FreakyFashion/ApplicationLayer.csproj", "FreakyFashion/."]
COPY ["FreakyFashion/RepositoriesDependencyInjectionProject.csproj", "FreakyFashion/."]
COPY ["FreakyFashion/InfrastructureLayer.csproj", "FreakyFashion/."]

RUN dotnet restore "FreakyFashion/FreakyFashion.csproj"

COPY . .
#WORKDIR "/src/FreakyFashion"
RUN dotnet build "FreakyFashion/FreakyFashion.csproj" -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish "FreakyFashion/FreakyFashion.csproj" -c Release -o /app/publish /p:UseAppHost=false
#RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

#FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
FROM ://microsoft.com AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreakyFashion.dll"]