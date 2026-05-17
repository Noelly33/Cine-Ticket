pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        SOLUTION        = 'CineTicket.sln'
        PROJECT_API     = 'CineTicket.API/CineTicket.API.csproj'
        PROJECT_TESTS   = 'CineTicket.Tests/CineTicket.Tests.csproj'
        PUBLISH_DIR     = 'publish'
        TEST_RESULTS    = 'TestResults/junit-results.xml'
    }

    stages {

        stage('Build') {
            steps {
                echo '== Restaurando dependencias =='
                bat "dotnet restore ${SOLUTION}"

                echo '== Compilando solución en modo Release =='
                bat "dotnet build ${SOLUTION} --configuration Release --no-restore"
            }
        }

        stage('Test') {
            steps {
                echo '== Ejecutando pruebas unitarias =='
                bat "dotnet test ${PROJECT_TESTS} --configuration Release --no-build --logger \"junit;LogFilePath=../${TEST_RESULTS}\""
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
                bat "dotnet publish ${PROJECT_API} --configuration Release --no-build --output ${PUBLISH_DIR}"
            }
        }

        stage('Deploy') {
            steps {
                echo '== Desplegando en entorno de prueba =='
                echo "Artefactos disponibles en: ${WORKSPACE}\\${PUBLISH_DIR}"
                bat "dir ${PUBLISH_DIR}"
            }
        }
    }

    post {
        success {
            echo 'Pipeline completado exitosamente. Todas las pruebas pasaron.'
        }
        failure {
            echo 'Pipeline fallido. Revisa los logs para mas detalles.'
        }
        always {
            echo 'Fin del pipeline CineTicket.'
        }
    }
}
