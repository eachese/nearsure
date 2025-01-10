# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["GameOfLifeApi.csproj", "./"]
RUN dotnet restore

# Copy the rest of the application source code
COPY . ./
WORKDIR /app
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the application port
EXPOSE 5000
ENTRYPOINT ["dotnet", "GameOfLifeApi.dll"]
