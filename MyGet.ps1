$project = "./src/AspNetRouteVersions/AspNetRouteVersions.csproj"

dotnet restore $project --no-cache
dotnet build $project --no-restore -c Release
dotnet pack $project --no-build -c Release --output '../..'