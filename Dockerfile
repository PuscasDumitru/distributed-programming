#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Insta/Insta.csproj", "Insta/"]
RUN dotnet restore "Insta/Insta.csproj"
COPY . .
WORKDIR "/src/Insta"
RUN dotnet build "Insta.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insta.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insta.dll"]