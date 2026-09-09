# AutoGestionSA

Solucion .NET 8 para AutoGestion S.A. Incluye APIs REST de Productos, Libros, Vehiculos y Personas, un API Gateway Ocelot, Redis, Identity/JWT, pruebas xUnit y Docker Compose.

## Requisitos

- .NET SDK 8.0
- Docker Desktop (para SQL Server, Redis y ejecucion integrada)

## Compilar y ejecutar pruebas

```powershell
dotnet restore AutoGestionSA.sln
dotnet build AutoGestionSA.sln
dotnet test tests/Personas.Tests/Personas.Tests.csproj
```

Las cinco pruebas usan EF Core InMemory y no requieren SQL Server:

- DUI invalido devuelve 400.
- Nombre vacio devuelve 400.
- Persona inexistente devuelve 404.
- Creacion valida persiste la persona.
- Actualizacion valida devuelve 200.

## Ejecutar toda la solucion con Docker

```powershell
docker compose up --build
```

Servicios disponibles:

| Servicio | URL |
| --- | --- |
| Gateway | http://localhost:5000 |
| Productos | http://localhost:5001 |
| Libros | http://localhost:5002 |
| Vehiculos | http://localhost:5003 |
| Personas | http://localhost:5004 |
| Redis | localhost:6379 |
| SQL Server | localhost:1433 |

SQL Server usa la cuenta `sa` y la clave `YourStrong!Passw0rd`. Dentro de Docker, las APIs usan los nombres de servicio `sqlserver` y `redis`, nunca `localhost`.

Para detener los servicios:

```powershell
docker compose down
```

## Capturas de Productos y Redis

Crear un producto:

```powershell
$producto = @{ nombre = "Teclado"; precio = 25.50; stock = 12 } | ConvertTo-Json
Invoke-WebRequest -Method POST -Uri http://localhost:5001/api/productos -ContentType "application/json" -Body $producto
```

Consultar dos veces la lista:

```powershell
curl.exe -i http://localhost:5001/api/productos
curl.exe -i http://localhost:5001/api/productos
```

La primera respuesta muestra `X-Cache: MISS` y la segunda `X-Cache: HIT`. La entrada `productos:lista` expira en cinco minutos y se elimina al crear, actualizar o eliminar un producto.

## Capturas de Gateway y Rate Limit

El Gateway traduce los prefijos externos a los controladores internos:

| Gateway | API interna |
| --- | --- |
| `/productos` | `Productos.Api/api/productos` |
| `/libros` | `Libros.Api/api/libros` |
| `/vehiculos` | `Vehiculos.Api/api/vehiculos` |
| `/personas/{id}` | `Personas.Api/api/personas/{id}` |

Ejecutar 11 solicitudes en menos de un minuto al mismo endpoint:

```powershell
1..11 | ForEach-Object { curl.exe -i http://localhost:5000/productos }
```

Ocelot aplica el limite global de 10 solicitudes por minuto y la solicitud excedente devuelve `429 Too Many Requests`.

## Capturas de Identity y JWT

Registrar un usuario:

```powershell
$registro = @{ email = "alumno@autogestion.com"; password = "Clave123!" } | ConvertTo-Json
Invoke-WebRequest -Method POST -Uri http://localhost:5003/api/auth/register -ContentType "application/json" -Body $registro
```

Iniciar sesion y copiar el valor de `accessToken`:

```powershell
Invoke-WebRequest -Method POST -Uri http://localhost:5003/api/auth/login -ContentType "application/json" -Body $registro
```

Sin token, Vehiculos responde 401:

```powershell
curl.exe -i http://localhost:5003/api/vehiculos
```

Con el token obtenido:

```powershell
curl.exe -i http://localhost:5003/api/vehiculos -H "Authorization: Bearer PEGAR_TOKEN_AQUI"
```

## Personas

Crear una persona valida:

```powershell
$persona = @{ nombre = "Ana Lopez"; dui = "12345678-9" } | ConvertTo-Json
Invoke-WebRequest -Method POST -Uri http://localhost:5004/api/personas -ContentType "application/json" -Body $persona
```

Para probar la validacion, enviar `dui: "123"` o `nombre: ""`; ambos casos retornan 400 con el ModelState.
