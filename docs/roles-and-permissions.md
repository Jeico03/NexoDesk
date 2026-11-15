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

| Acción                                   | Requester | Technician | Admin                         |
| ---------------------------------------- | --------- | ---------- | ----------------------------- |
| Iniciar sesión                           | Sí        | Sí         | Sí                            |
| Consultar sus propios datos (`/auth/me`) | Sí        | Sí         | Sí                            |
| Crear usuarios                           | No        | No         | Sí                            |
| Consultar y filtrar usuarios             | No        | No         | Sí                            |
| Activar y desactivar usuarios            | No        | No         | Sí (excepto su propia cuenta) |

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
- Solo el `Requester` crea incidencias. El `Admin` y el `Technician` no crean incidencias en la v1 (ver P-03 en `requirements.md`).

### 3.3. Cambios de estado por rol

Esta tabla resume quién puede ejecutar cada transición. Las condiciones y efectos completos están en `ticket-workflow.md`.

| ID   | Transición                                                               | Requester | Technician                   | Admin        |
| ---- | ------------------------------------------------------------------------ | --------- | ---------------------------- | ------------ |
| T-01 | `Open` → `Assigned`                                                      | No        | Autoasignación (disponibles) | Sí (asignar) |
| T-02 | `Reopened` → `Assigned`                                                  | No        | Autoasignación (disponibles) | Sí (asignar) |
| T-03 | `Assigned` / `InProgress` / `WaitingForUser` → `Assigned` (reasignación) | No        | No                           | Sí           |
| T-04 | `Assigned` → `InProgress`                                                | No        | Asignados                    | No           |
| T-05 | `InProgress` → `WaitingForUser`                                          | No        | Asignados                    | No           |
| T-06 | `WaitingForUser` → `InProgress`                                          | No        | Asignados                    | No           |
| T-07 | `InProgress` → `Resolved`                                                | No        | Asignados (con solución)     | No           |
| T-08 | `Resolved` → `Closed`                                                    | Propios   | No                           | Todos        |
| T-09 | `Resolved` → `Reopened`                                                  | Propios   | No                           | Todos        |

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

| Método | Endpoint                       | Acceso                | Alcance (validado en el servicio)                                                                                                                                        |
| ------ | ------------------------------ | --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| POST   | `/api/auth/login`              | Anónimo               | Usuario activo con credenciales válidas.                                                                                                                                 |
| GET    | `/api/auth/me`                 | Autenticado           | Usuario del token.                                                                                                                                                       |
| GET    | `/api/tickets`                 | Autenticado           | Filtra según el rol (ver 3.2).                                                                                                                                           |
| POST   | `/api/tickets`                 | `Requester`           | Categoría activa.                                                                                                                                                        |
| GET    | `/api/tickets/{id}`            | Autenticado           | Según rol y alcance.                                                                                                                                                     |
| PATCH  | `/api/tickets/{id}/assignment` | `Admin`, `Technician` | Ejecuta T-01, T-02 y T-03. Admin: asigna o reasigna a cualquier técnico activo. Technician: solo autoasignación de disponibles, y el técnico indicado debe ser él mismo. |
| PATCH  | `/api/tickets/{id}/status`     | Autenticado           | Ejecuta T-04 a T-09, según la tabla 3.3.                                                                                                                                 |
| PATCH  | `/api/tickets/{id}/priority`   | `Admin`, `Technician` | Technician solo en asignados.                                                                                                                                            |
| POST   | `/api/tickets/{id}/comments`   | Autenticado           | Según rol y alcance. Si el ticket está `Closed`, responde 400.                                                                                                           |
| GET    | `/api/tickets/{id}/comments`   | Autenticado           | Según rol y alcance.                                                                                                                                                     |
| GET    | `/api/tickets/{id}/history`    | Autenticado           | Según rol y alcance.                                                                                                                                                     |
| GET    | `/api/categories`              | Autenticado           | Admin puede ver también las inactivas.                                                                                                                                   |
| POST   | `/api/categories`              | `Admin`               |                                                                                                                                                                          |
| PATCH  | `/api/categories/{id}`         | `Admin`               | Editar o desactivar.                                                                                                                                                     |
| GET    | `/api/users`                   | `Admin`               | Admite filtro por rol.                                                                                                                                                   |
| POST   | `/api/users`                   | `Admin`               |                                                                                                                                                                          |
| PATCH  | `/api/users/{id}`              | `Admin`               | Activar o desactivar. No puede aplicarse a la propia cuenta.                                                                                                             |
| GET    | `/api/dashboard/summary`       | Autenticado           | Contenido según rol.                                                                                                                                                     |

La columna "Acceso" se aplica con `[Authorize]` y roles o políticas. La columna "Alcance" no puede resolverse con atributos, porque depende de los datos del ticket. Se valida en la capa de servicios.

## 5. Respuestas de la API ante accesos denegados

| Situación                                                              | Código | Ejemplo                                             |
| ---------------------------------------------------------------------- | ------ | --------------------------------------------------- |
| No hay token, está vencido o es inválido                               | 401    | Llamar a `/api/tickets` sin autenticarse.           |
| Inicio de sesión fallido (contraseña incorrecta o usuario desactivado) | 401    | Mensaje genérico, sin distinguir la causa.          |
| El rol no puede usar el endpoint                                       | 403    | Un `Requester` llama a `POST /api/users`.           |
| La incidencia no existe o está fuera del alcance del usuario           | 404    | Un `Requester` consulta el ticket de otro empleado. |
| El usuario tiene acceso al ticket pero no puede hacer esa acción       | 403    | Un técnico intenta cerrar un ticket `Resolved`.     |
| La acción rompe una regla de negocio o la transición no existe         | 400    | Cerrar un ticket que no está `Resolved`.            |
| Datos de entrada inválidos                                             | 400    | Título vacío o categoría inactiva.                  |
| Conflicto de concurrencia                                              | 409    | Dos técnicos se autoasignan el mismo ticket.        |

**Decisión: 404 para incidencias fuera de alcance.** Cuando una incidencia existe pero el usuario no tiene acceso a ella, la API responde 404 en lugar de 403, y es exactamente la misma respuesta que para una incidencia que no existe. Así no se revela la existencia de tickets ajenos. Esto afecta a todas las operaciones por identificador de ticket, no a los endpoints protegidos solo por rol, que devuelven 403.

El orden en que el servicio evalúa estos casos está en `ticket-workflow.md`, sección 7.

## 6. Reglas generales de seguridad

1. Cada operación valida el rol **y** el alcance en el backend. Ocultar botones en Angular no es una medida de seguridad.
2. La identidad del usuario sale siempre del token (claims), nunca de datos enviados en el cuerpo o la URL. Por ejemplo, el `RequesterId` de un ticket nuevo es el usuario autenticado, no un campo del request.
3. Las listas se filtran en la consulta a la base de datos según el rol, no en memoria después de descargar todo.
4. Un usuario desactivado no puede iniciar sesión ni ser elegido como técnico asignado. Qué ocurre con un token que sigue vigente tras la desactivación está pendiente (P-01 en `requirements.md`).
5. Las acciones del administrador sobre incidencias se limitan a asignar, reasignar, cambiar prioridad, cerrar y reabrir. El `Admin` no trabaja las incidencias (no cambia a `InProgress`, `WaitingForUser` ni registra soluciones).
6. Un `Admin` no puede desactivar su propia cuenta, para evitar que el sistema se quede sin administradores. Si lo intenta, la API responde 400.
