FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "DhoobiGO.API/DhoobiGO.API.csproj"
RUN dotnet publish "DhoobiGO.API/DhoobiGO.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5286
ENTRYPOINT ["dotnet", "DhoobiGO.API.dll"]
