#!/bin/bash
set -e

ROOT="$(cd "$(dirname "$0")" && pwd)"

echo "Iniciando banco de dados..."
docker-compose -f "$ROOT/docker-compose.yml" up -d

echo "Aguardando PostgreSQL ficar pronto..."
tries=0
while [ "$(docker inspect --format='{{.State.Health.Status}}' "$(docker-compose -f "$ROOT/docker-compose.yml" ps -q db)")" != "healthy" ]; do
  sleep 2
  tries=$((tries + 1))
  if [ $tries -ge 15 ]; then
    echo "Banco nao ficou pronto a tempo. Verifique o Docker."
    exit 1
  fi
done

echo "Banco pronto. Iniciando API (migrations e seed automaticos)..."
powershell -NoExit -Command "Set-Location '$ROOT/Api'; dotnet run" &
API_PID=$!

echo "Aguardando API subir..."
sleep 8

echo "Iniciando Blazor..."
powershell -NoExit -Command "Set-Location '$ROOT/Blazor'; dotnet run" &

sleep 5
echo "Abrindo navegador em http://localhost:5200 ..."
start "http://localhost:5200"

wait $API_PID
