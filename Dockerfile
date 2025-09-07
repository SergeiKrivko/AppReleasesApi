# Build stage for Angular
FROM node:22.12-alpine AS angular-build
WORKDIR /src

# Copy UI files
COPY UI/package*.json ./
RUN npm ci

# Copy Angular source and build
COPY UI/ .
RUN npm run build -- --configuration production

# Build stage for .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src

# Copy only API project files first
COPY AppReleases.Api/*.csproj ./AppReleases.Api/
COPY AppReleases.Application/*.csproj ./AppReleases.Application/
COPY AppReleases.Core/*.csproj ./AppReleases.Core/
COPY AppReleases.DataAccess/*.csproj ./AppReleases.DataAccess/
COPY AppReleases.Models/*.csproj ./AppReleases.Models/
COPY AppReleases.S3/*.csproj ./AppReleases.S3/

RUN dotnet restore AppReleases.Api

# Copy remaining source code
COPY . .

# Copy Angular build output to wwwroot
COPY --from=angular-build /src/dist/ ./AppReleases.Api/wwwroot/

# Build .NET application
RUN dotnet publish AppReleases.Api/ -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=dotnet-build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8081;3000
EXPOSE 3000
ENTRYPOINT ["dotnet", "AppReleases.Api.dll"]