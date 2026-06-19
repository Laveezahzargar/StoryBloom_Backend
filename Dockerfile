FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

WORKDIR /src

COPY . .

RUN dotnet restore P9_Blog_Generator_AI_Backend.csproj

RUN dotnet publish P9_Blog_Generator_AI_Backend.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "P9_Blog_Generator_AI_Backend.dll"]