# Build stage: uses the full SDK image (has compilers, MSBuild, NuGet client)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only .csproj files first — Docker caches this layer separately from
# source code, so a code-only change won't force a full package re-restore.
# NOTE: no "src/" prefix — VS's Solution Explorer shows a virtual "src"
# folder that only exists inside the .sln file, not on disk.
COPY ["ECommerce.API/ECommerce.API.csproj", "ECommerce.API/"]
COPY ["ECommerce.APP/ECommerce.APP.csproj", "ECommerce.APP/"]
COPY ["ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]

# Central Package Management: Directory.Packages.props/Directory.Build.props
# live at the repo root and control package versions for every project, so
# they must be copied too or restore will fail looking for version numbers.
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

RUN dotnet restore "ECommerce.API/ECommerce.API.csproj"

# Now copy everything else and publish
COPY . .
WORKDIR /src/ECommerce.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage: much smaller image, no SDK/compiler bloat
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render assigns a random port via the PORT env var at container start —
# Program.cs binds to it via UseUrls($"http://+:{port}")
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]