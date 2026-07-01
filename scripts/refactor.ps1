# refactor_complete.ps1
$oldName = "CleanArchTemplate"
$newName = "FinanceApp"

Write-Host "=== REFACTORIZANDO PROYECTO ===" -ForegroundColor Cyan
Write-Host "De: $oldName" -ForegroundColor Yellow
Write-Host "A: $newName" -ForegroundColor Yellow
Write-Host ""

function Update-FileContent {
    param($path, $old, $new)
    if (Test-Path $path) {
        $content = Get-Content $path -Raw -Encoding UTF8
        $updated = $content -replace $old, $new
        if ($content -ne $updated) {
            [System.IO.File]::WriteAllText($path, $updated, [System.Text.Encoding]::UTF8)
            Write-Host "  ✅ Actualizado: $(Split-Path $path -Leaf)" -ForegroundColor Green
        }
    }
}

# 1. Renombrar archivos .csproj
Write-Host "📁 Renombrando archivos de proyecto..." -ForegroundColor Cyan
Get-ChildItem -Recurse -Filter "*.csproj" | ForEach-Object {
    $newNameFile = $_.Name -replace $oldName, $newName
    if ($_.Name -ne $newNameFile) {
        Rename-Item -Path $_.FullName -NewName $newNameFile
        Write-Host "  📄 $($_.Name) -> $newNameFile" -ForegroundColor Yellow
    }
}

# 2. Renombrar carpetas (de adentro hacia afuera para evitar problemas)
Write-Host "`n📁 Renombrando carpetas..." -ForegroundColor Cyan
$folders = Get-ChildItem -Directory | Where-Object {$_.Name -like "*$oldName*"} | Sort-Object -Property Name -Descending
foreach ($folder in $folders) {
    $newFolderName = $folder.Name -replace $oldName, $newName
    if ($folder.Name -ne $newFolderName) {
        Rename-Item -Path $folder.FullName -NewName $newFolderName
        Write-Host "  📂 $($folder.Name) -> $newFolderName" -ForegroundColor Yellow
    }
}

# 3. Actualizar contenido de .csproj y .sln
Write-Host "`n📝 Actualizando contenido de archivos de proyecto..." -ForegroundColor Cyan
Get-ChildItem -Recurse -Include "*.csproj", "*.sln" | ForEach-Object {
    Update-FileContent -path $_.FullName -old $oldName -new $newName
}

# 4. Actualizar namespaces en archivos .cs
Write-Host "`n📝 Actualizando namespaces..." -ForegroundColor Cyan
Get-ChildItem -Recurse -Filter "*.cs" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8
    $original = $content

    # Reemplazar namespace completo
    $content = $content -replace "namespace\s+$oldName", "namespace $newName"
    $content = $content -replace "using\s+$oldName\.", "using $newName."
    $content = $content -replace "using\s+$oldName;", "using $newName;"

    if ($content -ne $original) {
        [System.IO.File]::WriteAllText($_.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "  ✅ $(Split-Path $_.FullName -Leaf)" -ForegroundColor Green
    }
}

# 5. Renombrar el archivo .sln
Write-Host "`n📁 Renombrando archivo de solución..." -ForegroundColor Cyan
$slnFile = Get-ChildItem -Filter "*.sln" | Select-Object -First 1
if ($slnFile) {
    $newSlnName = $slnFile.Name -replace $oldName, $newName
    if ($slnFile.Name -ne $newSlnName) {
        Rename-Item -Path $slnFile.FullName -NewName $newSlnName
        Write-Host "  📄 $($slnFile.Name) -> $newSlnName" -ForegroundColor Yellow
        $slnFile = Get-Item (Join-Path $slnFile.DirectoryName $newSlnName)
    }
}

# 6. Agregar proyectos a la solución (CORREGIDO)
Write-Host "`n🔄 Reconstruyendo solución..." -ForegroundColor Cyan
if ($slnFile) {
    # Primero limpiamos la solución (remover todos los proyectos)
    Write-Host "  Limpiando solución..." -ForegroundColor Yellow

    # Obtener la lista actual de proyectos
    $projectList = dotnet sln $slnFile.Name list
    $projectList = $projectList | Where-Object {$_ -match "\.csproj$"}

    foreach ($proj in $projectList) {
        $proj = $proj.Trim()
        if ($proj) {
            Write-Host "  Removiendo: $proj" -ForegroundColor Gray
            dotnet sln $slnFile.Name remove $proj 2>$null
        }
    }

    # Agregar todos los proyectos de nuevo
    $projects = Get-ChildItem -Recurse -Filter "*.csproj"
    foreach ($proj in $projects) {
        $relativePath = Resolve-Path -Relative $proj.FullName
        $relativePath = $relativePath -replace "\.\\", ""
        Write-Host "  Agregando: $relativePath" -ForegroundColor Yellow
        dotnet sln $slnFile.Name add $relativePath
    }
}

# 7. Limpiar y restaurar
Write-Host "`n🧹 Limpiando y restaurando paquetes..." -ForegroundColor Cyan
dotnet clean
dotnet restore

Write-Host "`n✅ ¡REFACTORIZACIÓN COMPLETADA!" -ForegroundColor Green
Write-Host "📌 Nuevo nombre de solución: $($slnFile.Name)" -ForegroundColor Yellow
Write-Host "📌 Proyectos renombrados a: $newName.*" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pasos siguientes:" -ForegroundColor Cyan
Write-Host "  1. Abre el .sln en Rider" -ForegroundColor White
Write-Host "  2. Verifica que todos los proyectos carguen correctamente" -ForegroundColor White
Write-Host "  3. Ejecuta: dotnet build" -ForegroundColor White