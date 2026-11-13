# Sistema de Registro de Pacientes (C# + SQL Server)

Aplicación de consola para practicar Programación Orientada a Objetos, operaciones CRUD y conexión a SQL Server.

## Funciones

1. Registrar paciente.
2. Mostrar todos los pacientes.
3. Buscar por correo electrónico.
4. Actualizar los datos de un paciente.
5. Eliminar un registro con confirmación.
0. Salir.

La tabla se llama `Contacts`, como una agenda de contactos, y almacena los datos de registro y contacto del paciente.

## Requisitos

- Visual Studio 2026 (o una versión compatible con .NET 10).
- .NET 10 SDK.
- SQL Server instalado y en ejecución.
- SQL Server Management Studio (SSMS).

## 1. Crear la base de datos

1. Abre SQL Server Management Studio.
2. Conéctate a tu instancia de SQL Server.
3. Abre el archivo `Database/ClinicaDB.sql`.
4. Ejecuta el script con el botón **Execute**.
5. Confirma que se creó la base de datos `ClinicaDB` y la tabla `dbo.Contacts`.

## 2. Configurar la conexión

Abre `Data/DatabaseConnection.cs`.

Para una instancia local predeterminada:

```csharp
"Server=localhost;Database=ClinicaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

Para SQL Server Express, normalmente:

```csharp
"Server=localhost\\SQLEXPRESS;Database=ClinicaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

Si tu instancia tiene otro nombre, reemplaza el valor de `Server` por el nombre que aparece al conectarte desde SSMS. El proyecto usa autenticación de Windows. Si utilizas autenticación de SQL Server, configura el usuario y la contraseña localmente y no publiques credenciales en GitHub.

## 3. Abrir y ejecutar en Visual Studio

1. Descomprime el proyecto.
2. Abre `SistemaRegistroPacientes.csproj` desde Visual Studio.
3. Espera a que se restauren los paquetes NuGet.
4. Revisa el nombre de servidor en `DatabaseConnection.cs`.
5. Ejecuta con **Ctrl + F5** o **F5**.

El paquete `Microsoft.Data.SqlClient` se descarga desde NuGet al restaurar el proyecto.

## 4. Seguridad contra SQL Injection

No se concatenan datos del usuario dentro de las consultas SQL. Por ejemplo, la búsqueda usa:

```csharp
const string sql = "SELECT ... FROM Contacts WHERE Email = @Email;";
command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
```

Los parámetros hacen que el correo se trate como un valor y no como parte del código SQL. Evita construir consultas como:

```csharp
// NO HACER ESTO:
string sql = $"SELECT * FROM Contacts WHERE Email = '{email}'";
```

## 5. Conceptos de programación incluidos

- **POO:** `Patient` representa el modelo; `PatientRepository` concentra el acceso a datos; `DatabaseConnection` administra la cadena de conexión.
- **Base de datos:** SQL Server almacena los registros en `Contacts`.
- **CRUD:** crear, consultar, actualizar y eliminar.
- **Estructuras de control:** `while`, `switch`, `if` y ciclos de validación.
- **Manejo de errores:** `try/catch`, validación de entradas y tratamiento de correo duplicado.
- **Conexión SQL:** `SqlConnection`, `SqlCommand` y `SqlDataReader`.

## 6. Crear un repositorio en GitHub

1. Entra en https://github.com e inicia sesión.
2. Pulsa **New** (o el signo `+` y luego **New repository**).
3. Escribe como nombre: `SistemaRegistroPacientes`.
4. Selecciona **Public** si el profesor necesita abrirlo sin permisos, o **Private** si debe ser privado.
5. No marques la opción de agregar README si ya vas a subir este proyecto con su README.
6. Pulsa **Create repository**.
7. En Visual Studio, abre el proyecto y busca **Git > Create Git Repository**.
8. Selecciona GitHub, inicia sesión si lo solicita y elige el repositorio que creaste.
9. Confirma la creación y usa **Push** para subir los archivos.

### Alternativa usando la terminal

Abre una terminal en la carpeta del proyecto y ejecuta:

```bash
git init
git add .
git commit -m "Primer avance del sistema de registro de pacientes"
git branch -M main
git remote add origin https://github.com/TU-USUARIO/SistemaRegistroPacientes.git
git push -u origin main
```

Reemplaza `TU-USUARIO` por tu nombre de usuario de GitHub. Si Visual Studio ya creó y vinculó el repositorio, no repitas `git init` ni `git remote add origin`.

## 7. Compartir el enlace

1. Abre el repositorio en GitHub.
2. Copia la dirección que aparece en la barra del navegador, por ejemplo:
   `https://github.com/TU-USUARIO/SistemaRegistroPacientes`
3. Pega ese enlace en la plataforma de la clase o envíalo al profesor.
4. Si el repositorio es privado, agrega al profesor como colaborador; de lo contrario no podrá verlo.

## Nota importante

Este proyecto es educativo. No debe utilizarse para almacenar datos reales de pacientes: para un sistema clínico real se requieren controles adicionales de privacidad, permisos, auditoría, cifrado, copias de seguridad y cumplimiento legal. Usa datos ficticios para las pruebas.
