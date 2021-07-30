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
![Esquema](https://user-images.githubusercontent.com/1031887/126097194-d9148b5f-4854-471b-b374-b328f97c4822.png)

##  -------  Git and manage Code  -------

```
#Crear directorio base de los proyectos y vincular cambios con github
mkdir Company.App.Back
cd Company.App.Back
echo # Company.App.Back >> README.md
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

#open browser: http://localhost:5000/weatherforecast
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
```
![3 Agents](https://user-images.githubusercontent.com/1031887/127592369-545251d2-2908-40f9-8917-72550af66632.PNG)
```
#Acceder a https://dev.azure.com/{username}/_settings/deploymentpools y crear un Pool de despliegue on-premise llamado IIS-Server
```
![3 DeploymentGroup](https://user-images.githubusercontent.com/1031887/127592393-59a2daef-1588-434b-80bb-e45843d1f489.PNG)
```
#Acceder a https://dev.azure.com/{username}/DevOps/_settings/adminservices y crear una conexión hacia sonarqube server
```
![3 ServiceConnection](https://user-images.githubusercontent.com/1031887/127592788-1c501b2b-928b-4798-9ad7-e483d8dbec8f.PNG)
```
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
  artifactName: 'Company.App.Back'
  buildConfiguration: 'Release'
  connectionKey: 'Company.App.Back'

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
![1 Summary](https://user-images.githubusercontent.com/1031887/127592354-452c14fe-f1a2-4852-b1cd-0b4ef42386b1.PNG)
![2 Test](https://user-images.githubusercontent.com/1031887/127592364-f71c8398-e384-4ed2-861a-d4b38a42b850.PNG)
![3 Release](https://user-images.githubusercontent.com/1031887/127592407-baa18d83-7284-4675-93bc-b26aaf0f19a5.PNG)
![4 SonarService](https://user-images.githubusercontent.com/1031887/127592415-100c21e4-0cdc-423e-90f1-08798aa27ec9.PNG)
![5 Sonarqube](https://user-images.githubusercontent.com/1031887/127592422-139a525a-b79a-4bf7-bf93-a9253873fd1f.PNG)

##  -------  Continuous Delivery on Premise  -------
```
#Acceder a https://dev.azure.com/{username}/_settings/deploymentpools y crear un Pool de despliegue on-premise llamado IIS-Server
#Acceder a https://dev.azure.com/{username}/DevOps/_release y crear un pipeline
#Seleccionar IIS website deployment y colocarle de nombre "dev"
#Elegir tab "tasks"
Sección dev:
	WebSiteName: Company.App.Back
	Binding Port: 3002
Sección IIS Deployment
	Deployment Group: IIS Server
IIS Web Ap Manage
	Physical path: %SystemDrive%\inetpub\wwwroot\Company.App.Back
	Create or update app pool: check
	Application Pool:
		Name: Company.App.Back
		.NET version: No manage code
IIS Web App Deploy
	Remove Additional Files at Destination: check
	
#Renombrar a Company.App.Back
#Asignar Artifact. 
	Project: DevOps
	Source: {username}.Company.App.Back
	Finalmente agregar.
#Seleccionar artefacto y 
	Continuous deployment trigger: activado
```
![1 Deploy](https://user-images.githubusercontent.com/1031887/127592429-5e6767a0-3749-405c-9562-fe507bae2072.PNG)


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
#Revisar el despliegue en IIS Local puerto 3002.
```
![7 IISBack](https://user-images.githubusercontent.com/1031887/127593339-11002288-7e3c-4031-8a82-b5b6573aa280.PNG)
![7 IISBack2](https://user-images.githubusercontent.com/1031887/127593524-8c469d59-5600-475c-933b-c4682ba0abe1.PNG)
