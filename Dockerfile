FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ManageSystem/ManageSystem.csproj", "ManageSystem/"]
COPY ["Operation/Operation.csproj", "Operation/"]
COPY ["Entity/Entity.csproj", "Entity/"]

RUN dotnet restore "ManageSystem/ManageSystem.csproj"
COPY . .
WORKDIR "/src/ManageSystem"
RUN dotnet build "ManageSystem.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ManageSystem.csproj" -c Release -o /app

FROM base AS final
COPY ManageSystem/ManageSystem.xml /app
COPY sources.list /etc/apt/sources.list

RUN apt-get update -y && apt-get install -y libgdiplus && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManageSystem.dll"]