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

Al iniciar se solicitarán usuario, contraseña y el código TOTP vigente. El enlace "Ver secretos TOTP" muestra los secretos precargados y las URI `otpauth://` listas para registrar en Google Authenticator. Además, el botón "Registrar en Authenticator" abre un asistente que genera un código QR listo para escanear y, si fuera necesario, permite copiar el secreto y la URI para enrolar nuevas cuentas de prueba.

## Registrar una cuenta en Google Authenticator desde la GUI

1. Desde la pantalla de inicio de sesión haz clic en **Registrar en Authenticator**.
2. En la ventana que se abre puedes seleccionar un usuario existente para ver su secreto, la URI `otpauth://` y el código QR correspondiente.
3. Para crear un nuevo usuario, completa los campos **Usuario**, **Nombre a mostrar** y **Contraseña**, y pulsa **Registrar**. Se generará un secreto TOTP único y el código QR se actualizará automáticamente.
4. Escanea el código QR con la aplicación Google Authenticator de tu dispositivo. Si no puedes escanearlo, utiliza los botones **Copiar** para obtener el secreto o la URI y agrégalos manualmente en la app.
5. Vuelve a la pantalla de inicio de sesión e ingresa el código TOTP que genera Google Authenticator para el usuario registrado.

## Configurar Google Authenticator

1. Abre la aplicación Google Authenticator en tu dispositivo y elige **Agregar cuenta**.
2. Selecciona la opción **Escanear código QR** y apunta al QR generado en la ventana de registro de la GUI. Si prefieres hacerlo manualmente, elige **Introducir clave de configuración** y usa el secreto o la URI `otpauth://` que muestra la misma pantalla.
3. Usa como **Nombre de cuenta** el issuer configurado en el proyecto, por defecto `ItemManager`. Puedes cambiarlo editando la constante `issuer` tanto en `src/ItemManager/Program.cs` como en `src/ItemManager.Gui/LoginForm.cs`.
4. Los secretos precargados se definen en `src/ItemManager.Core/Services/UserStore.cs`; puedes reemplazarlos por los de tu organización o agregar nuevos usuarios desde la propia interfaz gráfica.
5. Guarda la cuenta y obtén el código TOTP de 6 dígitos generado por Google Authenticator para iniciar sesión en la API o en la aplicación WinForms.

## Usuarios de ejemplo

Se incluyen usuarios precargados para facilitar las pruebas. Puedes consultarlos desde la interfaz gráfica (enlace "Ver secretos TOTP") o con el endpoint de la API:

```bash
curl http://localhost:5000/auth/users
```

Cada entrada incluye `SecretKey` y `QrUri`. Desde la aplicación de escritorio puedes escanear directamente el QR que genera la ventana "Registrar en Authenticator" o, si lo prefieres, copiar la URI/secreto para registrarlos manualmente en Google Authenticator.

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
