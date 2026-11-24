# Imagen para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# El contexto de compilación debe ser la raíz de la solución
COPY . .
RUN dotnet publish "Aeropuerto.WEB/Cafeteria.WEB.csproj" -c Release -o /app/publish

# Imagen ligera para ejecutar la web
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cafeteria.WEB.dll"]
