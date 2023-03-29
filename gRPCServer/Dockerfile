FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 81
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["gRPCServer/gRPCServer.csproj", "gRPCServer/"]
COPY ["WebContract/WebContract.csproj", "WebContract/"]
COPY ["ProtoContract/*", "ProtoContract/"]
RUN dotnet restore "gRPCServer/gRPCServer.csproj"
COPY . .
WORKDIR "/src/gRPCServer"
RUN dotnet build "gRPCServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gRPCServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gRPCServer.dll"]