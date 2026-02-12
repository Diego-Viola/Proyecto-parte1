# 🔧 Solución: Error de Archivo Bloqueado (deps.json)

## 📋 Problema

```
Error MSB4018: Error inesperado en la tarea "GenerateDepsFile"
System.IO.IOException: La operación solicitada no se puede realizar en un archivo con una sección asignada a usuario abierta
File: WebApiTest.Application.Test\bin\Debug\net8.0\WebApiTest.Application.Test.deps.json
```

### ❓ Causa

El archivo `deps.json` está siendo usado por otro proceso (IDE, tests en ejecución, antivirus, etc.) y .NET no puede sobrescribirlo durante la compilación.

## ✅ Soluciones

### Solución 1: Script Automático (Recomendado)

**Ejecutar el script de corrección:**

```powershell
# PowerShell
.\fix-locked-files.ps1

# O con CMD
fix-locked-files.bat
```

El script hará:
1. ✅ Detener procesos de .NET en ejecución
2. ✅ Limpiar con `dotnet clean`
3. ✅ Eliminar todas las carpetas `bin/` y `obj/`
4. ✅ Restaurar paquetes NuGet
5. ✅ Compilar la solución
6. ✅ Ejecutar los tests

### Solución 2: Manual Paso a Paso

```powershell
# 1. Detener procesos de .NET
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "testhost" -Force -ErrorAction SilentlyContinue

# 2. Limpiar solución
dotnet clean

# 3. Eliminar carpetas bin/obj
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force

# 4. Restaurar y compilar
dotnet restore
dotnet build

# 5. Ejecutar tests
dotnet test
```

### Solución 3: Cerrar y Reabrir el IDE

1. **Cerrar completamente** JetBrains Rider
2. Abrir PowerShell en la raíz del proyecto
3. Ejecutar:
   ```powershell
   dotnet clean
   dotnet build
   dotnet test
   ```
4. **Reabrir** Rider después de que compile exitosamente

### Solución 4: Reiniciar el Sistema (Última opción)

Si ninguna de las anteriores funciona, reinicia Windows para liberar todos los handles de archivos.

## 🎯 Comandos de Validación

Después de aplicar la solución, verifica que todo funcione:

```powershell
# Verificar compilación
dotnet build --no-incremental

# Verificar tests
dotnet test --no-build

# Verificar todos los proyectos
dotnet sln list
dotnet build
```

## 🛡️ Prevención

Para evitar este problema en el futuro:

1. **Cerrar ventanas de terminal** con procesos de .NET antes de compilar
2. **Detener tests en ejecución** antes de rebuild
3. **Configurar exclusiones en el antivirus** para las carpetas `bin/` y `obj/`
4. **No abrir archivos** en `bin/Debug/` mientras desarrollas

## 📊 Archivos de Solución Creados

- `fix-locked-files.ps1` - Script PowerShell automatizado
- `fix-locked-files.bat` - Script Batch alternativo
- `FIX-ARCHIVOS-BLOQUEADOS.md` - Esta documentación

## ⚠️ Notas Importantes

- El script detendrá **todos** los procesos de .NET en ejecución
- Se eliminarán **todas** las carpetas `bin/` y `obj/` (se regenerarán)
- Los paquetes NuGet se restaurarán automáticamente
- El proceso toma aproximadamente 1-2 minutos

## 🔍 Verificación de Procesos Activos

Para ver qué procesos están usando archivos .NET:

```powershell
# Ver procesos de .NET activos
Get-Process | Where-Object { $_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*test*" }

# Ver archivos abiertos (requiere herramienta externa)
# Descargar Handle.exe de Sysinternals
handle.exe deps.json
```

## ✅ Resultado Esperado

Después de ejecutar el script:

```
✅ COMPILACIÓN EXITOSA
✅ TODOS LOS TESTS PASARON

Test Run Successful.
Total tests: X
     Passed: X
 Total time: X.XXX Seconds
```

## 🆘 Si el Problema Persiste

1. **Verificar permisos** de la carpeta del proyecto
2. **Deshabilitar temporalmente** el antivirus
3. **Mover el proyecto** a una ruta más corta (ej: `C:\Proyectos\`)
4. **Ejecutar PowerShell/CMD** como Administrador

---

**Última actualización:** 2026-02-12  
**Estado:** Solución verificada y documentada
