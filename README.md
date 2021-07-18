# Company.App.Back
## Features
```
30 min
VSCode
NetCore SDK
Postman/WebBrowser
GitHub (create repo https://github.com/{username}/Company.App.Back)
Azure & AzureDevOpsAccount
```

##  -------  Git and manage Code  -------

```
#Crear directorio base de los proyectos y vincular cambios con github
mkdir Company.App.Back
cd Company.App.Back
echo "# Company.App.Back" >> README.md
git init
git add .
git commit -m "add readme"
git branch -M master
git remote add origin https://github.com/{username}/Company.App.Back.git
git push -u origin master

#Crear directorio para la solución
mkdir App
cd App

#Crear WebApi
dotnet new webapi -n Distributed.Services --no-https
cd Distributed.Services
dotnet build
dotnet run

#open browser: http://localhost:5000/swagger
#press ctrl + c for shut down

cd ..

#Crear XUnit
dotnet new xunit -n Distributed.Services.Test
cd .\Distributed.Services.Test\
dotnet build
dotnet test

cd ..

#Crear Solución y referenciar proyectos y dependencias.
dotnet new sln -n App
dotnet sln App.sln add Distributed.Services/Distributed.Services.csproj Distributed.Services.Test/Distributed.Services.Test.csproj
dotnet add Distributed.Services.Test\Distributed.Services.Test.csproj reference Distributed.Services\Distributed.Services.csproj
dotnet build

cd ..

#Abrir directorio con visual studio code
#Agregar archivo .gitignore
#Subir cambios a github

git add .
git commit -m "basic template"
git push -u origin master
```

##  -------  Continuous Integration on Premise  -------
```
#Acceder a https://dev.azure.com/{username}/_settings/agentpools?poolId=1&view=jobs y crear un agente on-premise llamada DevOps
#Acceder a https://dev.azure.com/{username}/_settings/deploymentpools y crear un Pool de despliegue on-premise llamado IIS-Server
#Acceder a https://dev.azure.com/{username}/DevOps/_settings/adminservices y crear una conexión hacia sonarqube server
#Acceder a https://dev.azure.com/{username}/DevOps/_build
#Crear pipeline
#Seleccionar repositorio GitHub
#Conectar github con azuredevops
#Elegir plantilla: "Starter Pipeline"
#Pegar el siguiente contenido:

trigger:
- master

pool: 'DevOps'

variables:
  artifactName: 'Net5WebApi'
  buildConfiguration: 'Release'
  connectionKey: 'Net5WebApi'

steps:
- task: DotNetCoreCLI@2
  inputs:
    command: 'restore'
    projects: '**/*.csproj'
    feedsToUse: 'select'

- task: DotNetCoreCLI@2
  inputs:
    command: test
    projects: '**/*Test/*.csproj'
    arguments: '--configuration $(buildConfiguration)'

- task: SonarQubePrepare@4
  displayName: 'Prepare sonarqube analisys'
  inputs:
    SonarQube: 'SonarQube'
    scannerMode: 'MSBuild'
    projectKey: '$(connectionKey)'
    projectName: '$(connectionKey)'

- task: DotNetCoreCLI@2
  displayName: 'Build solution'
  inputs:
    command: 'build'
    projects: '**/*.csproj'
    arguments: '--configuration $(buildConfiguration)'

- task: SonarQubeAnalyze@4
  displayName: 'Sonarqube analisys'

- task: SonarQubePublish@4
  displayName: 'Sonarqube publish'
  inputs:
    pollingTimeoutSec: '300'

- task: DotNetCoreCLI@2
  inputs:
    command: 'publish'
    publishWebProjects: true
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'

- task: PublishBuildArtifacts@1
  displayName: 'Publish Artifact'
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: '$(artifactName)'
    publishLocation: 'Container'
	
#Guardar y ejecutar. Se realizará la CI en azuredevops.
```

##  -------  Continuous Delivery on Premise  -------
```
#Acceder a https://dev.azure.com/{username}/_settings/deploymentpools y crear un Pool de despliegue on-premise llamado IIS-Server
#Acceder a https://dev.azure.com/{username}/DevOps/_release y crear un pipeline
#Seleccionar IIS website deployment y colocarle de nombre "dev"
#Elegir tab "tasks"
Sección dev:
	WebSiteName: Net5WebApi
	Binding Port: 5000
Sección IIS Deployment
	Deployment Group: IIS Server
IIS Web Ap Manage
	Physical path: %SystemDrive%\inetpub\wwwroot\Net5WebApi
	Create or update app pool: check
	Application Pool:
		Name: Net5WebApi
		.NET version: No manage code
IIS Web App Deploy
	Remove Additional Files at Destination: check
	
#Renombrar a Net5WebApi
#Asignar Artifact. 
	Project: DevOps
	Source: {username}.Company.App.Back
	Finalmente agregar.
#Seleccionar artefacto y 
	Continuous deployment trigger: activado
```

##  -------  Git and manage Code  -------
```
#Ir al directorio raíz de la fuente y bajar los cambios.
git pull

#Eliminar la referencua a logger en WeatherForecastController.cs  (limpiar las referencias no utilizadas)
#Reemplazar el test unitario por el siguiente código (limpiar las referencias no utilizadas)
        [Fact]
        public void GetMethod_WeatherForecast_ReturnSuccessfully()
        {
            WeatherForecastController controller = new();
            var returnValue = controller.Get() as WeatherForecast[];
            Assert.True(returnValue.Any());
        }

#Subir cambios
git add .
git commit -m "add test unit from get method"
git push -u origin master

#Con eso se activará el CI y CD.
#Revisar Analytics
#Revisar Sumary Test
#Revisar Releases
#Revsar Extension Sonarqube
#Revisar el despliegue en IIS Local puerto 5000.
```
