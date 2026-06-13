Envíos Rápidos GT — API REST


Universidad Mariano Gálvez de Guatemala | Centro Universitario de Jalapa

Facultad de Ingeniería | Análisis de Sistemas I — Examen Final

Catedrático: Ing. Marco Tulio Valdez | Fecha: 13/Jun/2026
Astrid Mileidy Peña Polanco 0907-23-5304
Link a Render: https://zero907-23-5304-analisisa2026final.onrender.com

Descripción del Proyecto

Sistema de gestión de envíos y paquetería para Envíos Rápidos GT, empresa que opera a nivel nacional con cobertura en 18 departamentos de Guatemala. La API permite registrar, rastrear y gestionar el ciclo de vida completo de cada envío, eliminando el seguimiento manual en oficinas.

Stack tecnológico:


Lenguaje: C# (.NET 9)
Base de datos: SQLite (via Entity Framework Core)
Pruebas: xUnit
Documentación: Swagger / OpenAPI
Despliegue: Render.com (Docker)



Historias de Usuario


RF-01 Registrar un nuevo envío

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe permitir a los operadores de oficina registrar nuevos envíos proporcionando los datos del remitente, destinatario, peso y oficinas de origen/destino. El sistema calcula automáticamente la tarifa según el peso, genera un código de rastreo único con formato ENV-YYYYMMDD-XXXX, y aplica un descuento del 5% si el cliente presenta NIT válido.

Historia de Usuario
Como operador de oficina, quiero registrar un nuevo envío en el sistema, para que el cliente reciba un código de rastreo y la tarifa calculada automáticamente.

Backend – Registro de envío

Criterios de Aceptación (Backend)


Dado que el operador envía los datos del envío (peso, remitente, destinatario, oficinas), cuando todos los campos son válidos, entonces el sistema debe crear el registro y retornar la respuesta en un tiempo máximo de 500 ms
Dado que se crea el envío, cuando el peso es ≤ 1 kg, entonces la tarifa debe ser Q 25.00
Dado que se crea el envío, cuando el peso es entre 1.01 y 5 kg, entonces la tarifa debe ser Q 45.00
Dado que se crea el envío, cuando el peso es entre 5.01 y 10 kg, entonces la tarifa debe ser Q 75.00
Dado que se crea el envío, cuando el peso es mayor a 10 kg, entonces la tarifa debe ser Q 100.00
Dado que el cliente tiene NIT válido (tieneNit = true), entonces el sistema debe aplicar un descuento del 5% sobre la tarifa calculada y almacenar ambos valores (tarifa original y tarifa final)
Dado que el envío se crea exitosamente, entonces el sistema debe generar automáticamente un código de rastreo con formato ENV-YYYYMMDD-XXXX
Dado que el envío se registra, entonces el estado inicial debe ser siempre Registrado y debe crearse la primera entrada en el historial con la oficina de origen y timestamp automático


Reglas de Negocio (Backend)


El peso debe ser mayor a 0 kg
El nombre del remitente es obligatorio
El nombre del destinatario es obligatorio
La oficina de origen y destino son obligatorias
La tarifa se calcula automáticamente según los rangos de peso definidos
El descuento por NIT se aplica únicamente cuando tieneNit = true
El código de rastreo es generado por el sistema, no por el usuario


Frontend – Formulario de registro de envío

Criterios de Aceptación (Frontend)


Dado que el operador accede al formulario de registro, entonces debe cargarse en menos de 2 segundos
Dado que el operador no completa los campos requeridos e intenta enviar, entonces se deben mostrar mensajes de validación en menos de 300 ms
Dado que el operador ingresa un peso válido, entonces el sistema debe mostrar una previsualización de la tarifa calculada en tiempo real
Dado que el registro es exitoso, entonces el sistema debe mostrar el código de rastreo generado y la tarifa final de forma clara
Dado que ocurre un error de conexión, entonces debe mostrarse el mensaje "Error al registrar el envío, intente nuevamente"


Reglas de Negocio (Frontend)


Todos los campos del formulario son obligatorios excepto la opción de NIT
El peso debe aceptar únicamente valores numéricos positivos
El checkbox de NIT debe estar desmarcado por defecto
No se debe enviar el formulario con campos vacíos



RF-02 Consultar todos los envíos

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe permitir a los supervisores de operaciones consultar el listado completo de todos los envíos registrados, incluyendo su estado actual, historial de movimientos, tarifas e información de remitente y destinatario.

Historia de Usuario
Como supervisor de operaciones, quiero ver la lista completa de envíos en el sistema, para tener visibilidad general del estado de las operaciones.

Backend – Consulta de listado de envíos

Criterios de Aceptación (Backend)


Dado que el supervisor solicita el listado de envíos, cuando existen registros en la base de datos, entonces el sistema debe retornar todos los envíos con sus datos completos en un tiempo máximo de 1000 ms
Dado que se retorna el listado, entonces cada envío debe incluir: ID, código de rastreo, estado actual, peso, tarifa, tarifa final, remitente, destinatario, oficinas y fecha de registro
Dado que se retorna el listado, entonces cada envío debe incluir su historial completo de estados ordenado cronológicamente
Dado que no existen envíos registrados, entonces el sistema debe retornar una lista vacía con código HTTP 200


Reglas de Negocio (Backend)


El endpoint no requiere filtros obligatorios; retorna todos los registros
El historial de estados se incluye siempre en la respuesta
Los envíos se retornan sin un orden específico garantizado


Frontend – Listado de envíos

Criterios de Aceptación (Frontend)


Dado que el supervisor accede a la pantalla de envíos, entonces la lista debe cargarse en menos de 2 segundos
Dado que la lista carga, entonces debe mostrarse el código de rastreo, estado actual y fechas de cada envío de forma clara
Dado que no hay envíos, entonces debe mostrarse el mensaje "No hay envíos registrados"
Dado que ocurre un error al cargar, entonces debe mostrarse "Error al cargar los envíos, intente nuevamente"


Reglas de Negocio (Frontend)


El estado del envío debe mostrarse con color diferenciado por tipo (ej. verde = Entregado, rojo = Devuelto)
La lista debe ser responsive y legible en dispositivos móviles



RF-03 Consultar un envío por ID

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe permitir a los agentes de soporte consultar los detalles completos de un envío específico utilizando su identificador interno, para poder atender consultas de clientes con información precisa y actualizada.

Historia de Usuario
Como agente de soporte, quiero obtener los detalles de un envío específico por su ID, para responder consultas de clientes con información precisa.

Backend – Consulta de envío por ID

Criterios de Aceptación (Backend)


Dado que el agente solicita un envío con un ID válido, cuando el registro existe, entonces el sistema debe retornar los datos completos del envío con código HTTP 200 en menos de 500 ms
Dado que el ID proporcionado no existe en la base de datos, entonces el sistema debe retornar código HTTP 404 con el mensaje "Envío con ID {id} no encontrado"
Dado que el envío es encontrado, entonces la respuesta debe incluir el historial completo de estados


Reglas de Negocio (Backend)


El ID debe ser un número entero positivo
Si el ID no existe, no debe retornarse información parcial


Frontend – Detalle de envío

Criterios de Aceptación (Frontend)


Dado que el agente ingresa un ID válido, entonces los detalles deben mostrarse en menos de 1 segundo
Dado que el ID no existe, entonces debe mostrarse el mensaje "Envío no encontrado"
Dado que el envío es encontrado, entonces debe mostrarse el historial de estados en orden cronológico


Reglas de Negocio (Frontend)


El campo ID acepta únicamente números enteros positivos
El historial debe mostrarse con timestamp legible en formato local



RF-04 Rastrear envío por código de rastreo

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe permitir a los clientes finales consultar el estado y ubicación de su paquete utilizando el código de rastreo proporcionado al momento del registro, sin necesidad de conocer el ID interno del envío.

Historia de Usuario
Como cliente final, quiero consultar el estado de mi paquete usando mi código de rastreo, para saber dónde está mi envío sin necesitar llamar a la empresa.

Backend – Rastreo por código

Criterios de Aceptación (Backend)


Dado que el cliente proporciona un código de rastreo con formato ENV-YYYYMMDD-XXXX, cuando el código existe, entonces el sistema debe retornar el estado actual e historial completo en menos de 500 ms
Dado que el código de rastreo no existe en la base de datos, entonces el sistema debe retornar código HTTP 404 con el mensaje "Código de rastreo '{codigo}' no encontrado"
Dado que el código es válido, entonces la respuesta debe incluir estado actual, ubicación del último movimiento y el historial completo ordenado cronológicamente


Reglas de Negocio (Backend)


El código de rastreo es sensible al formato ENV-YYYYMMDD-XXXX
No se expone información interna del sistema (IDs de base de datos) al cliente


Frontend – Formulario de rastreo

Criterios de Aceptación (Frontend)


Dado que el cliente ingresa su código de rastreo y presiona "Rastrear", entonces el resultado debe mostrarse en menos de 2 segundos
Dado que el código no existe, entonces debe mostrarse "Código de rastreo no encontrado. Verifique e intente nuevamente"
Dado que el rastreo es exitoso, entonces debe mostrarse claramente el estado actual, la última ubicación y el historial de movimientos
Dado que ocurre un error de conexión, entonces debe mostrarse "Error al rastrear el envío, intente nuevamente"


Reglas de Negocio (Frontend)


El campo de código de rastreo acepta únicamente el formato ENV-YYYYMMDD-XXXX
El historial se muestra de más reciente a más antiguo



RF-05 Actualizar el estado de un envío

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe permitir a mensajeros y bodegueros actualizar el estado de un envío en cada punto del proceso logístico. Cada actualización debe registrar la ubicación (oficina) donde se realiza el cambio, un timestamp automático y notas opcionales. Las transiciones de estado son unidireccionales y siguen un flujo predefinido.

Historia de Usuario
Como mensajero o bodeguero, quiero actualizar el estado de un envío al momento de cada movimiento, para que el sistema refleje la ubicación real del paquete en tiempo real.

Backend – Actualización de estado

Criterios de Aceptación (Backend)


Dado que se solicita un cambio de estado válido (Registrado → EnTransito), cuando el envío existe, entonces el sistema debe actualizar el estado y registrar en el historial en menos de 500 ms
Dado que se intenta una transición inválida (ej. de Registrado a Entregado directamente), entonces el sistema debe rechazar la operación con código HTTP 400 y el mensaje "No se puede cambiar de '{estadoActual}' a '{nuevoEstado}'"
Dado que el envío no existe, entonces el sistema debe retornar código HTTP 404
Dado que se actualiza el estado, entonces el historial debe registrar: nuevo estado, ubicación, timestamp automático (UTC) y notas opcionales
Dado que el nuevo estado es EnReparto, entonces el sistema debe incrementar el contador de intentos de entrega


Flujo de estados permitido:

Registrado → EnTransito → EnReparto → Entregado
                                   ↘
                               EnDevolucion → Devuelto

Reglas de Negocio (Backend)


Los estados solo pueden avanzar en la dirección definida, nunca retroceder
La ubicación es obligatoria en cada cambio de estado
El timestamp se genera automáticamente en el servidor (no lo envía el cliente)
Las notas son opcionales


Frontend – Actualización de estado

Criterios de Aceptación (Frontend)


Dado que el operador selecciona un nuevo estado válido e ingresa la ubicación, entonces el cambio debe reflejarse en pantalla en menos de 1 segundo
Dado que la transición es inválida, entonces debe mostrarse el mensaje de error del backend de forma clara
Dado que el cambio es exitoso, entonces el historial visible debe actualizarse automáticamente


Reglas de Negocio (Frontend)


Solo se muestran como opciones los estados a los que puede transicionar el envío desde su estado actual
La ubicación es un campo obligatorio visible siempre
Las notas son un campo opcional de texto libre



RF-06 Control automático de intentos de entrega fallidos

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe controlar automáticamente la cantidad de intentos de entrega fallidos. Al alcanzar el tercer intento sin éxito, el sistema debe cambiar el estado del envío a EnDevolucion de forma automática, sin requerir intervención manual del operador.

Historia de Usuario
Como sistema automatizado, quiero que al tercer intento fallido de entrega el envío pase a devolución automáticamente, para que no se acumulen paquetes sin gestión en las oficinas.

Backend – Control de intentos

Criterios de Aceptación (Backend)


Dado que se registra un paso a estado EnReparto, entonces el sistema debe incrementar el contador de intentos de entrega del envío
Dado que el contador de intentos llega a 3, entonces el sistema debe cambiar automáticamente el estado a EnDevolucion sin esperar una solicitud adicional
Dado que el cambio automático ocurre, entonces el historial debe registrar la razón: "Máximo de intentos alcanzado (3). Enviado a devolución automáticamente"
Dado que el envío está en EnDevolucion, entonces no debe ser posible intentar entregarlo nuevamente


Reglas de Negocio (Backend)


El máximo de intentos de entrega es 3
El cambio a EnDevolucion es irreversible una vez alcanzado el límite
El contador de intentos es visible en la respuesta del API


Frontend – Visualización de intentos

Criterios de Aceptación (Frontend)


Dado que el envío tiene intentos de entrega registrados, entonces debe mostrarse visualmente el contador (ej. "Intentos: 2/3")
Dado que el sistema cambia automáticamente a EnDevolucion, entonces la interfaz debe reflejar el nuevo estado sin necesidad de recargar manualmente


Reglas de Negocio (Frontend)


El contador de intentos se muestra siempre que el estado sea EnReparto o posterior
Cuando se alcanza el límite, debe mostrarse un indicador visual de alerta



RF-07 Consultar historial completo de un envío

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe mantener y exponer un historial completo e inmutable de todos los cambios de estado de cada envío, incluyendo la ubicación donde ocurrió cada cambio, el timestamp exacto y las notas registradas. Este historial sirve como auditoría del proceso logístico.

Historia de Usuario
Como supervisor de calidad, quiero ver el historial completo de estados de un envío, para poder auditar el proceso y detectar demoras o irregularidades.

Backend – Historial de estados

Criterios de Aceptación (Backend)


Dado que se consulta cualquier envío (por ID o código), entonces la respuesta siempre debe incluir el historial completo de estados
Dado que el historial se retorna, entonces cada entrada debe contener: ID de entrada, estado, ubicación, notas (si existen) y timestamp en formato UTC
Dado que el historial se retorna, entonces las entradas deben estar ordenadas cronológicamente de más antiguo a más reciente
Dado que no ha habido cambios adicionales desde el registro, entonces el historial debe contener al menos una entrada (la del estado Registrado)


Reglas de Negocio (Backend)


El historial es de solo lectura; no puede modificarse ni eliminarse
Cada cambio de estado genera exactamente una entrada en el historial
El timestamp es generado por el servidor en UTC


Frontend – Visualización del historial

Criterios de Aceptación (Frontend)


Dado que se consulta un envío con historial, entonces cada entrada debe mostrarse con: estado, ubicación, fecha/hora legible y notas
Dado que el historial tiene múltiples entradas, entonces debe mostrarse en línea de tiempo visual de más antiguo a más reciente


Reglas de Negocio (Frontend)


El historial no es editable desde la interfaz
El timestamp se muestra en formato local del usuario (no UTC crudo)



RF-08 Aplicar descuento por NIT válido

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe aplicar automáticamente un descuento del 5% sobre la tarifa calculada cuando el remitente o destinatario presenta un NIT válido al momento de registrar el envío. Ambos valores (tarifa base y tarifa final con descuento) deben almacenarse y mostrarse de forma diferenciada.

Historia de Usuario
Como cajero de oficina, quiero que el sistema aplique automáticamente un 5% de descuento al registrar un envío con NIT, para que los clientes con NIT reciban el beneficio tributario correspondiente.

Backend – Descuento por NIT

Criterios de Aceptación (Backend)


Dado que se registra un envío con tieneNit = true, entonces el sistema debe calcular tarifaFinal = tarifa * 0.95 y almacenar ambos valores
Dado que se registra un envío con tieneNit = false, entonces tarifaFinal debe ser igual a tarifa sin descuento
Dado que se aplica el descuento, entonces el valor de tarifaFinal debe estar redondeado a 2 decimales
Dado que se consulta el envío, entonces la respuesta debe incluir siempre tarifa (original), tarifaFinal (con o sin descuento) y tieneNit


Reglas de Negocio (Backend)


El descuento es del 5% fijo, no configurable por el usuario
El campo tieneNit es booleano; no se valida el número de NIT en esta versión
Una vez registrado el envío, el descuento no puede modificarse


Frontend – Visualización del descuento

Criterios de Aceptación (Frontend)


Dado que el operador marca la opción de NIT, entonces la previsualización de tarifa debe mostrar el precio original tachado y el precio con descuento
Dado que el registro es exitoso con NIT, entonces el comprobante debe mostrar: tarifa base, descuento aplicado (5%) y tarifa final


Reglas de Negocio (Frontend)


El checkbox de NIT debe estar desmarcado por defecto
El descuento se muestra como "- 5% NIT" de forma explícita



RF-09 Validar datos obligatorios al registrar un envío

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe validar que todos los campos obligatorios estén presentes y sean válidos antes de registrar un nuevo envío, retornando mensajes de error descriptivos que permitan al operador corregir los datos ingresados.

Historia de Usuario
Como sistema, quiero validar que todos los datos obligatorios estén presentes al registrar un envío, para que no se ingresen envíos incompletos que generen problemas operativos.

Backend – Validaciones de registro

Criterios de Aceptación (Backend)


Dado que el peso enviado es 0 o negativo, entonces el sistema debe retornar HTTP 400 con el mensaje "El peso debe ser mayor a 0"
Dado que el nombre del remitente está vacío o ausente, entonces el sistema debe retornar HTTP 400 con el mensaje "El nombre del remitente es obligatorio"
Dado que el nombre del destinatario está vacío o ausente, entonces el sistema debe retornar HTTP 400 con el mensaje "El nombre del destinatario es obligatorio"
Dado que todos los campos son válidos, entonces el sistema no debe retornar errores de validación y debe proceder con el registro


Reglas de Negocio (Backend)


Las validaciones se ejecutan antes de intentar guardar en la base de datos
Cada campo inválido genera su propio mensaje de error descriptivo
Los errores de validación retornan siempre código HTTP 400


Frontend – Validaciones en formulario

Criterios de Aceptación (Frontend)


Dado que el operador intenta enviar el formulario con campos vacíos, entonces se deben mostrar mensajes de validación por campo en menos de 300 ms sin enviar la petición al backend
Dado que el backend retorna un error 400, entonces el mensaje debe mostrarse en el formulario de forma legible
Dado que todos los campos son válidos, entonces el formulario se envía al backend sin mensajes de error


Reglas de Negocio (Frontend)


La validación de campos vacíos se hace en el frontend antes de enviar la petición
Los mensajes de error se muestran debajo de cada campo correspondiente
No se deben enviar peticiones al backend con datos claramente inválidos



RF-10 Generar reporte de eficiencia de entregas

Tipo de Requerimiento
Funcional

Descripción General
El sistema debe generar un reporte de eficiencia operativa que permita a la gerencia visualizar el rendimiento de las entregas, incluyendo totales por estado, porcentaje de éxito y total recaudado. Este reporte se calcula en tiempo real sobre todos los envíos registrados en el sistema.

Historia de Usuario
Como gerente de operaciones, quiero obtener un reporte de eficiencia de entregas, para medir el rendimiento de la empresa y tomar decisiones basadas en datos.

Backend – Generación de reporte

Criterios de Aceptación (Backend)


Dado que se solicita el reporte, entonces el sistema debe calcularlo en tiempo real en menos de 1000 ms
Dado que el reporte se genera, entonces debe incluir: total de envíos, cantidad de entregados, cantidad de devueltos, cantidad en proceso, porcentaje de entrega exitosa y total recaudado
Dado que el porcentaje de entrega se calcula, entonces debe ser (entregados / total) * 100 redondeado a 2 decimales
Dado que el total recaudado se calcula, entonces debe sumar únicamente los tarifaFinal de los envíos con estado Entregado
Dado que no hay envíos registrados, entonces todos los valores numéricos deben retornarse en 0


Reglas de Negocio (Backend)


El reporte es de solo lectura; no modifica ningún dato
Solo los envíos en estado Entregado suman al total recaudado
El porcentaje de entrega es 0 cuando no hay envíos registrados (evitar división por cero)


Frontend – Visualización del reporte

Criterios de Aceptación (Frontend)


Dado que el gerente accede al reporte, entonces debe cargarse en menos de 2 segundos
Dado que el reporte carga, entonces cada métrica debe mostrarse en una tarjeta visual diferenciada (total, entregados, devueltos, en proceso, porcentaje, recaudado)
Dado que no hay datos, entonces debe mostrarse el mensaje "Sin datos suficientes para generar el reporte"


Reglas de Negocio (Frontend)


El porcentaje de entrega se muestra con barra de progreso visual
El total recaudado se muestra en formato de moneda guatemalteca (Q)
El reporte tiene un botón "Actualizar" para refrescar los datos en tiempo real



Estructura del Proyecto

EnviosRapidosGT/
├── EnviosRapidosGT.sln
├── Dockerfile
├── .gitignore
├── README.md
└── src/
    ├── EnviosRapidosGT.API/
    │   ├── Controllers/
    │   │   └── EnviosController.cs      # 6 endpoints REST
    │   ├── Data/
    │   │   └── AppDbContext.cs          # EF Core + SQLite
    │   ├── DTOs/
    │   │   └── EnvioDTOs.cs            # Request/Response records
    │   ├── Models/
    │   │   └── Envio.cs                # Entidades: Envio, HistorialEstado
    │   ├── Services/
    │   │   └── EnvioService.cs         # Lógica de negocio
    │   ├── Program.cs
    │   └── appsettings.json
    └── EnviosRapidosGT.Tests/
        └── EnvioServiceTests.cs        # 12 pruebas unitarias con xUnit


Endpoints de la API

MétodoRutaDescripciónRFPOST/api/enviosRegistrar nuevo envíoRF-01GET/api/enviosListar todos los envíosRF-02GET/api/envios/{id}Obtener envío por IDRF-03GET/api/envios/rastreo/{codigo}Rastrear por códigoRF-04PUT/api/envios/{id}/estadoActualizar estadoRF-05, RF-06GET/api/envios/reporte/eficienciaReporte de eficienciaRF-10


Flujo de Estados

Registrado → EnTransito → EnReparto → Entregado
                                   ↘
                               EnDevolucion → Devuelto


Al 3er intento fallido en EnReparto, el sistema cambia automáticamente a EnDevolucion.




Tarifas

PesoTarifa≤ 1 kgQ 25.001.01 – 5 kgQ 45.005.01 – 10 kgQ 75.00> 10 kgQ 100.00


Clientes con NIT válido reciben 5% de descuento sobre la tarifa calculada.


Cómo ejecutar localmente

Requisitos


.NET 9 SDK
Git


Pasos

bash# 1. Clonar el repositorio
git clone https://github.com/AstridPolanco/0907-23-5304_ANALISISA2026FINAL.git
cd 0907-23-5304_ANALISISA2026FINAL

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar la API
cd src/EnviosRapidosGT.API
dotnet run

# 4. Abrir Swagger UI en el navegador
# http://localhost:5000

Ejecutar pruebas unitarias

bashdotnet test
# Resultado esperado: Passed! - Failed: 0, Passed: 12


Despliegue en Render.com


Crear cuenta en render.com
Conectar repositorio de GitHub
Seleccionar Web Service → Docker
Render detecta el Dockerfile automáticamente
El puerto configurado es 10000
Hacer clic en Deploy
