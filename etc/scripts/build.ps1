[string]$workspace = "$PSScriptRoot\..\.."
[string]$configuration = "Release"

& dotnet restore "$workspace\src\"

If ($LASTEXITCODE -ne 0) {
    Throw "Restore command failed with code $LASTEXITCODE"
}

& dotnet build "$workspace\src\" --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Build command failed with code $LASTEXITCODE"
}

& dotnet test "$workspace\src\System.Data.JsonRpc.Tests\System.Data.JsonRpc.Tests.csproj" --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Test command failed with code $LASTEXITCODE"
}

& dotnet pack "$workspace\src\System.Data.JsonRpc\System.Data.JsonRpc.csproj" --no-build --configuration $configuration

If ($LASTEXITCODE -ne 0) {
    Throw "Pack command failed with code $LASTEXITCODE"
}