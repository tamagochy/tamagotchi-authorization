FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY /Tamagotchi.Authorization.csproj Tamagotchi.Authorization/
RUN dotnet restore Tamagotchi.Authorization/Tamagotchi.Authorization.csproj
WORKDIR /src/Tamagotchi.Authorization
COPY . .
RUN dotnet build Tamagotchi.Authorization.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Tamagotchi.Authorization.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tamagotchi.Authorization.dll"]
