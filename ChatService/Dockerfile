#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ChatService/ChatService.csproj", "ChatService/"]
RUN dotnet restore "ChatService/ChatService.csproj"
COPY . .
WORKDIR "/src/ChatService"
RUN dotnet build "ChatService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.dll", "--server.urls", "http://+:80;https://+:443"]