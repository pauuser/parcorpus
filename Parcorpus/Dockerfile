# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443

ARG CONFIGURATION
RUN echo ${CONFIGURATION}

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ./*.sln ./
COPY . .

RUN dotnet restore

# copy everything else and build app
COPY . .

ARG CONFIGURATION
RUN echo ${CONFIGURATION}

WORKDIR /src/src/Parcorpus.API/Parcorpus.API
RUN dotnet build "Parcorpus.API.csproj" -c "$CONFIGURATION" -o /app/build 

FROM build AS publish
RUN dotnet publish "Parcorpus.API.csproj" -c "$CONFIGURATION" -o /app/publish

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parcorpus.API.dll"]
