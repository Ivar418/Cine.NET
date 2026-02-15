# powershell
param(
    [switch]$SkipTests,
    [switch]$skiptest,
    [switch]$nonDestructive
)
$ErrorActionPreference = "Stop"

dotnet.exe build
if ($LASTEXITCODE -ne 0) {
Write-Host "==> Build failed."
exit $LASTEXITCODE
}
Write-Host "==> Bringing down compose stack (remove volumes)..."
if ($nonDestructive) {
Write-Host "==> Skipping volume removal due to non-destructive flag."
docker compose -f .\docker-compose.yml down
} else {
docker compose -f .\docker-compose.yml down -v
}
Write-Host "==> Bringing up compose stack (rebuild images)..."
docker compose -f docker-compose.yml up --build

Write-Host "==> Done."