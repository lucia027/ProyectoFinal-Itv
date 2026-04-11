#  Sistema de Gestión de ITV 🚗🛠️

Solución integral para la gestión técnica de vehículos, diseñada con una arquitectura de servicios robusta, múltiples sistemas de persistencia y reglas de negocio estrictas.

---

## 🚘 Especificaciones del Modelo
El sistema gestiona objetos **Vehículo** con los siguientes atributos:
* **Matrícula:** Formato único `NNNNLLL` (No repetible).
* **Marca:** Datos técnicos.  
* **Modelo:** Datos técnicos.  
* **Cilindrada:** Datos técnicos.  
* **Motor:** Tipos admitidos: Diésel, Gasolina, Híbrido y Eléctrico.
* **Dueño:** Identificado por su DNI.

---

## 💾 Persistencia y Almacenamiento
Capacidad de gestión de datos flexible mediante diferentes estrategias:

### Repositorios
* **Tipos:** Memoria, JSON y Binario Secuencial.
* **Paginación:** Todos los métodos de consulta general (`GetAll`) están paginados.
* **Borrados:** Soporte para borrado **Físico** y borrado **Lógico**.

### Conexión a Bases de Datos (SQLite)
Para demostrar la versatilidad del sistema, se han implementado tres capas de acceso a datos distintas:
* **ADO.NET:** Gestión directa mediante comandos SQL y DataReaders para un control total.
* **Dapper:** Uso de este micro-ORM para mapear consultas SQL de forma ágil y eficiente.
* **Entity Framework Core (EFC):** Implementación de un ORM completo para la gestión del modelo de datos y migraciones.

### Almacenamiento y Backup
* **Formatos de exportación:** CSV, JSON, XML y Binario Secuencial.
* **Servicio de Backup:** Generación de copias de seguridad comprimidas en formato `.zip`.
* **Caché:** Implementación de **Caché LRU** de tamaño fijo para optimización de consultas.

---

## 📋 Reglas de Negocio y Lógica
El sistema garantiza la integridad de los datos mediante:
1.  **Validador:** Comprobación estricta de formatos y requisitos.
2.  **Restricción de propiedad:** Un dueño no puede tener más de **tres vehículos** registrados.
3.  **Configuración:** Parámetros del sistema gestionados vía `appsettings.json`.
4.  **Logger:** Registro detallado de actividad tanto en **Consola** como en **Fichero**.
5.  **Excepciones:** Sistema de errores personalizado para una depuración precisa.

---

## 📃 Calidad y Documentación
* **Documentación:** Uso de **XMLDoc** en todo el código fuente.
* **Testing:** Proyecto completamente testeado.
* **Cobertura:** Informe detallado de la cobertura de código incluida.
* **SonarQube:** Uso en el proyecto de SonarQube.
* **Diagramas:** Repositorio con todos los diagramas vistos en clase (Clases, Casos de Uso, Secuencia, etc.).
* **Control de Versiones:** Uso de flujo de trabajo por ramas.

---

## 🖥️ Interfaz de Usuario
Disponemos de una interfaz funcional que incluye:
* **Operaciones:** Crear, Actualizar, Buscar y Borrar.
* **Extras:** Apartado "Acerca de" e icono personalizado de la aplicación.

---

## 🎬 Entrega Multimedia
Se adjunta un **vídeo explicativo** donde el alumno:
1.  Ejecuta la aplicación y muestra su funcionamiento.
2.  Enseña los resultados de la cobertura de tests.
3.  Explica la arquitectura y lógica del programa.

---
