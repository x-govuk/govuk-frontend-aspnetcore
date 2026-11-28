#!/usr/bin/env pwsh

[CmdletBinding(SupportsShouldProcess)]
param()

$ErrorActionPreference = "Stop"

# The packages the samples restore from ../packages
$packageIds = @(
    "GovUk.Frontend.AspNetCore",
    "GovUk.Frontend.AspNetCore.Testing",
    "GovUk.Frontend.AspNetCore.Testing.Xunit.v3",
    "GovUk.Frontend.AspNetCore.Testing.Playwright"
)

$globalPackagesOutput = & dotnet nuget locals global-packages --list
if ($LASTEXITCODE -ne 0) {
    throw "'dotnet nuget locals global-packages --list' failed with exit code $LASTEXITCODE."
}

$match = $globalPackagesOutput | Select-String -Pattern '^\s*global-packages:\s*(.+?)\s*$' | Select-Object -First 1
if (-not $match) {
    throw "Could not determine the global packages folder."
}
$globalPackageCache = $match.Matches[0].Groups[1].Value

Write-Verbose "Global packages folder: $globalPackageCache"

foreach ($packageId in $packageIds) {
    $packageCache = Join-Path $globalPackageCache $packageId.ToLowerInvariant()
    if (-not (Test-Path $packageCache)) {
        Write-Host "$packageId is not in the global packages cache."
        continue
    }

    # Cached versions are directory names; a hyphen means the version has a pre-release label.
    $preReleaseVersions = Get-ChildItem $packageCache -Directory | Where-Object { $_.Name -like "*-*" }

    if (-not $preReleaseVersions) {
        Write-Host "No pre-release versions of $packageId found in the global packages cache."
        continue
    }

    foreach ($version in $preReleaseVersions) {
        if ($PSCmdlet.ShouldProcess($version.FullName, "Remove")) {
            Remove-Item $version.FullName -Recurse -Force
            Write-Host "Removed $packageId $($version.Name) from the global packages cache."
        }
    }
}
