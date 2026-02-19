param(
[switch]$SkipTests,
[switch]$skiptest,
[switch]$nonDestructive
)
$ErrorActionPreference = "Stop"

dotnet build
if ($LASTEXITCODE -ne 0)
{
    Write-Host "==> Build failed."
    exit $LASTEXITCODE
}
if ($nonDestructive)
{
    Write-Host "==> Skipping volume removal due to non-destructive flag."
    docker compose -f .\docker-compose.yml down
}
else
{
    Write-Host "==> Bringing down compose stack (remove volumes)..."

    docker compose -f .\docker-compose.yml down -v
}
Write-Host "==> Bringing up compose stack (rebuild images)..."
docker compose -f docker-compose.yml up --build

Write-Host "==> Done."