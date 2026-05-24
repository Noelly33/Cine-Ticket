# Cine-Ticket
API para gestion de tickets de cine / CI/CD con Jenkins

# Descripción: 
Este proyecto implementa un pipeline de Integración Continua y Despliegue Continuo (CI/CD) para la aplicación Cine-Ticket API, utilizando Jenkins como servidor de automatización principal.

# Flujo Detallado del Pipeline:

- Integración GitHub + Jenkins

/github-webhook/

- Restauración de Dependencias

dotnet restore

- Compilación del Proyecto

dotnet build --configuration Release

- Ejecución de Pruebas

dotnet test

- Publicación de Artefactos

dotnet publish -c Release -o ./publish

- Construcción del Contenedor Docker

docker build -t app-name:tag .

- Despliegue en AWS EC2

docker stop container-name

docker run -d -p 80:80 app-name:tag
