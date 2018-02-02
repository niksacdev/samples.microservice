FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/samples.microservice.core/out ./
COPY --from=build-env /app/samples.microservice.entities/out ./
COPY --from=build-env /app/samples.microservice.repository/out ./
COPY --from=build-env /app/samples.microservice.api/out ./

ENTRYPOINT ["dotnet", "samples.microservice.api.dll"]