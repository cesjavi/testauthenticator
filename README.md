# Mockup ABM de Items con login mediante Google Authenticator

Este proyecto contiene un ejemplo mínimo construido con .NET que ofrece dos formas de interactuar con un ABM (Alta, Baja y Modificación) de items protegido por un inicio de sesión que requiere un código TOTP compatible con Google Authenticator:

* Una API minimalista basada en ASP.NET Core.
* Una aplicación de escritorio WinForms que consume directamente los servicios en memoria sin pasar por la API.

## Requisitos

* .NET 8 SDK
* Para ejecutar la interfaz gráfica es necesario Windows (el proyecto se dirige a `net8.0-windows`).

## Estructura

* `src/ItemManager.Core`: biblioteca con los modelos y servicios compartidos (usuarios, autenticación, sesiones, items y TOTP).
* `src/ItemManager`: API minimalista con endpoints para autenticación y gestión de items.
* `src/ItemManager.Gui`: aplicación WinForms que permite iniciar sesión y administrar los items directamente.
* `src/ItemManager/Filters/SessionValidationFilter.cs`: filtro que protege el grupo de endpoints `/items` en la API.

## Ejecutar la API

```bash
dotnet run --project src/ItemManager/ItemManager.csproj
```

La API quedará disponible en `http://localhost:5000` por defecto.

## Ejecutar la aplicación WinForms

```bash
dotnet run --project src/ItemManager.Gui/ItemManager.Gui.csproj
```

Al iniciar se solicitarán usuario, contraseña y el código TOTP vigente. El enlace "Ver secretos TOTP" muestra los secretos precargados y las URI `otpauth://` listas para registrar en Google Authenticator.

## Usuarios de ejemplo

Se incluyen usuarios precargados para facilitar las pruebas. Puedes consultarlos desde la interfaz gráfica (enlace "Ver secretos TOTP") o con el endpoint de la API:

```bash
curl http://localhost:5000/auth/users
```

Cada entrada incluye `SecretKey` y `QrUri`. Copia la URI en un generador de QR o agrégala manualmente en Google Authenticator para generar códigos válidos.

## Flujo de autenticación en la API

1. Registra en Google Authenticator la clave secreta del usuario (por ejemplo `JBSWY3DPEHPK3PXP`).
2. Obtén un código TOTP de 6 dígitos.
3. Envía la solicitud de login:

```bash
curl -X POST http://localhost:5000/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123","otpCode":"123456"}'
```

4. Si las credenciales y el código son correctos, recibirás un token de sesión.
5. Usa el token en los headers `X-Session-Token` para interactuar con `/items`.

## Endpoints del ABM

* `GET /items`: lista todos los items.
* `GET /items/{id}`: obtiene un item por ID.
* `POST /items`: crea un nuevo item. Requiere JSON con `name`, `description` y `quantity`.
* `PUT /items/{id}`: modifica un item existente.
* `DELETE /items/{id}`: elimina un item por ID.

Todos los endpoints `/items` requieren autenticación previa.

## Notas

Este es un mockup con almacenamiento en memoria. Para un sistema real se recomienda persistencia en base de datos, hashing de contraseñas y gestión de usuarios y sesiones más robusta.
