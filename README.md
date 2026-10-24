# Sistema de gestión de usuarios

Aplicación web con una API REST en ASP.NET Web API, una interfaz ASP.NET Web Forms y una base de datos MySQL. Permite iniciar sesión, consultar usuarios y administrar sus perfiles y roles.

## Componentes

- `apiRestCtrlUsuariosFull/`: API REST.
- `wsCtrlUsuarioFull/`: interfaz web y consumo de la API.
- `database/control_acceso_preparar_seguro.sql`: prepara una base local de pruebas llamada `control_acceso_pruebas`; crea lo necesario sin eliminar usuarios existentes.
- `database/migracion_password_hash.sql`: migración para una base existente. Conserva los usuarios, pero modifica el almacenamiento de contraseñas y reemplaza procedimientos; revisa el script antes de ejecutarlo.
- `postman/gestion-usuarios.postman_collection.json`: solicitudes de ejemplo sin credenciales predeterminadas.

## Requisitos

- Visual Studio con soporte para ASP.NET y .NET Framework 4.7.2.
- MySQL 8 o compatible con la intercalación usada en el SQL.
- Paquetes NuGet del proyecto, restaurables desde `packages.config`.

## Configuración local

1. Copia los `Web.config.example` como `Web.config` en cada proyecto.
2. En el `Web.config` de la API, configura la conexión local a MySQL. Sustituye `CHANGE_ME_LOCAL_PASSWORD` por una contraseña local y ajusta `ApiUsuarioBaseUrl` en la interfaz para que coincida con el puerto local de la API.
3. Abre ambos `.csproj` en Visual Studio y restaura los paquetes NuGet.
4. En Postman, configura `baseUrl` y proporciona datos ficticios para las solicitudes.

Los `Web.config` locales, el entorno local de Postman, las fotografías de usuarios y los archivos generados están excluidos de Git. El archivo `database/control_acceso.sql` también se excluye: es un volcado local con registros de usuarios y datos de autenticación. No se necesita para preparar las pruebas; usa `control_acceso_preparar_seguro.sql` en una base desechable.

## Seguridad

Las contraseñas nuevas se almacenan con PBKDF2-HMAC-SHA256, salt aleatorio e iteraciones configuradas en la API. Las contraseñas existentes se reemplazan por su hash después del primer acceso correcto. Las respuestas de consulta no incluyen contraseñas ni hashes.

El proyecto es una aplicación académica/local y requiere revisiones adicionales antes de usarlo en producción. Para pruebas, utiliza datos ficticios.
