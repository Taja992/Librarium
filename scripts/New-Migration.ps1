# Usage: .\scripts\New-Migration.ps1 -Name "DescriptionHere"
# Example: .\scripts\New-Migration.ps1 -Name "AddAuthorTable"
param (
    [Parameter(Mandatory = $true)]
    [string]$Name
)

$repoRoot = Resolve-Path "$PSScriptRoot\.."
$sqlDir = "$repoRoot\migrations\sql"

# Determine next version number by scanning existing V{n}__*.sql files
$existing = Get-ChildItem -Path $sqlDir -Filter "V*.sql" -ErrorAction SilentlyContinue
$nextNumber = 1

if ($existing) {
    $versions = $existing | ForEach-Object {
        if ($_.Name -match '^V(\d+)__') { [int]$Matches[1] }
    } | Sort-Object -Descending

    if ($versions) {
        $nextNumber = $versions[0] + 1
    }
}

$version = "V{0:D3}" -f $nextNumber
$sqlFileName = "${version}__${Name}.sql"
$sqlOutputPath = "$sqlDir\$sqlFileName"

Write-Host "Creating migration '$Name'..." -ForegroundColor Cyan
dotnet ef migrations add $Name --project "$repoRoot\src\Librarium.Data" --startup-project "$repoRoot\src\Librarium.Api"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Migration add failed. Aborting."
    exit 1
}

Write-Host "Generating SQL script -> migrations/sql/$sqlFileName" -ForegroundColor Cyan
dotnet ef migrations script --idempotent --project "$repoRoot\src\Librarium.Data" --startup-project "$repoRoot\src\Librarium.Api" -o $sqlOutputPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "SQL script generation failed."
    exit 1
}

Write-Host "Done: $sqlFileName" -ForegroundColor Green
