# Mockup ABM de Items con login mediante Google Authenticator

Este proyecto es un ejemplo mínimo de una API construida con .NET que expone un ABM (Alta, Baja y Modificación) de items protegido por un inicio de sesión que requiere un código TOTP compatible con Google Authenticator.

## Requisitos

* .NET 8 SDK

## Estructura

* `src/ItemManager/Program.cs`: API minimalista con endpoints para autenticación y gestión de items.
* `src/ItemManager/Services`: Servicios para usuarios, autenticación, sesiones, items y generación/validación de códigos TOTP.
* `src/ItemManager/Filters/SessionValidationFilter.cs`: Filtro que protege el grupo de endpoints `/items`.

## Ejecutar la aplicación

```bash
dotnet run --project src/ItemManager/ItemManager.csproj
```

La API quedará disponible en `http://localhost:5000` por defecto.

## Usuarios de ejemplo

Se incluyen usuarios precargados para facilitar las pruebas. Puedes consultar sus secretos TOTP y la URL para generar un código QR con el endpoint:

```bash
curl http://localhost:5000/auth/users
```

Cada entrada incluye `SecretKey` y `QrUri`. Copia la URI en un generador de QR o agrégala manualmente en Google Authenticator para generar códigos válidos.

## Flujo de autenticación

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
