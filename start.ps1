$root = $PSScriptRoot

Write-Host "Iniciando banco de dados..." -ForegroundColor Cyan
docker-compose -f "$root\docker-compose.yml" up -d

Write-Host "Aguardando PostgreSQL ficar pronto..." -ForegroundColor Cyan
$tries = 0
do {
    Start-Sleep -Seconds 2
    $status = docker inspect --format="{{.State.Health.Status}}" (docker-compose -f "$root\docker-compose.yml" ps -q db) 2>$null
    $tries++
} while ($status -ne "healthy" -and $tries -lt 15)

if ($status -ne "healthy") {
    Write-Host "Banco nao ficou pronto a tempo. Verifique o Docker." -ForegroundColor Red
    exit 1
}

Write-Host "Banco pronto. Iniciando API..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root\Api'; dotnet run"

Write-Host "Aguardando API subir (migrations e seed inclusos)..." -ForegroundColor Cyan
Start-Sleep -Seconds 8

Write-Host "Iniciando Blazor..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root\Blazor'; dotnet run"

Start-Sleep -Seconds 5
Write-Host "Abrindo navegador em http://localhost:5200 ..." -ForegroundColor Green
Start-Process "http://localhost:5200"
