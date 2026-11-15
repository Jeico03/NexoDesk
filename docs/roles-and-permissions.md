# NexoDesk: Roles y permisos

## 1. Roles

| Rol          | Descripción                                                                        |
| ------------ | ---------------------------------------------------------------------------------- |
| `Requester`  | Empleado que reporta incidencias y hace seguimiento de las suyas.                  |
| `Technician` | Personal de TI que atiende incidencias.                                            |
| `Admin`      | Administra usuarios, categorías y asignaciones, y supervisa todas las incidencias. |

Cada usuario tiene exactamente un rol. Un usuario desactivado (`IsActive = false`) no puede iniciar sesión, sea cual sea su rol.

## 2. Conceptos de alcance

La columna de alcance de las matrices usa estos términos:

- **Propios:** incidencias donde `RequesterId` es el usuario autenticado.
- **Asignados:** incidencias donde `AssignedTechnicianId` es el usuario autenticado.
- **Disponibles:** incidencias sin técnico asignado (`AssignedTechnicianId` nulo) y en estado `Open` o `Reopened`.
- **Todos:** todas las incidencias del sistema.
- **No:** la acción no está permitida.

## 3. Matriz de permisos

### 3.1. Autenticación y usuarios

| Acción                                   | Requester | Technician | Admin |
| ---------------------------------------- | --------- | ---------- | ----- |
| Iniciar sesión                           | Sí        | Sí         | Sí    |
| Consultar sus propios datos (`/auth/me`) | Sí        | Sí         | Sí    |
| Crear usuarios                           | No        | No         | Sí    |
| Consultar y filtrar usuarios             | No        | No         | Sí    |
| Activar y desactivar usuarios            | No        | No         | Sí    |

### 3.2. Incidencias

| Acción                           | Requester                       | Technician              | Admin                           |
| -------------------------------- | ------------------------------- | ----------------------- | ------------------------------- |
| Crear incidencia                 | Sí                              | No                      | No                              |
| Ver lista de incidencias         | Propios                         | Asignados y disponibles | Todos                           |
| Ver detalle de una incidencia    | Propios                         | Asignados y disponibles | Todos                           |
| Ver historial de una incidencia  | Propios                         | Asignados y disponibles | Todos                           |
| Comentar                         | Propios                         | Asignados               | Todos                           |
| Ver comentarios                  | Propios                         | Asignados y disponibles | Todos                           |
| Cambiar prioridad                | No                              | Asignados               | Todos                           |
| Asignar o reasignar a un técnico | No                              | No                      | Todos                           |
| Autoasignarse una incidencia     | No                              | Disponibles             | No                              |
| Cambiar estado                   | Solo cerrar o reabrir (ver 3.3) | Asignados (ver 3.3)     | Solo cerrar o reabrir (ver 3.3) |
| Registrar solución               | No                              | Asignados               | No                              |

Notas:

- Ninguna operación de escritura (comentar, cambiar prioridad, asignar, cambiar estado) se permite sobre una incidencia `Closed`.
- Un técnico puede **ver** las incidencias disponibles para decidir si las toma, pero no puede comentar ni modificarlas hasta que se las asigne.
- Un técnico no ve las incidencias asignadas a otros técnicos.
- Solo el `Requester` crea incidencias. El `Admin` y el `Technician` no crean incidencias en nombre de otros en la v1.

### 3.3. Cambios de estado por rol

Esta tabla resume quién puede ejecutar cada transición. Las condiciones y efectos completos están en `ticket-workflow.md`.

| Transición                                                               | Requester | Technician                   | Admin        |
| ------------------------------------------------------------------------ | --------- | ---------------------------- | ------------ |
| `Open` → `Assigned`                                                      | No        | Autoasignación (disponibles) | Sí (asignar) |
| `Reopened` → `Assigned`                                                  | No        | Autoasignación (disponibles) | Sí (asignar) |
| `Assigned` / `InProgress` / `WaitingForUser` → `Assigned` (reasignación) | No        | No                           | Sí           |
| `Assigned` → `InProgress`                                                | No        | Asignados                    | No           |
| `InProgress` → `WaitingForUser`                                          | No        | Asignados                    | No           |
| `WaitingForUser` → `InProgress`                                          | No        | Asignados                    | No           |
| `InProgress` → `Resolved`                                                | No        | Asignados (con solución)     | No           |
| `Resolved` → `Closed`                                                    | Propios   | No                           | Todos        |
| `Resolved` → `Reopened`                                                  | Propios   | No                           | Todos        |

Cualquier transición que no aparezca en esta tabla está prohibida para todos los roles.

### 3.4. Categorías

| Acción                         | Requester | Technician | Admin |
| ------------------------------ | --------- | ---------- | ----- |
| Consultar categorías activas   | Sí        | Sí         | Sí    |
| Consultar categorías inactivas | No        | No         | Sí    |
| Crear categoría                | No        | No         | Sí    |
| Editar categoría               | No        | No         | Sí    |
| Activar y desactivar categoría | No        | No         | Sí    |

### 3.5. Dashboard

| Rol          | Qué ve                                                                                  |
| ------------ | --------------------------------------------------------------------------------------- |
| `Requester`  | Totales de sus incidencias propias por estado.                                          |
| `Technician` | Totales de sus incidencias asignadas por estado y el número de incidencias disponibles. |
| `Admin`      | Totales generales, incidencias por estado, por prioridad y por técnico.                 |

## 4. Autorización por endpoint

| Método | Endpoint                       | Acceso                | Alcance (validado en el servicio)                                         |
| ------ | ------------------------------ | --------------------- | ------------------------------------------------------------------------- |
| POST   | `/api/auth/login`              | Anónimo               | Usuario activo con credenciales válidas.                                  |
| GET    | `/api/auth/me`                 | Autenticado           | Usuario del token.                                                        |
| GET    | `/api/tickets`                 | Autenticado           | Filtra según el rol (ver 3.2).                                            |
| POST   | `/api/tickets`                 | `Requester`           | Categoría activa.                                                         |
| GET    | `/api/tickets/{id}`            | Autenticado           | Según rol y alcance.                                                      |
| PATCH  | `/api/tickets/{id}/assignment` | `Admin`, `Technician` | Admin: asigna o reasigna. Technician: solo autoasignación de disponibles. |
| PATCH  | `/api/tickets/{id}/status`     | Autenticado           | Según la tabla 3.3.                                                       |
| PATCH  | `/api/tickets/{id}/priority`   | `Admin`, `Technician` | Technician solo en asignados.                                             |
| POST   | `/api/tickets/{id}/comments`   | Autenticado           | Según rol y alcance. Ticket no `Closed`.                                  |
| GET    | `/api/tickets/{id}/history`    | Autenticado           | Según rol y alcance.                                                      |
| GET    | `/api/categories`              | Autenticado           | Admin puede ver también las inactivas.                                    |
| POST   | `/api/categories`              | `Admin`               |                                                                           |
| PATCH  | `/api/categories/{id}`         | `Admin`               |                                                                           |
| GET    | `/api/users`                   | `Admin`               |                                                                           |
| POST   | `/api/users`                   | `Admin`               |                                                                           |
| PATCH  | `/api/users/{id}`              | `Admin`               | Activar o desactivar.                                                     |
| GET    | `/api/dashboard/summary`       | Autenticado           | Contenido según rol.                                                      |

Las columnas "Acceso" se aplican con `[Authorize]` y roles o políticas. La columna "Alcance" no puede resolverse con atributos, porque depende de los datos del ticket. Se valida en la capa de servicios.

## 5. Respuestas de la API ante accesos denegados

| Situación                                                        | Código | Ejemplo                                                                             |
| ---------------------------------------------------------------- | ------ | ----------------------------------------------------------------------------------- |
| No hay token, está vencido o es inválido                         | 401    | Llamar a `/api/tickets` sin autenticarse.                                           |
| El rol no puede usar el endpoint                                 | 403    | Un `Requester` llama a `POST /api/users`.                                           |
| La incidencia existe pero está fuera del alcance del usuario     | 404    | Un `Requester` consulta el ticket de otro empleado.                                 |
| El usuario tiene acceso al ticket pero no puede hacer esa acción | 403    | Un técnico intenta cambiar el estado de un ticket disponible que no se ha asignado. |
| La acción rompe una regla de negocio o la transición no existe   | 400    | Cerrar un ticket que no está `Resolved`.                                            |
| Conflicto de concurrencia                                        | 409    | Dos técnicos se autoasignan el mismo ticket.                                        |
| Datos de entrada inválidos                                       | 400    | Título vacío o categoría inactiva.                                                  |

**Decisión: 404 para incidencias fuera de alcance.** Cuando una incidencia existe pero el usuario no tiene acceso a ella, la API responde 404 en lugar de 403. Así no se revela la existencia de tickets ajenos. Esto afecta a las consultas por identificador, no a los endpoints protegidos solo por rol, que devuelven 403.

## 6. Reglas generales de seguridad

1. Cada operación valida el rol **y** el alcance en el backend. Ocultar botones en Angular no es una medida de seguridad.
2. La identidad del usuario sale siempre del token (claims), nunca de datos enviados en el cuerpo o la URL. Por ejemplo, el `RequesterId` de un ticket nuevo es el usuario autenticado, no un campo del request.
3. Las listas se filtran en la consulta a la base de datos según el rol, no en memoria después de descargar todo.
4. Un usuario desactivado no puede iniciar sesión ni ser elegido como técnico asignado.
5. Las acciones del administrador sobre incidencias se limitan a asignar, reasignar, cambiar prioridad, cerrar y reabrir. El `Admin` no trabaja las incidencias (no cambia a `InProgress`, `WaitingForUser` ni registra soluciones).
