FROM mcr.microsoft.com/dotnet/sdk:10.0.101 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Abysalto.CartApi/Abysalto.CartApi.csproj
RUN dotnet publish src/Abysalto.CartApi/Abysalto.CartApi.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0.1
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Abysalto.CartApi.dll"]
