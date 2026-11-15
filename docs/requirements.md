# NexoDesk: Requisitos

## 1. Alcance

NexoDesk es un sistema web interno de soporte técnico (IT Service Desk). Permite que los empleados de una empresa registren incidencias de TI, que los técnicos las atiendan y las resuelvan, y que los administradores gestionen usuarios, categorías y asignaciones.

**Roles:** `Requester` (empleado), `Technician` (técnico) y `Admin` (administrador).

Este documento define qué debe hacer el sistema en su primera versión (MVP). Los detalles de permisos están en `roles-and-permissions.md` y el ciclo de vida de las incidencias en `ticket-workflow.md`.

## 2. Requisitos funcionales

### Autenticación y usuarios

- **RF-01:** Un usuario activo puede iniciar sesión con correo y contraseña y recibir un token de acceso (JWT).
- **RF-02:** Un usuario autenticado puede consultar sus propios datos (nombre, correo y rol).
- **RF-03:** No existe registro público. Solo el `Admin` crea cuentas de empleados y técnicos.
- **RF-04:** El `Admin` puede crear usuarios y asignarles un rol.
- **RF-05:** El `Admin` puede consultar la lista de usuarios y filtrarla por rol, por ejemplo para listar técnicos al asignar.
- **RF-06:** El `Admin` puede activar y desactivar usuarios. Un usuario desactivado no puede iniciar sesión.

### Incidencias (tickets)

- **RF-07:** El `Requester` puede crear una incidencia con título, descripción, categoría y prioridad.
- **RF-08:** Cada incidencia recibe un número legible único (por ejemplo `INC-000125`), además de su identificador interno.
- **RF-09:** El `Requester` puede consultar la lista y el detalle de sus propias incidencias.
- **RF-10:** El `Technician` puede consultar las incidencias asignadas a él y las disponibles (sin técnico asignado).
- **RF-11:** El `Admin` puede consultar todas las incidencias.
- **RF-12:** El `Admin` puede asignar y reasignar incidencias a un técnico activo.
- **RF-13:** El `Technician` puede autoasignarse una incidencia disponible.
- **RF-14:** El `Technician` y el `Admin` pueden cambiar la prioridad de una incidencia.
- **RF-15:** El `Technician` puede cambiar el estado de las incidencias que tiene asignadas, respetando las transiciones permitidas.
- **RF-16:** El `Technician` puede registrar la solución (`ResolutionSummary`) y marcar la incidencia como resuelta.
- **RF-17:** El `Requester` puede cerrar una incidencia propia que esté resuelta.
- **RF-18:** El `Requester` puede reabrir una incidencia propia que esté resuelta.
- **RF-19:** El `Admin` puede cerrar y reabrir cualquier incidencia que esté resuelta.
- **RF-20:** La lista de incidencias admite búsqueda por número o texto, filtros (estado, prioridad, categoría), ordenamiento y paginación.

### Categorías

- **RF-21:** El `Admin` puede crear, editar y consultar categorías.
- **RF-22:** El `Admin` puede activar y desactivar categorías. No se eliminan categorías que tengan incidencias.
- **RF-23:** Todos los usuarios autenticados pueden consultar las categorías activas para crear incidencias.

### Comentarios

- **RF-24:** El `Requester` dueño, el `Technician` asignado y el `Admin` pueden agregar comentarios a una incidencia.
- **RF-25:** Quien tenga acceso a una incidencia puede ver sus comentarios, con autor y fecha.

### Historial

- **RF-26:** Cada cambio importante de una incidencia genera un registro de historial con el autor, el tipo de evento, el valor anterior, el valor nuevo y la fecha. Eventos: creación, asignación, reasignación, cambio de estado, cambio de prioridad y registro de solución.
- **RF-27:** El `Admin` puede consultar el historial completo de cualquier incidencia. El `Requester` y el `Technician` pueden consultar el historial de las incidencias a las que tienen acceso.

### Dashboard

- **RF-28:** El sistema muestra un resumen estadístico que depende del rol:
  - `Requester`: totales de sus propias incidencias por estado.
  - `Technician`: sus incidencias asignadas por estado y las disponibles.
  - `Admin`: totales generales, incidencias por estado, por prioridad y por técnico.

## 3. Reglas de negocio

- **RN-01:** El título y la descripción de una incidencia son obligatorios. El título tiene un máximo de 120 caracteres.
- **RN-02:** Una incidencia solo puede crearse con una categoría activa.
- **RN-03:** La prioridad (`Low`, `Medium`, `High`, `Critical`) tiene valor predeterminado `Medium`. La elige el `Requester` al crear la incidencia.
- **RN-04:** Toda incidencia nueva comienza en estado `Open`.
- **RN-05:** El estado solo puede cambiar mediante las transiciones definidas en `ticket-workflow.md`. Cualquier otro cambio es rechazado.
- **RN-06:** No se puede resolver una incidencia sin registrar `ResolutionSummary`.
- **RN-07:** Una incidencia solo puede cerrarse desde el estado `Resolved`.
- **RN-08:** Una incidencia `Closed` no admite modificaciones ni comentarios.
- **RN-09:** El `Requester` no cambia estados libremente. Solo puede ejecutar dos transiciones sobre sus propias incidencias: cerrar y reabrir una incidencia `Resolved`.
- **RN-10:** La salida de `WaitingForUser` la realiza siempre el técnico. El comentario del empleado no cambia el estado automáticamente.
- **RN-11:** Cuando una incidencia `Open` se asigna a un técnico (por el `Admin` o por autoasignación), pasa a `Assigned` y deja de aparecer como disponible.
- **RN-12:** Reasignar una incidencia activa (`Assigned`, `InProgress` o `WaitingForUser`) a otro técnico la devuelve al estado `Assigned`.
- **RN-13:** Al reabrir una incidencia (`Resolved` → `Reopened`) se quita el técnico asignado, se limpia `ResolvedAtUtc` y la incidencia vuelve a estar disponible. Debe pasar de nuevo por asignación (`Reopened` → `Assigned`), y puede tomarla cualquier técnico, incluido el anterior.
- **RN-14:** Solo se puede asignar una incidencia a un usuario con rol `Technician` y activo.
- **RN-15:** El nombre de una categoría es único. Una categoría con incidencias no se elimina; solo se desactiva.
- **RN-16:** Un usuario desactivado no puede iniciar sesión ni ser elegido como técnico asignado.
- **RN-17:** Todo cambio importante de una incidencia se registra en el historial en la misma operación que lo produce.

## 4. Requisitos no funcionales

### Seguridad

- **RNF-01:** Todas las funcionalidades privadas requieren autenticación. Una solicitud sin token válido recibe 401.
- **RNF-02:** La autorización se valida en el backend en cada operación. No depende de ocultar botones en el frontend.
- **RNF-03:** Una solicitud sin permisos suficientes recibe 403, y un usuario no puede ver ni modificar incidencias ajenas.
- **RNF-04:** Las contraseñas no se almacenan en texto plano (hash gestionado por ASP.NET Core Identity).
- **RNF-05:** Claves JWT, cadenas de conexión y otros secretos no se suben a GitHub. Se usan User Secrets en desarrollo y variables de entorno en despliegue.
- **RNF-06:** CORS se restringe al origen del frontend.

### Calidad de datos y API

- **RNF-07:** Todos los datos de entrada se validan, y los errores de validación devuelven 400 con un mensaje claro.
- **RNF-08:** La API devuelve códigos HTTP apropiados (200, 201, 400, 401, 403, 404, 409) y errores en formato Problem Details.
- **RNF-09:** La paginación, los filtros y el ordenamiento se ejecutan en la consulta a la base de datos, no en memoria.
- **RNF-10:** Si dos técnicos intentan tomar la misma incidencia a la vez, solo uno lo consigue y el otro recibe 409 (control de concurrencia).
- **RNF-11:** Las fechas se almacenan en UTC.

### Mantenibilidad

- **RNF-12:** Las reglas de negocio no residen en los controladores.
- **RNF-13:** Las reglas críticas (transiciones, permisos y resolución) están cubiertas por pruebas automatizadas.
- **RNF-14:** La API está documentada con Swagger/OpenAPI.

## 5. Fuera de alcance de la versión 1

- Registro público de usuarios.
- Renovación de tokens (refresh tokens) y revocación de sesiones.
- Cierre automático de incidencias resueltas sin respuesta del empleado.
- Notificaciones por correo o en tiempo real.
- Comentarios internos exclusivos para técnicos.
- Archivos adjuntos.
- SLA y control de tiempos de respuesta.
- Edición del título y la descripción de una incidencia después de crearla.
- Chat en tiempo real, aplicación móvil, microservicios, inteligencia artificial, clasificación automática, integraciones externas y multiempresa.

## 6. Criterios de aceptación

**CA-01: Resolver sin solución**

- _Dado_ un ticket en `InProgress` asignado a un técnico y sin solución registrada,
- _cuando_ el técnico intenta pasarlo a `Resolved`,
- _entonces_ la API responde 400, el estado sigue siendo `InProgress` y no se crea ningún evento de historial.

**CA-02: Acceso a incidencias ajenas**

- _Dado_ un `Requester` autenticado y un ticket creado por otro empleado,
- _cuando_ intenta consultar o modificar ese ticket,
- _entonces_ la API rechaza la solicitud y no revela el contenido del ticket.

**CA-03: Autoasignación simultánea**

- _Dado_ un ticket `Open` y dos técnicos que intentan autoasignárselo a la vez,
- _cuando_ se procesan ambas solicitudes,
- _entonces_ solo una tiene éxito, el ticket queda en `Assigned` con un único técnico y la otra recibe 409.

**CA-04: Reapertura**

- _Dado_ un ticket `Resolved` propio del `Requester`,
- _cuando_ el empleado lo reabre,
- _entonces_ el ticket pasa a `Reopened`, no tiene técnico asignado, `ResolvedAtUtc` queda vacío, aparece como disponible y se registra el evento en el historial.

**CA-05: Cierre solo desde Resolved**

- _Dado_ un ticket en cualquier estado distinto de `Resolved`,
- _cuando_ un `Requester` o un `Admin` intenta cerrarlo,
- _entonces_ la API responde 400 y el estado no cambia.

**CA-06: Comentarios en ticket cerrado**

- _Dado_ un ticket en estado `Closed`,
- _cuando_ cualquier usuario intenta agregar un comentario,
- _entonces_ la API rechaza la solicitud y no se crea el comentario.

**CA-07: Usuario desactivado**

- _Dado_ un usuario desactivado por el `Admin`,
- _cuando_ intenta iniciar sesión con credenciales correctas,
- _entonces_ la API rechaza el inicio de sesión y no emite token.

**CA-08: Categoría inactiva**

- _Dado_ una categoría desactivada,
- _cuando_ un `Requester` intenta crear una incidencia con esa categoría,
- _entonces_ la API responde 400 y no se crea la incidencia.
