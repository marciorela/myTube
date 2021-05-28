#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.Config","/"]
COPY ["myTube.WS/myTube.WS.csproj", "myTube.WS/"]
COPY ["myTube.Domain/myTube.Domain.csproj", "myTube.Domain/"]
COPY ["myTube.Services/myTube.Services.csproj", "myTube.Services/"]
COPY ["myTube.Data/myTube.Data.csproj", "myTube.Data/"]
RUN dotnet restore "myTube.WS/myTube.WS.csproj"
COPY . .
WORKDIR "/src/myTube.WS"
RUN dotnet build "myTube.WS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "myTube.WS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "myTube.WS.dll"]