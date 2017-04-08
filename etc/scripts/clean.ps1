[string]$workspace = "$PSScriptRoot\..\.."

ForEach ($directory in (Get-ChildItem -Path "$workspace\" -Directory -Include @("bin", "obj") -Recurse)) {
    If (Test-Path -Path $directory.FullName) {
        Remove-Item -Path $directory.FullName -Recurse -Force
    }
}