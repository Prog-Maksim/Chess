FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

COPY .env ./.env
COPY Image ./Image
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Backend.dll"]
