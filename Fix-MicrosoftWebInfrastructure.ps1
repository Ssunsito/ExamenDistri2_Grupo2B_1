# ============================================
# Script para reparar error:
# "No se puede cargar el archivo o ensamblado 'Microsoft.Web.Infrastructure'"
# Autor: ChatGPT
# ============================================

Write-Host "=== Reparando proyecto ASP.NET ===" -ForegroundColor Cyan

# 1️⃣ Detener Visual Studio si está abierto
Write-Host "Cerrando Visual Studio si está abierto..." -ForegroundColor Yellow
Get-Process devenv -ErrorAction SilentlyContinue | Stop-Process -Force

# 2️⃣ Limpiar carpetas bin/obj
Write-Host "Eliminando carpetas bin y obj..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force

# 3️⃣ Limpiar carpeta packages (si existe)
if (Test-Path ".\packages") {
    Write-Host "Eliminando carpeta packages..." -ForegroundColor Yellow
    Remove-Item ".\packages" -Recurse -Force
}

# 4️⃣ Restaurar paquetes NuGet
Write-Host "Restaurando paquetes NuGet..." -ForegroundColor Yellow
nuget restore

# 5️⃣ Reinstalar Microsoft.Web.Infrastructure
Write-Host "Reinstalando Microsoft.Web.Infrastructure..." -ForegroundColor Yellow
dotnet add package Microsoft.Web.Infrastructure --version 1.0.0

# 6️⃣ Mostrar confirmación
Write-Host "Reparación completada. Abre Visual Studio y reconstruye la solución." -ForegroundColor Green
