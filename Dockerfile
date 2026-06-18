FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["WebApiShop/WebApiShop.csproj", "WebApiShop/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repositeries/Repositeries.csproj", "Repositeries/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["DTOs/DTOs.csproj", "DTOs/"]

RUN dotnet restore "WebApiShop/WebApiShop.csproj"

COPY . .
WORKDIR /src/WebApiShop
RUN dotnet publish "WebApiShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApiShop.dll"]
