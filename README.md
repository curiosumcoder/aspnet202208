# ASP.NET Core MVC
Material relacionado con el curso de ASP.NET Core MVC impartido en UCenfotec (modalidad virtual) en Setiembre 2021.

### Requerimientos de software
1.	Requisitos mínimos del hardware que ocupamos. 
	https://docs.microsoft.com/en-us/visualstudio/releases/2022/system-requirements
	
2.	Última versión del Microsoft .NET Core SDK
	https://dotnet.microsoft.com/en-us/download,  el de 64bits aquí, 
	https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.400-windows-x64-installer
	
	Utilizar el Windows Installer de acuerdo con la versión de Windows que se esté utilizando.
	Se hace efectuar una ejecución inicial para descargar los paquetes iniciales. 
	Estos pasos crean una pequeña aplicación de línea de comandos que imprime la palabra “Hello World” en la consola. 
	
	Ejecutar desde la línea de comandos: 
	
	mkdir t1
	cd t1
	dotnet new console 

	En este paso puede aparecer un mensaje que señala que se están descargando los paquetes iniciales de .NET Core. 
	Esperar a que se complete la descarga.
		
	dotnet build
	dotnet run

3.	Última versión del .NET Core Hosting Bundle 
	https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-6.0.8-windows-hosting-bundle-installer

4.	Microsoft Visual Studio Code 
	https://code.visualstudio.com/
	Instalar o actualizar a la última versión.
	
5.	Microsoft SQL Server 2008 R2 o superior. 
	https://www.microsoft.com/en-us/sql-server/sql-server-downloads
	Se acostumbra a utilizar la edición Express, en SQL Server 2017 para desarrollo es posible utilizar la edición Developer.	
	
6.	Microsoft Visual Studio 2022 (edición Community o superior) 
	https://visualstudio.microsoft.com/downloads/
	https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022#install-workloads
	
	Aquí se documenta como obtener los instaladores para la instalación local, 
	https://docs.microsoft.com/en-us/visualstudio/install/create-a-network-installation-of-visual-studio?view=vs-2022, 
	esto baja todos los “workloads” pero al momento de instalar no se deben de instalar todos.
	
	Si el Visual Studio 2022 ya se encuentra instalado se puede utilizar el Visual Studio Installer, 
	para efectuar la actualización.

	Se deben instalar al menos los “workloads”: 
	- Web & Cloud 
		+ ASP.NET and web development
		+ Data storage and processing
		
	En caso de contar con una instalación del Visual Studio 2022, proceder con la actualización a la última versión, 
	y confirmar que se tengan instalados los “workloads” señalados en el punto anterior. Esto se hace ejecutando el 
	Visual Studio Installer, y aplicar en el equipo la actualización cuando aparece el botón “Update”, es solo de 
	aplicarlo y esperar que finalice.
 
	Se puede confirmar el resultado con el “Acerca de” de Visual Studio 2022.
	
7.	Internet Information Services habilitado 
	https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2008-R2-and-2008/cc731911(v=ws.11)?redirectedfrom=MSDN
	
8.	Web Deploy 3.6 
	https://www.iis.net/downloads/microsoft/web-deploy.  El enlace del instalador se encuentra en la parte inferior 
	de la página.
	
9.	Navegadores Web actualizados a la última versión. Cualquiera de los siguientes puede ser utilizado.
	https://www.mozilla.org/en-US/firefox/
	https://www.google.com/chrome/index.html
	https://www.microsoft.com/en-us/edge?r=1

10. Postman
	https://www.postman.com/downloads/
	
11. EF Core Power Tools (Extensión de Visual Studio 2022)
    https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools
	
De ser posible efectuar la instalación de las versiones del software con el idioma inglés, para unificar con la 
configuración utilizada por el profesor.

### Bases de datos de ejemplo
* https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs
