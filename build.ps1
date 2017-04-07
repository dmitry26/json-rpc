ForEach ($file in (Get-ChildItem -Path "$PSScriptRoot\src\" -File -Filter "*.sln" -Recurse)) {
    & dotnet restore $file.FullName
    If ($LASTEXITCODE -ne 0) {
        Throw "Restore command failed"
    }
    & dotnet build $file.FullName --configuration Release
    If ($LASTEXITCODE -ne 0) {
        Throw "Build command failed"
    }
} 
ForEach ($file in (Get-ChildItem -Path "$PSScriptRoot\src\" -File -Filter "*.Tests.csproj" -Recurse)) {
    & dotnet test $file.FullName --configuration Release
    If ($LASTEXITCODE -ne 0) {
        Throw "Test command failed"
    }
} 