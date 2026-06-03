FROM ://microsoft.com AS builder
WORKDIR /src

COPY *.slnx ./
COPY ["FreakyFashion/FreakyFashion.csproj", "FreakyFashion/"]
COPY ["DomainLayer/DomainLayer.csproj", "DomainLayer/"]
COPY ["ApplicationLayer/ApplicationLayer.csproj", "ApplicationLayer/"]
COPY ["RepositoriesDependencyInjectionProject/RepositoriesDependencyInjectionProject.csproj", "RepositoriesDependencyInjectionProject/"]
COPY ["InfrastructureLayer/InfrastructureLayer.csproj", "InfrastructureLayer/"]
RUN dotnet restore

COPY . .
WORKDIR "/src/FreakyFashion"
RUN dotnet build -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM ://microsoft.com AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreakyFashion.dll"]
Use code with caution.