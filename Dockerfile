FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app


COPY *.slnx ./
COPY ServiceHub.Domain/*.csproj ./ServiceHub.Domain/
COPY ServiceHub.Application/*.csproj ./ServiceHub.Application/
COPY ServiceHub.Infrastructure/*.csproj ./ServiceHub.Infrastructure/
COPY ServiceHub.Web/*.csproj ./ServiceHub.Web/
COPY ServiceHub.Tests/*.csproj ./ServiceHub.Tests/


RUN dotnet restore

COPY . ./


RUN dotnet publish -c Release -o out --no-restore

# Step 2: Build runtime image using ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ServiceHub.Web.dll"]