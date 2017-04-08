[string]$workspace = "$PSScriptRoot\..\..\src"
[string]$packageID = "System.Data.JsonRpc"

& dotnet restore "$workspace\$packageID.sln"

If ($LASTEXITCODE -ne 0) {
    Throw "Restore command failed with code $LASTEXITCODE"
}

& dotnet build "$workspace\$packageID.sln" --configuration Release

If ($LASTEXITCODE -ne 0) {
    Throw "Build command failed with code $LASTEXITCODE"
}

& dotnet test "$workspace\$packageID.Tests\$packageID.Tests.csproj" --configuration Release

If ($LASTEXITCODE -ne 0) {
    Throw "Test command failed with code $LASTEXITCODE"
}

& dotnet pack "$workspace\$packageID\$packageID.csproj" --configuration Release

If ($LASTEXITCODE -ne 0) {
    Throw "Pack command failed with code $LASTEXITCODE"
}