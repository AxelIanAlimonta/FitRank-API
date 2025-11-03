# Proyecto ASP.NET API REST

Este proyecto implementa una API REST con **ASP.NET Core** y utiliza **PostgreSQL** como base de datos.

## Requisitos previos

* [Docker Desktop](https://www.docker.com/products/docker-desktop)

  > Es necesario que **Docker Desktop esté en ejecución** para que funcionen los comandos de `docker compose`.

---

## Levantar base de datos en contenedor

El proyecto incluye un `docker-compose.yml` ya configurado en la carpeta:

```
tools/compose-postgres-dev/docker-compose.yml
```

Para levantar la base de datos:

```bash
cd tools/compose-postgres-dev
docker compose up -d
```

Esto crea un contenedor con PostgreSQL configurado y disponible para la API .

---

## Migraciones y estructura de base de datos

> ⚠️ Importante: el contenedor solo crea la **instancia vacía de PostgreSQL**, no la estructura de tablas.
> Es necesario aplicar las migraciones de **Entity Framework** luego de clonar el repositorio.

Ejecutar el siguiente comando desde la **Consola del Administrador de Paquetes** en Visual Studio:

```bash
Update-Database
```

Esto genera las tablas y esquema inicial en la base de datos. No necesitas hacer ningun cambio en cadena de conexion, ya esta todo configurado

---

## Ejecutar la API

Una vez que la base de datos esté lista, podés correr la aplicación con:

```bash
dotnet run --project FitRank.API
```

La API quedará disponible en:

* Swagger UI: [https://localhost:7226/swagger/index.html](https://localhost:7226/swagger/index.html)
* Endpoints REST: `https://localhost:7226/api/...`

---

## Notas

* Para reiniciar la base de datos, podés bajar el contenedor y volver a levantarlo:

```bash
docker compose down -v
docker compose up -d
Update-Database
```

