pipeline {
    agent {
        label 'docker-agent'
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        SOLUTION           = 'CineTicket.sln'
        PROJECT_API        = 'CineTicket.API/CineTicket.API.csproj'
        PROJECT_INFRA      = 'CineTicket.Infrastructure/CineTicket.Infrastructure.csproj'
        PROJECT_TESTS      = 'CineTicket.Tests/CineTicket.Tests.csproj'
        PROJECT_NOTIFY     = 'CineTicket.Notifications/CineTicket.Notifications.csproj'
        PUBLISH_DIR        = 'publish'
        TEST_RESULTS       = 'TestResults/junit-results.xml'
        IMAGE_NAME         = 'cineticket-api'
        CONTAINER_NAME     = 'cineticket-api'
        APP_PORT           = '8080'

        // Base de datos
        DB_CONTAINER_NAME  = 'cineticket-sqlserver'
        NETWORK_NAME       = 'cineticket-net'
        DB_PORT            = '1433'
        DB_NAME            = 'CineTicket'
        SA_PASSWORD        = credentials('cineticket-sa-password')

        // Notificaciones — credencial 'resend-api-key' configurada en Jenkins
        RESEND_API_KEY     = credentials('resend-api-key')
        NOTIFY_RECIPIENTS  = 'angeldaniel6709@gmail.com,gnoely319@gmail.com'
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
                    if docker ps -aq --filter name=^/${DB_CONTAINER_NAME}\$ | grep -q .; then
                        docker start ${DB_CONTAINER_NAME} || true
                        docker network connect ${NETWORK_NAME} ${DB_CONTAINER_NAME} || true
                    else
                        docker run -d \
                            --name ${DB_CONTAINER_NAME} \
                            --network ${NETWORK_NAME} \
                            -e ACCEPT_EULA=Y \
                            -e MSSQL_SA_PASSWORD='${SA_PASSWORD}' \
                            -p ${DB_PORT}:1433 \
                            --restart unless-stopped \
                            mcr.microsoft.com/azure-sql-edge:latest
                    fi
                """

                sh """
                    echo 'Esperando que el motor SQL este listo para aceptar conexiones...'
                    for i in \$(seq 1 30); do
                        if docker exec ${DB_CONTAINER_NAME} \
                            /opt/mssql-tools/bin/sqlcmd \
                            -S localhost -U sa -P '${SA_PASSWORD}' \
                            -Q 'SELECT 1' -b -l 2 > /dev/null 2>&1; then
                            echo "Base de datos lista en el intento \$i."
                            break
                        fi
                        if [ \$i -eq 30 ]; then
                            echo 'La base de datos no respondio despues de 150 segundos.'
                            docker logs ${DB_CONTAINER_NAME}
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
                sh "docker stop ${CONTAINER_NAME} || true"
                sh "docker rm   ${CONTAINER_NAME} || true"

                sh """
                    docker run -d \
                        --name ${CONTAINER_NAME} \
                        --network ${NETWORK_NAME} \
                        -p ${APP_PORT}:8080 \
                        -e ConnectionStrings__DefaultConnection="Server=${DB_CONTAINER_NAME},1433;Database=${DB_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True" \
                        --restart unless-stopped \
                        ${IMAGE_NAME}:latest
                """

                sh "docker ps --filter network=${NETWORK_NAME} --format 'table {{.Names}}\t{{.Status}}\t{{.Ports}}'"
            }
        }
    }

    post {
        success {
            script {
                env.BUILD_STATUS = 'SUCCESS'
                env.STAGE_NAME   = 'Deploy'
            }
            sh """
                dotnet run --project ${PROJECT_NOTIFY} -- "${NOTIFY_RECIPIENTS}"
            """
            echo "Pipeline completado exitosamente. Contenedor '${CONTAINER_NAME}' desplegado en el puerto ${APP_PORT}."
        }
        failure {
            script {
                env.BUILD_STATUS = 'FAILURE'
                env.STAGE_NAME   = env.STAGE_NAME ?: 'Unknown'
            }
            sh """
                dotnet run --project ${PROJECT_NOTIFY} -- "${NOTIFY_RECIPIENTS}"
            """
            echo 'Pipeline fallido. Revisa los logs para mas detalles.'
        }
        always {
            echo 'Fin del pipeline CineTicket.'
        }
    }
}
