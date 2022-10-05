Desarrollo de Aplicaciones con ASP.NET Core MVC
Gilberto Bermúdez Garro.
gbermude@outlook.com

---------------------------------------------------------------------------------------------------
Laboratorio #2

Objetivo: Reconocer las diferentes opciones que ofrece Razor para encapsular la generación
de contenido al momento de elaborar vistas para ASP.NET Core MVC.

Con cada cambios que efectúe en el código fuente confirme la compilación y el funcionamiento
esperado del mismo, puede habilitar "Live Reload".

En el archivo WA4.png puede observar la representación final de la estructura de la solución
de este laboratorio.

---------------------------------------------------------------------------------------------------
Actividades

1. Crear una aplicación Web utilizando ASP.NET Core MVC. Se debe utilizar la plantilla 
   "ASP.NET Core Web App (Model-View-Controller)". 
   El nombre que se debe asignar a la aplicación y la solución es WA4.
   
2. Agregue al proyecto WA4 referencias a los proyectos Northwind.Store.Data y Northwind.Store.Model.
 
   Copie el contenido completo de los proyectos mencionados dentro de la solución WA4 
   (estos proyectos se crearon el laboratorio #1), y posteriormente agregar una referencia 
   de WA4 a los proyectos Northwind.Store.Data y Northwind.Store.Model.
 
3. Agregue al proyecto WA4 el paquete Microsoft.EntityFrameworkCore y Microsoft.EntityFrameworkCore.SqlServer. 
   
   Se busca modificar la aplicación WA4 para aprovechar el acceso a datos disponible por medio
   de los proyectos referenciados.

4. Se procederá con cambios sobre la clase Program. Se deben agregar los siguientes using.

	using Microsoft.EntityFrameworkCore;
	using Northwind.Store.Data;	
	
5. Agregar el uso de la clase de acceso a datos NWContext. Al código en el archivo Program.cs 
   se debe agregar la depencia antes del punto en donde se agregan la dependencias de MVC.

	var builder = WebApplication.CreateBuilder(args);
	ConfigurationManager configuration = builder.Configuration;

	// Add services to the container.
	builder.Services.AddDbContext<NWContext>(options => options.UseSqlServer(configuration.GetConnectionString("NW")));
	builder.Services.AddControllersWithViews();

	var app = builder.Build();	

6. Se debe modificar el archivo appsettings.json para incluir el connection string esperado por la clase NWContext y
   que los valores de configuración de la conexión a la base de datos.
   
   Confirme los valores de acuerdo a la instancia de base de datos de SQL Server que está utilizando en donde se encuentra
   la base de datos Northwind.
   
	{
	  "ConnectionStrings": {
		"NW": "Data Source=.\\sqlexpress;Database=Northwind;Integrated Security=SSPI;MultipleActiveResultSets=True;"
	  },
	  "Logging": {
		"LogLevel": {
		  "Default": "Information",
		  "Microsoft": "Warning",
		  "Microsoft.Hosting.Lifetime": "Information"
		}
	  },
	  "AllowedHosts": "*"
	}
   
7. Se debe modificar la vista de Index del controlador Home para mostrar la lista de todos los productos disponibles. 
   Se puede eliminar el HTML que contiene esa vista.
   
   Los grupos deberán de agruparse por medio de la categoría. Se deben ofrecer los datos de Name, QuantityPerUnit, 
   Price y Stock, para lo productos por cada categoría se debe reportar el número total de productos disponibles en 
   esa categoría.

   Considere el uso de un ViewModel. La siguiente es una alternativa.
   
	using Northwind.Store.Model;

	namespace WA4.ViewModels
	{
		public class HomeIndexViewModel
		{
		    public string Filter { get; set; } = "";
				
			public List<CategoryGroupViewModel> Groups { get; set; } = new List<CategoryGroupViewModel>();

			public class CategoryGroupViewModel
			{
				public string CategoryName { get; set; } = "";
				public List<Product> Products { get; set; } = new List<Product>();
			}
		}
	}
	   
   El siguiente HTML es un ejemplo de resultado final que podría ofrecer la vista.
   
   <div class="row">
    <div class="col-12">
   		<h2>Products</h2>

		<section>
			<h3>Beverages</h3>

			<table class="table table-bordered">
				<thead>
					<tr>
						<th>Name</th>
						<th>QuantityPerUnit</th>
						<th>Price</th>
						<th>Stock</th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>Chai X2</td>
						<td>10 boxes x 20 bags</td>
						<td>18,0000</td>
						<td>39</td>
					</tr>
					<tr>
						<td>Chang</td>
						<td>24 - 12 oz bottles</td>
						<td>19,0000</td>
						<td>17</td>
					</tr>
					....
				</tbody>					
				<tfoot>
					<tr>
						<td colspan="4"> Items<span class="badge"> 9</span>
						</td>
					</tr>
				</tfoot>
			</table>
		</section>
		...		
   
   Para incluir el uso de la clase de contexto de Entity Framework en el controlador de Home recuerde modificar el 
   constructor del controlador, así:
   
	   public class HomeController : Controller
	   {
	   ...
			private readonly NWContext _db;

			public HomeController(ILogger<HomeController> logger, NWContext db)
			{
				_logger = logger;
				this._db = db;
			}
		...
		
    Una posible versión de la consulta para agrupar los productos por Categoría puede ser:
	
		var q1 = from p in _db.Products.Include(p => p.Category).ToList()
				 group p by p.Category?.CategoryName ?? "Sin Categoría" into CategoryProducts
				 select new CategoryGroupViewModel()
				 {
					 CategoryName = CategoryProducts.Key,
					 Products = CategoryProducts.ToList()
				 };
   
8. Se debe modificar la vista de Index del controlador Home para filtrar la lista de productos
   por el nombre de producto. La búsqueda debe ser tipo LIKE sobre el campo ProductName. 
   
   Se debe modificar la vista para incluir un pequeño formulario que permita capturar el 
   valor que se ha de utilizar como filtro.
   
   Se debe mantener en el formulario de búsqueda el filtro utilizado.
   
   Este es un posible ejemplo del formulario.

	<form asp-action="Index">
		<div class="mb-3">
			<input asp-for="Filter" class="form-control" placeholder="Search filter ..." >
		</div>
		<button type="submit" class="btn btn-primary">Search</button>
	</form>
	
9. Existen diversas opciones que permiten en una vista de Razor encapsular el código responsable de la generación
   del HTML de forma que se permita la reutilización y la distribución de forma más sencilla.
   
   Entre las opciones disponibles se encuentran los: HTML Helpers, Tag Helpers, Partial Views y View Components.
   
   En adelante se buscará explorar estás opciones y las posibilidades que estás ofrecen, aplicando cada opción
   sobre la vista recientemente creada.
   
   Los HTML Helpers son básicamente métodos en una clase que retornan una hilera que se espera corresponda
   a código de HTML y que se puede incluir como parte del cuerpo del documento generado por la vista de Razor.

   Los Tag Helpers tienen como propósito el mismo que el HTML Helpers, con la particilaridad que su uso en una
   vista de Razor se hace incluyendo una etiqueta similar a las de HTML, así como existe <p> o <h1>, con 
   los Tag Helpers se puede tener una etiqueta <detalle-producto> cuya procesamiento de como resultado el
   detalle de un producto con una tabla de HTML.   
   
10. En la raíz del proyecto WA4 cree una nueva carpeta de nombre Helpers, y dentro de esta
   otra carpeta de nombre HTML. Agregar dentro de la carpeta Helpers\HTML la clase
   de nombre ProductHelper cuyo código fuente se adjunta a este laboratorio.
   
   Revise la clase ProductHelper, puede observar que básicamente cuenta con dos métodos estáticos
   que retornan un HtmlString, en realidad en su estado más básico eso es un HTML Helper, un método
   que retorna HTML que puede ser reutilizado por una vista de Razor.
   
   Para poder utilizar el HTML Helper vaya a la vista de Index, y agregue el siguiente using:
   
   @using WA4.Helpers.HTML
   
   Después como si se llamara cualquier otro método desde Razor sustituya el HTML que genera
   la línea de detalle del Producto, por el resultado de pasar la variable del Producto
   como parámetro, comente el código de Razor existente, de la siguiente manera:
   
	<tbody>
	@foreach (var p in g.Products)
	{
		@ProductHelper.ProductDetail(p)
	}
	</tbody>
	
   Ambos métodos ProductDetail y ProductDetail2 solo muestran don maneras posibles de generar
   el HTML en forma dinámica. Modifique el código anterior para ahora generar la línea de
   detalle de producto, pero utilizando el método ProductDetail2, de la siguiente manera:
   
	<tbody>
	@foreach (var p in g.Products)
	{
		@ProductHelper.ProductDetail2(p)
	}
	</tbody>
   
   Por medio del uso de métodos de extensión se puede incorporar el HTML Helper a la
   propiedad Html de Razor en donde se pueden encontrar varios de los HTML Helpers
   incluídos por defecto. Para hacerlo copie el archivo ProductExtensions incluído con
   este laboratorio en la carpeta Helpers\HTML, agregue el siguiente using en la
   vista Index:
   
   @using WA4.Helpers.HTML
   
   Esto incorpora el método ProductDetail a la propiedad Html. Modifique como genera
   el detalle del producto haciendo uso ahora del método de extensión de la siguiente
   manera:
   
	<tbody>
	@foreach (var p in c.Items)
	{
		@Html.ProductDetail(p)
	}
	</tbody>
   
11. ASP.NET Core MVC agrega otra forma de Helper conocido como Tag Helper, este permite
	lograr el mismo objetivo que con los HTML Helpers pero utilizando sintaxis muy 
	similar a la utilizada en HTML.
	
	Para probar esta forma de crear Helpers agregue una nueva carpeta en Helpers\Tag.
	Copie el archivo ProductTagHelper.cs incluido con este laboratorio en la carpeta 
	Helpers\Tag.
	
	Modifique la vista de Views\_ViewsImports para incorporar el uso del nuevo Tag
	Helper, de la siguiente manera:
	
	@using WA4
	@using WA4.Models
	@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
	@addTagHelper *, WA4
	
	Modifique la vista Index, para cambiar la manera en como se genera el detalle
	del producto de la siguiente manera:
	
	<tbody>
	@foreach (var p in c.Items)
	{
		<product data="p" />
	}
	</tbody>
	
	Revise el código fuente en el archivo Helpers\Tag\ProductTagHelper.cs, este 
	muestra la primera forma de crear un Tag Helper, en este caso basado en 
	convenciones:
	
		- Nombre de Tag Helper: Prefijo de nombre terminado en TagHelper y que hereda 
		de la clase TagHelper, así: <nombre>TagHelper : TagHelper.
		
		- Atributos del Tag Helper: Propiedades públicas de la clase. En el ejemplo:
		public Product Data { get; set; }
	
	Observe su correspondencia con la etiqueta: <product data="p" />
	
	También es posible crear Tag Helpers sin utilizar la convención, y en su
	lugar aplicar anotaciones (atributos) para definir la correspondencia con los
	elementos de un Tag Helper.
	
	Copie el archivo ProductRow.cs incluido con este laboratorio en la carpeta 
	Helpers\Tag. Con este se crea un Tag Helper que agrega comportamiento a la 
	etiqueta estandar de HTML <tr> (table row), para que del lado del servidor
	pueda agregarsele contenido de forma dinámica, considerar que el contenedor
	original se mantiene.
	
	Para aplicar este Tag Helper modifique la generación del detalle del 
	producto en la vista Index de la siguiente manera:
	
	<tbody>
	@foreach (var p in c.Items)
	{
		<tr product="p"></tr>
	}
	</tbody>
	
	Revise el código fuente en Helpers\Tag\ProductRow.cs y como este se
	relaciona con su uso final: <tr product="p"></tr>, note el uso
	de los atributos: HtmlTargetElement, HtmlAttributeName.
	
	En caso necesario es posible combinar la dos formas de crear Tag Helpers
	por convención o de forma explícita.
	
12. Otra forma de encapsular contenido para una vista de Razor es por medio
	de View Components, estos permiten reutilizar conceptos de vistas de Razor
	pero permitiendo agregar comportamiento o código relacionado a esa vista,
	además los View Components pueden utilizarse como resultado de ejecutar
	la acción de un controlador.
	
	Para poner a prueba este otro recurso cree una carpeta en la raíz
	del proyecto WA4 con el nombre ViewComponents, esta es la carpeta
	por defecto. Copie el archivo ProductViewComponent.cs incluido como 
	parte de este laboratorio en la carpeta ViewComponents.
	
	Puede observar el código fuente de ProductViewComponent.cs, existe
	un método InvokeAsync que retorna una vista, y además puede de forma
	opcional recibir parámetros, en este ejemplo InvokeAsync(int id)
	recibe un id de tipo entero. El método InvokeAsync puede retornar
	la vista por defecto "return View()" o una vista específica 
	"return View("Links")" y igual que cuando una acción retorna a la 
	vista se le puede asignar un modelo.
	
	¿Dónde va un ViewComponent a localizar sus vistas relacionadas?
	Si se retorna "return View()" se busca a una vista en varios lugares,
	primero en la carpeta Views/<Nombre de Controlador>/Components/
	<Nombre de Component>, en el caso del código de ejemplo, por 
	convención es nombre que aparece antes de ViewComponent, así 
	<Nombre de Component>ViewComponent. Para el ejemplo, se busca
	primero en: Views/Home/Components/Product/Default.cshtml;  
	en segundo lugar se buscaría en Views/Shared/Components/Product/Default.cshtml.
	
	Para continuar con la prueba de este nuevo concepto copie el archivo
	Default.cshtml incluído por parte de este laboratorio en la carpeta
	Views/Home/Components/Product.

	Modifique ahora la vista Index para que la generación del 
	Producto se haga utilizando un ViewComponent, puede hacerlo
	de la siguiente forma:
	
	<tbody>
	@foreach (var p in c.Items)
	{
		@await Component.InvokeAsync("Product", new { id = p.ProductId })
	}
	</tbody>
	
	Como puede ver para incorporar el ViewComponent fué necesario
	utilizar el método @await Component.InvokeAsync, y como se mencionaba
	anteriormente es opcional pasar parámetros, que si se hace sería
	por medio de un objeto anónimo, como en el caso del ejemplo.
	
	Vamos a modificar nuestro ViewComponent para demostrar como utilizar
	una vista específica, en este caso proceda a modificar el método 
	InvokeAsync de la clase ProductViewComponent.cs de la siguiente
	manera: 
	
	public async Task<IViewComponentResult> InvokeAsync(int id)
	{
		var data = await db.Products.
			SingleOrDefaultAsync(p => p.ProductId == id).ConfigureAwait(false);

		// La vista por defecto es Default.cshtml
		//return View(data);
		return View("Links", data);
	}
	
	De esta manera se define de forma explícita que se utilice la vista
	de ViewComponent Links, para ello copie el archivo Links.cshml que
	se incluye como parte de este laboratorio en la carpeta
	Views/Home/Components/Product. Repita la ejecución de la aplicación
	notará que ahora se obtiene el resultado muy parecido solo que
	una columna ofrece un enlace para obtener el detalle del producto
	(este en será implementado en este laboratorio).
	
	Con los ViewComponents se puede agregar código previo a obtener
	la vista asociada.

13. Otro recurso para encapsular contenido de Razor es por medio de 
	Partial Views. Las Partial Views permiten encapsular Razor en una
	vista que puede ser reutilizada en otras vistas, de hecho casi
	cualquier vista podría ser reutilizada como Partial View al fin 
	de cuentas esta escrita en Razor.
	
	Para poner a prueba es concepto copie el archivo ProductPartial.cshtml
	que se adjunta como este laboratorio en la ruta Views/Home.
	
	Las Partial Views a similitud que un ViewComponent también
	puede ser retornada desde una acción de un Controller.
	
	Las Partial Views al estar contenidas dentro de otra vista tienen
	la posibilidad de tener acceso a la propiedades ViewBag, ViewData
	y Model de la vista ("padre") que las contiene, o también
	se le puede asignar un modelo completamente personalizado.
	
	En ejemplo que se trabaja en este ejercicio se asigna un modelo	personalizado. Modifique 
	la vista Index para mostrar el detalle del producto utilizando una Partial View, puede 
	hacerlo	de la siguiente forma:
	
	<tbody>
	@foreach (var p in c.Items)
	{
		<partial name="ProductPartial" model="p" />
		await Html.RenderPartialAsync("ProductPartial", p);			
		@await Html.PartialAsync("ProductPartial", p)
	}
	</tbody>
	
	Las tres líneas hacen lo mismo, puede probar todas a la vez o solo una, tenga en cuenta 
	que una Partial View no requiere que siempre se le pase un modelo, como se comentaba antes, 
	sino se le asigna un modelo explícito la Partial View puede usar su propiedad "Model" 
	para tener acceso al modelo de la vista	que la contiene.
	
	¿Cuál de esas tres formas de incluir una Partial View es mejor?
	
	Las líneas:
	
		<partial name="ProductPartial" model="p" />
		await Html.RenderPartialAsync("ProductPartial", p);	
	
	hacen lo mismo, la primera utiliza el Tag Helper <partial> y la
	segundo un HTML Helper, en general se recomienda usar una de esta formas; en el caso de 
	la línea:
	
		@await Html.PartialAsync("ProductPartial", p)
	
	aunque se obtiene el mismo resultado, esta forma de incluir la Partial View también permite 
	obtener el resultado de la generación de la Partial View y en caso de se necesario manipular 
	el contenido previo a incorporarse al resultado de la generación de la vista.

14. En al ASP.NET MVC que existe en el .NET Framework existen HTML Helpers específicos
	para facilitan la implementación de AJAX, específicamente la generación de solicitudes
	de HTTP de forma asincrónica, en ASP.NET Core MVC esos HTML Helpers no se incluyen.
	
	Esa funcionalidad se sustenta en el uso del plugin de jQuery jquery.unobtrusive-ajax,
	que a su vez utiliza jQuery para emitir la solicitudes de HTTP. No es muy complicado
	revisando la documentación del plugin jquery.unobtrusive-ajax incorporar la funcionalidad
	manualmente si utilizar un HTML Helper.
	
	Como un ejemplo de como crear un Tag Helper que encapsula funcionalidad basada en código
	de JavaScript con esta laboratorio se incluye un ejemplo haciendo siempre uso del plugin
	jquery.unobtrusive-ajax, tener claro que lo que hace este plugin actualmente podría ser 
	perfectamente implementado solamente utilizando JavaScript estándar.
	
	Copie el archivo RequestExtensions.cs incluido con este laboratorio en la carpeta
	Extensions (puede ser necesario crear la carpeta), con este se agrega un método de extensión
	a la clase HttpRequest para determinar si un Request fué emitido de forma asincrónica
	revisando si el Request incluye el encabezado X-Requested-With, este encabezado solo
	lo envían algunas bibliotecas (tener en cuenta). Para poder utilizar le extensión en el
	Controller de Home se debe agregar a este Controller el using:
	
		using WA4.Extensions;
	
	Agregue los dos siguientes métodos a las acciones del Controller de Home. Ambas acciones
	sirven como ejemplo para observar como se puede utilizar una Partial View o un ViewComponent
	como resulta de la ejecución una acción de un Controller.
	
	    public IActionResult IndexPartial(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return PartialView("ProductPartial", db.Products.Where(p => p.ProductId == id).SingleOrDefault());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult IndexViewComponent(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return ViewComponent("Product", new { id });
            }
            else
            {
                return NotFound();
            }
        }
		
	Copiar el archivo AjaxTagHelper.cs incluido con este laboratorio en la carpeta
	Helpers/Tag. Si revisa el código fuente del archivo verá que básicamente se
	busca mapear lo que ofrece el plugin jquery.unobtrusive-ajax a un Tag Helper, queda
	claro que es perfectamente posible solo haber colocado las anotaciones del plugin en
	la etiqueta <a>.
	
	Copiar el archivo libman.json incluido con este laboratorio en la carpeta raíz
	del proyecto WA. Este agrega el plugin jquery.unobtrusive-ajax de jQuery, que 
	permite por medio de anotaciones que haciendo clic en un elemento del DOM se
	puedan emitir solicitudes de HTTP.
	
	Copie el siguiente bloque de Razor al inicio de la vista Index. No copie las líneas
	que dicen "ESTE CÓDIGO YA EXISTE, NO COPIE ESTO" pero uselo para determinar la posición
	en el código de Razor que ya tiene.
	
	ESTE CÓDIGO YA EXISTE, NO COPIE ESTO @{
    ESTE CÓDIGO YA EXISTE, NO COPIE ESTO 	ViewData["Title"] = "Home Page";
	ESTE CÓDIGO YA EXISTE, NO COPIE ESTO }
	
	<div class="row">
		<div class="col-4">
			<a class="btn btn-outline-primary"
			   ajax="true" ajax-url="/Home/IndexPartial/1" ajax-target="#demo" ajax-mode="after">
				Get Detail
			</a>
		</div>
		<div class="col-8">
			<form class="form-inline" ajax="true" ajax-method="post" 
				  ajax-url="/Home/IndexViewComponent" ajax-target="#demo" 
				  ajax-failure="productAjaxFailure">
				<div class="form-group">
					<label>Product ID</label>
					<input type="text" class="form-control" name="id" placeholder="Type product ID">
				</div>
				<button type="submit" class="btn btn-primary">Get Detail</button>
			</form>
			<script>
				function productAjaxFailure(xhr) {
					if (xhr.status === 404) {
						alert('Product not found!');
					}
				}
			</script>
		</div>
	</div>
	<table class="table table-bordered table-striped">
		<tbody id="demo"></tbody>
	</table>

	@section scripts {
		<environment names="Development">
			<script src="~/lib/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.js"></script>
		</environment>
		<environment names="Production">
			<script src="~/lib/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.min.js"></script>
		</environment>
	}

	<hr />	
	
	ESTE CÓDIGO YA EXISTE, NO COPIE ESTO <div class="row">
    ESTE CÓDIGO YA EXISTE, NO COPIE ESTO 	<div class="col-12">
    ESTE CÓDIGO YA EXISTE, NO COPIE ESTO	    <h2>Products</h2>
	
	Observe que las etiquetas <a> y <form> tienen ahora la propiedad ajax="true" de acuerdo
	al Tag Helper AjaxTagHelper esto le permite hacer cambios en la etiqueta correspondiente
	para que cuando el código llegue al navegador el plugin pueda proveer la funcionalidad
	AJAX que se espera.
	
	Pruebe la ejecución al hacer clic en la etiqueta <a>Get Detail</a>, esta emite una solicitud
	a la accion /Home/IndexPartial/1, puede colocar un "Break Point" en esta acción para confirmar
	la ejecución, en este caso el resultado se genera por medio de una Partial View, el resultado
	una vez que es recibido en el navegador se agrega a una tabla HTML.
	
	Pruebe la ejecución al hacer clic en la etiqueta <button>Get Detail</button>, esta emite una 
	solicitud a la accion /Home//IndexViewComponent, en este caso el ID se envía a partir de un
	valor captura en un pequeño formulario, puede colocar un "Break Point" en esta acción para 
	confirmar la ejecución, en este caso el resultado se genera por medio de un ViewComponent, 
	el resultado una vez que es recibido en el navegador se sustituye en una tabla HTML.
	
	Lo anterior es ejemplo de un patrón muy común implementado con AJAX llamado Partial Rendering,
	como puede notar se actualiza el contenido del HTML, pero no se un refresco completo del 
	contenido de todo el documento HTML.
	
	Puede encontrar documentación sobre en plugin en https://github.com/aspnet/jquery-ajax-unobtrusive.
	Recuerde este comportamiento se puede lograr con otras bibliotecas, o inclusive manualmente
	solamente utilizando JavaScript.
	
	Del código que copió anteriormente ponga atención a la siguiente porción:
	
	<environment names="Development">
		<script src="~/lib/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.js"></script>
	</environment>
	
	Este es el Tag Helper <environment> este permite incluie de forma condicional contenido
	dentro de la vista de Razor considerando los valores posibles de la variable ASPNETCORE_ENVIRONMENT,
	esta puede estar definida a nivel de: sesión, usuario, o equipo; y generalmente los valores 
	que tiene son: Development o Production, pero en realidad puede ser cualquier valor, esos dos
	son comprendidos por ASP.NET Core. En el caso del ejemplo básicamente lo que dice es que
	si la variable ASPNETCORE_ENVIRONMENT=Development entonces se incluirá la etiqueta:
	
	<script src="~/lib/jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax.js"></script>
	
	si la variable no tiene ese valor la etiqueta no es incluida, el Tag Helper <environment> 
	es muy útil para determinar de acuerdo al ambiente que contenido se desea inyectar en la
	vista de Razor, recuerde que puede ser cualquier porción de los soportado en una vista
	de de Razor. En tiempo de desarrollo el valor de la varible ASPNETCORE_ENVIRONMENT se puede
	cambiar en las Properties del proyecto WA4, haga la prueba cambiando el valor de la variable
	entre Development o Production y vea en la sección de Network de las herramientas de desarrollo
	del navegador como se ve reflejado el cambio, como se carga el código de JavaScript sin 
	minificar y luego minificado.

--- ÚLTIMA LÍNEA ---