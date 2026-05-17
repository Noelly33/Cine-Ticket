pipeline {
    agent {
        label 'docker-agent'
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        SOLUTION      = 'CineTicket.sln'
        PROJECT_API   = 'CineTicket.API/CineTicket.API.csproj'
        PROJECT_TESTS = 'CineTicket.Tests/CineTicket.Tests.csproj'
        PUBLISH_DIR   = 'publish'
        TEST_RESULTS  = 'TestResults/junit-results.xml'
        IMAGE_NAME    = 'cineticket-api'
        CONTAINER_NAME = 'cineticket-api'
        APP_PORT      = '8080'
    }

    stages {

        stage('Build') {
            steps {
                echo '== Restaurando dependencias =='
                sh "dotnet restore ${SOLUTION}"

                echo '== Compilando solución en modo Release =='
                sh "dotnet build ${SOLUTION} --configuration Release --no-restore"
            }
        }

        stage('Test') {
            steps {
                echo '== Ejecutando pruebas unitarias =='
                sh "dotnet test ${PROJECT_TESTS} --configuration Release --no-build --logger \"junit;LogFilePath=../${TEST_RESULTS}\""
            }
            post {
                always {
                    junit "${TEST_RESULTS}"
                }
            }
        }

        stage('Publish') {
            steps {
                echo '== Publicando artefacto de despliegue =='
                sh "dotnet publish ${PROJECT_API} --configuration Release --no-build --output ${PUBLISH_DIR}"
            }
        }

        stage('Build Image') {
            steps {
                echo '== Construyendo imagen Docker =='
                sh "docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} -t ${IMAGE_NAME}:latest ."
            }
        }

        stage('Deploy') {
            steps {
                echo '== Deteniendo contenedor anterior (si existe) =='
                sh "docker stop ${CONTAINER_NAME} || true"
                sh "docker rm ${CONTAINER_NAME} || true"

                echo '== Desplegando nuevo contenedor =='
                sh "docker run -d --name ${CONTAINER_NAME} -p ${APP_PORT}:8080 --restart unless-stopped ${IMAGE_NAME}:latest"

                echo '== Verificando que el contenedor está corriendo =='
                sh "docker ps --filter name=${CONTAINER_NAME} --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'"
            }
        }
    }

    post {
        success {
            echo "Pipeline completado exitosamente. Contenedor '${CONTAINER_NAME}' desplegado en el puerto ${APP_PORT}."
        }
        failure {
            echo 'Pipeline fallido. Revisa los logs para mas detalles.'
        }
        always {
            echo 'Fin del pipeline CineTicket.'
        }
    }
}
