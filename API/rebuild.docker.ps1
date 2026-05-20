param(
    [switch]$SkipTests,
    [switch]$skiptest,
    [switch]$nonDestructive
)
$ErrorActionPreference = "Stop"

# --- TLS / HTTPS Setup Check ---
$certPath = Join-Path $PSScriptRoot "https\aspnetapp.pfx"
if (-not (Test-Path $certPath)) {
    Write-Host "==> TLS certificate not found at $certPath" -ForegroundColor Yellow
    $choice = Read-Host "Would you like to generate a self-signed certificate automatically? (y/n)"
    if ($choice -eq 'y') {
        # Try to get password from .env
        $envFile = Join-Path $PSScriptRoot ".env"
        $password = "yourpassword" # default fallback
        if (Test-Path $envFile) {
            $passLine = Get-Content $envFile | Select-String "KESTREL_CERT_PASSWORD="
            if ($passLine) {
                $lineStr = $passLine.ToString()
                $password = $lineStr.Substring($lineStr.IndexOf('=') + 1).Trim()
            }
        }
        
        Write-Host "==> Generating certificate at $certPath..."
        $certDir = Split-Path -Parent $certPath
        if (-not (Test-Path $certDir)) {
            New-Item -ItemType Directory -Path $certDir | Out-Null
        }
        
        dotnet dev-certs https -ep $certPath -p $password
        if ($LASTEXITCODE -ne 0) {
            Write-Host "==> Failed to generate certificate." -ForegroundColor Red
            exit $LASTEXITCODE
        }
        dotnet dev-certs https --trust
        Write-Host "==> Certificate generated and trusted at $certPath." -ForegroundColor Green
    }
}
# -------------------------------

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
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host "==> Failed to bring down compose stack."
        exit $LASTEXITCODE
    }
}
Write-Host "==> Bringing up compose stack (rebuild images)..."
docker compose -f docker-compose.yml up --build -d
if ($LASTEXITCODE -ne 0)
{
    Write-Host "==> Failed to bring up compose stack."
    exit $LASTEXITCODE
}
#Start-Process "http://localhost:8080/swagger/"
Write-Host "==> Done."
