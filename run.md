## Instrucciones de Instalación y Ejecución

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado en tu máquina.
- (Opcional) Un editor como Visual Studio 2022 o Visual Studio Code.

### Pasos para ejecutar la API

1. **Descomprimir el archivo ZIP del proyecto**

2. **Con una terminal acceder a la carpeta principal WebApiTest (dentro de la misma se encuentra WebApiTest.sln) y ejecutar los siguientes comandos:**
										
	dotnet restore (restaura las dependencias)
    dotnet build   (compila el proyecto)
    dotnet run (ejecuta el proyecto)


3. **Accede a la documentación Swagger:**

   Una vez iniciada la API, abre tu navegador y navega a: http://localhost:5289/index.html

---

**Nota:**  

Puedes encontrar ejemplos de requests en el archivo `WebApiTest.http` incluido en el proyecto. Asegúrate de que la API esté en ejecución para poder probarlos correctamente.

---

### Extra: Correr la suit de tests

**En la carpeta principal WebApiTest, ejecutar el siguiente comando:**
										
	dotnet test (ejecuta todos los proyecto de test)

---