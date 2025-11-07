# Mockup ABM de Items con login mediante Google Authenticator

Este proyecto contiene un ejemplo mínimo construido con .NET que ofrece dos formas de interactuar con un ABM (Alta, Baja y Modificación) de items protegido por un inicio de sesión que requiere un código TOTP compatible con Google Authenticator:

* Una API minimalista basada en ASP.NET Core.
* Una aplicación de escritorio WinForms que consume directamente los servicios en memoria sin pasar por la API.

## Requisitos

* .NET 8 SDK
* Para ejecutar la interfaz gráfica es necesario Windows (el proyecto se dirige a `net8.0-windows`).

## Estructura

### `src/ItemManager.Core`

Biblioteca de clases compartida que modela el dominio del ABM. Incluye entidades (`Item`, `User`), repositorios y servicios de
negocio (`ItemRepository`, `AuthService`, `SessionService`, `UserStore`, `TotpService`) y contratos comunes reutilizados por el
resto de los proyectos. Desde este ensamblado se centraliza la lógica de autenticación basada en TOTP, la creación de sesiones
en memoria y las validaciones de negocio para altas, bajas y modificaciones de items.

### `src/ItemManager`

Proyecto ASP.NET Core Minimal API que expone los endpoints REST para autenticación y gestión de items. Configura la inyección de
dependencias de los servicios de `ItemManager.Core`, define rutas como `/auth/login`, `/auth/users` y el grupo protegido `/items`,
además de incluir utilidades como `Filters/SessionValidationFilter.cs` para aplicar la validación del token de sesión en todas
las operaciones CRUD.

### `src/ItemManager.ApiClient`

Biblioteca de cliente HTTP fuertemente tipado pensada para reutilizarse en cualquier aplicación .NET que necesite consumir la
API. Provee clases como `ItemManagerApiClient` y contratos de datos que encapsulan las llamadas `GET`, `POST`, `PUT` y `DELETE`
contra los endpoints del proyecto `ItemManager`, manejando serialización JSON y encabezados de autenticación.

### `src/ItemManager.Client`

Aplicación de consola multiplataforma que utiliza la biblioteca `ItemManager.ApiClient` para interactuar con la API. Implementa
un menú interactivo (`CliApplication`) que guía al usuario por el flujo de autenticación, listado de items y operaciones CRUD,
ideal para pruebas rápidas desde la terminal sin necesidad de interfaz gráfica.

### `src/ItemManager.Gui`

Aplicación WinForms dirigida a `net8.0-windows` que consume directamente los servicios en memoria de `ItemManager.Core`. Ofrece
formularios de inicio de sesión con validación TOTP, administración visual de items y herramientas para registrar usuarios en
Google Authenticator (visualización de secretos, generación de QR y alta de cuentas de prueba).

### `src/ItemManager.WebApp`

Aplicación Razor Pages que actúa como frontend web sobre la API. Administra el ciclo de autenticación almacenando el token de
sesión en `ISession`, muestra el listado de items consumiendo el endpoint protegido y expone formularios para crear o eliminar
registros, reutilizando los contratos definidos en `ItemManager.ApiClient`.

## Implementación de autenticación con TOTP

El proyecto utiliza claves compartidas en formato Base32 y el algoritmo TOTP (RFC 6238) para generar y validar códigos de seis
dígitos compatibles con Google Authenticator. Las claves se almacenan en el archivo `src/ItemManager/App_Data/users.json`, que
se carga en memoria al iniciar la aplicación. Cada entrada contiene usuario, contraseña en texto plano para las pruebas de
concepto y el secreto TOTP. Cuando no existe el archivo, se crea automáticamente con usuarios de ejemplo.

La clase central es `TotpService` (en `src/ItemManager.Core/Services/TotpService.cs`), que expone métodos para:

* Generar claves secretas nuevas (`GenerateSecretKey`).
* Validar códigos proporcionados por los clientes (`ValidateCode`) usando ventanas de tolerancia de 30 segundos.
* Construir URIs `otpauth://` listas para registrar en aplicaciones de autenticación (`BuildOtpAuthUri`).

En ambos frontales (API y GUI) se reutilizan estos servicios a través de la capa Core, por lo que el comportamiento es
consistente entre las dos experiencias.

## Ejemplos de uso

### API ASP.NET Core

En la API se expone un endpoint de login que utiliza `TotpService` y `UserStore` para autenticar usuarios. A continuación se
muestra un extracto simplificado del flujo (ubicado en `src/ItemManager/Program.cs`):

```csharp
app.MapPost("/auth/login", (LoginRequest request, TotpService totp, UserStore users) =>
{
    var user = users.FindByUsername(request.Username);
    if (user is null || user.Password != request.Password)
    {
        return Results.Unauthorized();
    }

    if (!totp.ValidateCode(user, request.OtpCode))
    {
        return Results.Unauthorized();
    }

    var session = users.CreateSession(user);
    return Results.Ok(new { session.Token, session.ExpiresAt });
});
```

Una vez obtenido el token, los consumidores pueden invocar los endpoints de `/items` incluyendo el encabezado `X-Session-Token`.
El archivo `src/ItemManager/Filters/SessionValidationFilter.cs` contiene la validación reutilizable para proteger estos recursos.

#### Formulario de login con Razor Pages

Si deseas construir una interfaz web con ASP.NET Core que consuma directamente `AuthService`, puedes reutilizar la misma lógica
del formulario WinForms dentro de una página Razor. El siguiente ejemplo guarda el token de sesión en `ISession` para posteriores
llamadas a la API:

```csharp
public class LoginModel : PageModel
{
    private readonly AuthService _authService;

    public LoginModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string OtpCode { get; set; } = string.Empty;

    public IActionResult OnPost()
    {
        var login = new LoginRequest(Username, Password, OtpCode);
        var result = _authService.TryLogin(login);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, "Credenciales o código TOTP inválido");
            return Page();
        }

        HttpContext.Session.SetString("X-Session-Token", result.SessionToken!);
        return RedirectToPage("/Items/Index");
    }
}
```

El formulario `.cshtml` correspondiente pediría los tres campos (`Username`, `Password`, `OtpCode`) y mostraría los errores
agregados a `ModelState`. Recuerda importar `ItemManager.Services` para disponer de `AuthService` en la página. Una vez
autenticado, la aplicación puede reutilizar el token almacenado en sesión para invocar `GET` o `POST` sobre `/items`.

### Aplicación WinForms

La interfaz gráfica (`src/ItemManager.Gui`) también consume `TotpService` y `UserStore`. El formulario de inicio de sesión
(`LoginForm.cs`) solicita las credenciales y el código TOTP vigente. Si la validación tiene éxito se abre la ventana principal
con el listado de items. El siguiente fragmento ilustra la lógica principal del botón de ingreso:

```csharp
private void OnLoginClick(object sender, EventArgs e)
{
    var user = _userStore.FindByUsername(txtUsername.Text);
    if (user is null || user.Password != txtPassword.Text)
    {
        MessageBox.Show("Usuario o contraseña inválidos");
        return;
    }

    if (!_totpService.ValidateCode(user, txtTotp.Text))
    {
        MessageBox.Show("Código TOTP inválido");
        return;
    }

    var session = _userStore.CreateSession(user);
    new MainForm(_userStore, session).Show();
    Hide();
}
```

Desde el menú de la GUI también es posible registrar nuevos usuarios de prueba y generar el código QR mediante `BuildOtpAuthUri`
para enrolarlos rápidamente en Google Authenticator.

## Ejecutar la API

```bash
dotnet run --project src/ItemManager/ItemManager.csproj
```

La API quedará disponible en `http://localhost:5000` por defecto.

## Cliente de consola para consumir la API

El proyecto `src/ItemManager.Client` ofrece un cliente de referencia que consume los endpoints expuestos por la API minimalista.
Funciona en cualquier sistema operativo con .NET 8 instalado y facilita probar el flujo completo sin necesidad de construir una
interfaz gráfica.

```bash
dotnet run --project src/ItemManager.Client/ItemManager.Client.csproj -- --url=http://localhost:5000
```

Si omites el parámetro `--url`, el cliente intentará conectarse a `http://localhost:5000` o al valor definido en la variable de
entorno `ITEM_MANAGER_BASE_URL`.

Al iniciarse mostrará un menú interactivo con las siguientes acciones:

1. **Iniciar sesión**: solicita usuario, contraseña y código TOTP vigente. Al autenticarse correctamente almacena el token de
   sesión.
2. **Listar items**: realiza `GET /items` usando el encabezado `X-Session-Token`.
3. **Crear item**: envía `POST /items` con el cuerpo JSON correspondiente.
4. **Actualizar item**: utiliza `PUT /items/{id}` para modificar nombre, descripción y cantidad.
5. **Eliminar item**: invoca `DELETE /items/{id}`.
6. **Cerrar sesión**: descarta el token para forzar un nuevo login en la siguiente operación protegida.

Los errores de autenticación muestran mensajes claros y, en caso de que el token caduque, el cliente solicitará volver a iniciar
sesión antes de continuar.

## Ejecutar la aplicación web ASP.NET Core

```bash
dotnet run --project src/ItemManager.WebApp/ItemManager.WebApp.csproj
```

La aplicación Razor Pages arranca en `http://localhost:5130` (o el puerto asignado por Kestrel) y consume la API configurada en
`appsettings.json` bajo la clave `ItemManagerApi:BaseUrl`. Puedes modificar ese valor o exportar la variable de entorno
`ASPNETCORE_URLS` para que ambos servicios convivan en puertos distintos.

La interfaz web replica el flujo de autenticación de Mercado Pago que ya se implementó en la app WinForms:

1. **Inicio de sesión** (`/Index`): solicita usuario, contraseña y código TOTP. Si la autenticación es exitosa, almacena el token de
   sesión en `ISession`.
2. **Listado de items** (`/Items`): muestra la tabla obtenida de `GET /items` y permite refrescar el contenido manualmente.
3. **Alta y baja**: desde la misma página se pueden crear items (enviando `POST /items`) o eliminarlos (`DELETE /items/{id}`) con
   confirmación en el navegador.
4. **Cerrar sesión**: desde el encabezado se limpia la sesión y se redirige al formulario de login.

Si la API responde con un `401`, la aplicación invalida la sesión y redirige nuevamente al inicio para solicitar credenciales.

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
