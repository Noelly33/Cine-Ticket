pipeline {
    agent {
        label 'docker-agent'
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        SOLUTION      = 'CineTicket.sln'
        PROJECT_API   = 'CineTicket.API/CineTicket.API.csproj'
        PROJECT_INFRA = 'CineTicket.Infrastructure/CineTicket.Infrastructure.csproj'
        PROJECT_TESTS = 'CineTicket.Tests/CineTicket.Tests.csproj'
        PUBLISH_DIR   = 'publish'
        TEST_RESULTS  = 'TestResults/junit-results.xml'
        IMAGE_NAME    = 'cineticket-api'
        CONTAINER_NAME = 'cineticket-api'
        APP_PORT      = '8080'

        DB_CONTAINER_NAME = 'cineticket-sqlserver'
        NETWORK_NAME      = 'cineticket-net'
        DB_PORT           = '1433'
        DB_NAME           = 'CineTicket'
        SA_PASSWORD       = credentials('cineticket-sa-password')
    }

    stages {

        stage('Build') {
            steps {
                sh "dotnet restore ${SOLUTION}"

                sh "dotnet build ${SOLUTION} --configuration Release --no-restore"
            }
        }

        stage('Test') {
            steps {
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
                sh "dotnet publish ${PROJECT_API} --configuration Release --no-build --output ${PUBLISH_DIR}"
            }
        }

        stage('Build Image') {
            steps {
                sh "docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} -t ${IMAGE_NAME}:latest ."
            }
        }

        stage('Setup Network') {
            steps {
                sh "docker network create ${NETWORK_NAME} || true"
            }
        }

        stage('Start Database') {
            steps {
                sh """
                    # Si el contenedor ya existe pero está detenido, lo iniciamos
                    if docker ps -aq --filter name=^/${DB_CONTAINER_NAME}\$ | grep -q .; then
                        docker start ${DB_CONTAINER_NAME} || true
                        # Asegurar que esté conectado a la red
                        docker network connect ${NETWORK_NAME} ${DB_CONTAINER_NAME} || true
                    else
                        docker run -d \
                            --name ${DB_CONTAINER_NAME} \
                            --network ${NETWORK_NAME} \
                            -e ACCEPT_EULA=Y \
                            -e MSSQL_SA_PASSWORD='${SA_PASSWORD}' \
                            -p ${DB_PORT}:1433 \
                            --restart unless-stopped \
                            mcr.microsoft.com/mssql/server:2022-latest
                    fi
                """

                sh """
                    for i in \$(seq 1 30); do
                        if docker exec ${DB_CONTAINER_NAME} \
                            /opt/mssql-tools18/bin/sqlcmd \
                            -S localhost -U sa -P '${SA_PASSWORD}' \
                            -Q 'SELECT 1' -C -l 2 > /dev/null 2>&1; then
                            echo "SQL Server listo en el intento \$i."
                            break
                        fi
                        if [ \$i -eq 30 ]; then
                            echo 'SQL Server no respondio despues de 150 segundos.'
                            exit 1
                        fi
                        echo "Intento \$i/30 — esperando 5s..."
                        sleep 5
                    done
                """
            }
        }

        stage('Run Migrations') {
            steps {
                echo '== Instalando dotnet-ef (si no está instalado) =='
                sh """
                    export PATH="\$PATH:\$HOME/.dotnet/tools"
                    dotnet tool install --global dotnet-ef || true
                """

                sh """
                    export PATH="\$PATH:\$HOME/.dotnet/tools"
                    dotnet ef database update \
                        --project ${PROJECT_INFRA} \
                        --startup-project ${PROJECT_API} \
                        --connection "Server=localhost,${DB_PORT};Database=${DB_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True"
                """
            }
        }

        stage('Deploy') {
            steps {
                echo '== Deteniendo contenedor anterior (si existe) =='
                sh "docker stop ${CONTAINER_NAME} || true"
                sh "docker rm ${CONTAINER_NAME} || true"

                echo '== Desplegando nuevo contenedor =='
                sh """
                    docker run -d \
                        --name ${CONTAINER_NAME} \
                        --network ${NETWORK_NAME} \
                        -p ${APP_PORT}:8080 \
                        -e ConnectionStrings__DefaultConnection="Server=${DB_CONTAINER_NAME},1433;Database=${DB_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True" \
                        --restart unless-stopped \
                        ${IMAGE_NAME}:latest
                """

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
