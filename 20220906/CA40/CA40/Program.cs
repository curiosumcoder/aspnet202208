using System;
using CA40.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace CA40
{
    class Program
    {
        static void Main(string[] args)
        {
            string lectura = "";
            do
            {
                Console.WriteLine("Seleccione una opción:\n1. Basic\n2. CRUD\n3. LINQ To Entities\n4. Lazy Loading\n5. Eager Loading\n6. LINQ Dynamic\n7. LINQ To XML\n8. PLINQ\n9. Transactions\n10. Raw SQL\n11. Store Procedures\n12. Raw SQL Client (ADO.NET)\n");

                lectura = Console.ReadLine();

                if (!string.IsNullOrEmpty(lectura))
                {
                    int opcion = Convert.ToInt32(lectura);
                    Console.Clear();

                    switch (opcion)
                    {
                        case 1:
                            Console.WriteLine("Basic");
                            Basic();
                            break;
                        case 2:
                            Console.WriteLine("CRUD");
                            CRUD();
                            break;
                        case 3:
                            Console.WriteLine("LINQToEntities");
                            LINQToEntities();
                            break;
                        case 4:
                            Console.WriteLine("LazyLoading");
                            LazyLoading();
                            break;
                        case 5:
                            Console.WriteLine("EagerLoading");
                            EagerLoading();
                            break;
                        case 6:
                            Console.WriteLine("LINQDynamic");
                            LINQDynamic();
                            break;
                        case 7:
                            Console.WriteLine("LINQToXML");
                            LINQToXML();
                            break;
                        case 8:
                            Console.WriteLine("PLINQ");
                            PLINQ();
                            break;
                        case 9:
                            Console.WriteLine("Transactions");
                            Transactions();
                            break;
                        case 10:
                            Console.WriteLine("RawSQL");
                            RawSQL();
                            break;
                        case 11:
                            Console.WriteLine("StoreProcedures");
                            StoreProcedures();
                            break;
                        case 12:
                            Console.WriteLine("RawSQLClient");
                            RawSQLClient();
                            break;
                    }

                    Console.WriteLine("\n¡Listo!");
                    Console.ReadKey();
                    Console.Clear();
                }

            } while (!string.IsNullOrEmpty(lectura));
        }

        #region Demostraciones
        static string LeerConfiguracion()
        {
            // Soporte de archivo de configuración local
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var connStr = configuration.GetConnectionString("NW");

            Console.WriteLine(connStr);
            return connStr;
        }

        private static void Basic()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<NWContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("NW"));

            //using (var db = new NWContext())
            using (var db = new NWContext(optionsBuilder.Options))
            {
                // query
                // IQueryable<Product>
                // IEnumerable<Product> - materializado
                var q0 = from p in db.Products
                         where p.ProductName.Contains("queso")
                         select p;

                var q1 = (from p in db.Products
                          where p.ProductName.Contains("queso")
                          select p).All(p => p.Discontinued);

                // métodos de extensión + expresiones lambda
                var query = db.Products.Where(p => p.ProductName.Contains("queso"));

                bool discontinued = db.Products.All(p => p.Discontinued);

                //foreach (var p in db.Products)
                foreach (var p in query)
                {
                    Console.WriteLine($"{p.ProductId} {p.ProductName}");
                }
            }
        }

        private static void CRUD()
        {
            //using (var ts = new TransactionScope())
            using (var db = new NWContext())
            {
                int lastId = db.Products.Max(p => p.ProductId);

                var np = new Product()
                {
                    ProductName = $"Demostración #{++lastId}",
                    Discontinued = false
                };

                db.Products.Add(np);
                db.SaveChanges();

                np.ProductName += " ACTUALIZADO";

                foreach (var e in db.ChangeTracker.Entries<Product>())
                {
                    Console.WriteLine($"{e.Property("ProductName").CurrentValue} {e.State}");
                }

                db.SaveChanges();

                //db.Remove(np);
                db.Products.Remove(np);
                db.SaveChanges();

                foreach (var p in db.Products.Include(p => p.Category))
                {
                    Console.WriteLine($"{p.ProductId}, {p.ProductName}, {p.UnitPrice}, {p.Category?.CategoryName ?? "Sin categoría"}");
                }

                Console.WriteLine($"Total de productos: {db.Products.Count()}");
                Console.WriteLine($"Total de productos, inventario > 100: {db.Products.Count(p => p.UnitsInStock > 100)}");
                Console.WriteLine($"Total de productos, proveedores de Québec: {db.Products.Count(p => p.Supplier.Region == "Québec")}");

                //ts.Complete();

                Console.ReadLine();
            }
        }

        static void LINQToEntities()
        {
            // * Filtro
            using (NWContext db = new NWContext())
            {
                var q1 = from c in db.Customers
                         where c.Country == "Mexico"
                         select c;

                var q1x = db.Customers.
                    Where(c => c.Country == "Mexico");

                var result = q1.Count();
                var lista = q1.ToList();
                var any = q1.Any();
                var any2 = db.Customers.Any(c => c.Country == "Mexico");

                foreach (var item in q1x)
                {
                    Console.WriteLine($"{item.CustomerId} {item.CompanyName} {item.ContactName} {item.Country}");
                }
            }

            // * Proyecciones
            using (NWContext db = new NWContext())
            {
                var q2 = from c in db.Customers
                         select c.Country;
                var q2x = db.Customers.Select(c => c.Country);

                var q2y = from c in db.Customers
                          select new { c.CustomerId, c.ContactName };

                var q2z = db.Customers.Select(c =>
                    new { Id = c.CustomerId, c.ContactName });

                var q2w = db.Customers.Select(c =>
                    new Category() { CategoryName = c.ContactName });

                Console.Clear();
                foreach (var item in q2z)
                {
                    Console.WriteLine($"{item.Id}, {item.ContactName}");
                }
            }

            // SelectMany
            using (NWContext db = new NWContext())
            {
                var q4 = db.Customers.
                    Where(c => c.Country == "Mexico").
                    SelectMany(c => c.Orders);

                //foreach (var c in q4)
                //{
                //    foreach (var o in c)
                //    {

                //    }
                //}

                var q4x = db.Orders.
                    Where(o => o.Customer.Country == "Mexico");

                Console.Clear();
                foreach (var item in q4)
                {
                    Console.WriteLine($"{item.CustomerId}, {item.OrderId}");
                }
            }

            // * Ordenamiento
            using (NWContext db = new NWContext())
            {
                var q5 = from c in db.Customers
                         where c.Orders.Count > 5
                         orderby c.Country descending
                         select c;

                var q5x = db.Customers.
                    Where(c => c.Orders.Count > 5).
                    OrderByDescending(c => c.Country);

                Console.Clear();
                foreach (var item in q5)
                {
                    Console.WriteLine($"{item.CompanyName}, {item.Country}");
                }

                var q6 = from c in db.Customers
                         orderby c.CompanyName, c.ContactTitle, c.ContactName
                         select c;
                var t0 = q6.First();

                //var q6x1 = db.Customers.OrderBy(c => new { c.CompanyName, c.ContactTitle });

                var q6x = db.Customers.OrderBy(c => c.CompanyName).OrderBy(c => c.ContactTitle ).ThenBy(c => c.ContactName);
                var t1 = q6x.First();

                Console.Clear();
                foreach (var item in q6x)
                {
                    Console.WriteLine($"{item.CompanyName}, {item.Country}");
                }
            }

            // Agrupamiento
            using (NWContext db = new NWContext())
            {
                var q7 = from c in db.Customers.ToList()
                         where c.Orders.Count() > 5
                         group c by c.Country into CustByCountry
                         select CustByCountry;

                var q7x = db.Customers.GroupBy(c => c.Country);

                Console.Clear();
                foreach (var grupo in q7)
                {
                    Console.WriteLine($"{grupo.Key}, {grupo.Count()}");

                    foreach (var c in grupo)
                    {
                        Console.WriteLine($"\t{c.ContactName}");
                    }
                }

                var q7y = from c in db.Customers
                          group c by new { c.Country, c.City } into CountryCity
                          where CountryCity.Count() > 1
                          select new
                          {
                              Country = CountryCity.Key.Country,
                              City = CountryCity.Key.City,
                              Count = CountryCity.Count(),
                              Items = CountryCity
                          };

                var q7y2 = db.Customers.ToList().GroupBy(c => new { c.Country, c.City }).
                    Where(g => g.Count() > 1).
                    Select(g => new
                    {
                        Country = g.Key.Country,
                        City = g.Key.City,
                        Count = g.Count(),
                        Items = g
                    });

                Console.Clear();
                foreach (var item in q7y2)
                {
                    Console.WriteLine($"{item.Country}, {item.City}, {item.Count}");

                    foreach (var c in item.Items)
                    {
                        Console.WriteLine($"\t{c.ContactName}");
                    }
                }
            }

            // Join
            using (NWContext db = new NWContext())
            {
                var q8 = from c in db.Customers
                         join o in db.Orders on c.CustomerId
                         equals o.CustomerId
                         select new { c, o };

                //                new { c.CustomerID, c.Country }
                //equals new { o.CustomerID, Country =  o.ShipCountry }

                var q8x = db.Customers.Join(
                    db.Orders, c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o });

                Console.Clear();
                foreach (var item in q8)
                {
                    Console.WriteLine($"{item.c.CustomerId}, {item.o.OrderId}");
                }

                // Join agrupado
                var q8y = from c in db.Customers
                          join o in db.Orders on c.CustomerId
                          equals o.CustomerId into CustomerOrders
                          select new { c, Orders = CustomerOrders };
                //select CustomerOrders;

                //foreach (var ordenes in q8y)
                //{
                //    //foreach (var orden in ordenes)
                //    //{

                //    //}
                //}

                // Left Ourter Join
                var q8z = from c in db.Customers
                          join o in db.Orders on c.CustomerId
                          equals o.CustomerId into CustomerOrders
                          from detalle in CustomerOrders.DefaultIfEmpty()
                          select new
                          {
                              Customer = c,
                              Order = detalle
                          };

                foreach (var item in q8z)
                {
                    if (item.Order == null)
                    {
                        Console.WriteLine($"Customer {item.Customer.CustomerId} with NO orders!");
                    }
                }
            }

            // Conjuntos
            using (NWContext db = new NWContext())
            {
                var q9 = db.Customers.
                    Select(c => c.Country).Distinct();

                var q10 = db.Customers.Except(
                    db.Customers.Where(
                    c => c.Country == "Mexico")).
                    Select(c => c.Country).Distinct();

                var q11 = db.Customers.Where(c => c.Country == "Italy").Union(db.Customers.Where(c => c.Country == "Germany"));

                Console.Clear();
                foreach (var item in q10)
                {
                    Console.WriteLine($"{item}");
                }
            }

            // * Partición (paginación)
            using (NWContext db = new NWContext())
            {
                var q10 = db.Customers.OrderBy(c => c.CustomerId).Take(5);
                var q100 = db.Customers.OrderBy(c => c.CustomerId).Skip(5);
                var q101 = db.Customers.OrderBy(c => c.CustomerId).Skip(5).Take(5);

                var q11 = db.Customers.
                    OrderBy(c => c.CustomerId).
                    Skip(10);
                // Tomar los primero 10 elementos
                var q12 = db.Customers.
                    OrderBy(c => c.CustomerId).
                    Take(10);
                // Segunda página de 10 elementos
                var q13 = db.Customers.
                    OrderBy(c => c.CustomerId).
                    Skip(10).Take(10);

                Console.Clear();
                foreach (var item in q13)
                {
                    Console.WriteLine($"{item.CustomerId}, {item.CompanyName}");
                }
            }

            // * Modificación de consulta
            using (NWContext db = new NWContext())
            {
                var q14 = db.Customers.
                    Where(c => c.Orders.Count > 5);

                Console.Clear();
                Console.WriteLine(q14.Count());

                q14 = q14.Where(c => c.Country == "Mexico");
                Console.WriteLine(q14.Count());

                q14 = q14.OrderByDescending(c => c.ContactName);

                foreach (var item in q14)
                {
                    Console.WriteLine(item.ContactName);
                }
            }

            // Métodos útiles
            using (NWContext db = new NWContext())
            {
                var o1 = db.Customers.First();
                o1 = db.Customers.FirstOrDefault();
                o1 = db.Customers.Where(c => c.CustomerId == "ALFKI")
                    .Single();
                o1 = db.Customers.Single(c => c.CustomerId == "ALFKI");
                o1 = db.Customers.Where(c => c.CustomerId == "ALFKI").
                    SingleOrDefault();

                var o2 = db.Customers.All(c => c.Orders.Count > 5 &&
                        c.Country == "Mexico");
                o2 = db.Customers.Any(c => c.Orders.Count > 5);

                var sum = db.OrderDetails.
                    Sum(od => od.Quantity * od.UnitPrice);
                var sum1 = db.OrderDetails.
                   Average(od => od.Quantity * od.UnitPrice);
                var sum2 = db.OrderDetails.
                   Max(od => od.Quantity * od.UnitPrice);
            }
        }
        static void LazyLoading()
        {
            // Lazy Loading
            using (NWContext db = new NWContext())
            {
                //db.ChangeTracker.LazyLoadingEnabled = false;
                //bool isLazy = true;
                //db.ChangeTracker.LazyLoadingEnabled = isLazy; // default = true

                // .Include(c=>c.Orders). // Eager Loading
                var customers = db.Customers.
                    OrderBy(c => c.CustomerId);

                foreach (var c in customers.ToList())
                {
                    Console.WriteLine($"{c.CustomerId}, {c.ContactName}");

                    //if (!isLazy)
                    //{
                    //    db.Entry(c).Collection(o => o.Orders).Load();
                    //}

                    Console.WriteLine($"Número de Órdenes: {c.Orders.Count}");
                }

                foreach (var o in db.Orders)
                {
                    //if (!isLazy)
                    //{
                    //    db.Entry(o).Reference(c => c.Customer).Load();
                    //}

                    Console.WriteLine(o.Customer.ContactName);
                    Console.WriteLine($"{o.OrderId}, {o.OrderDate}");
                }
            }
        }
        static void EagerLoading()
        {
            // Eager Loading
            using (NWContext db = new NWContext())
            {
                // Proyección
                var customersOrders =
                    from c in db.Customers.
                        OrderBy(c => c.CustomerId).
                        Skip(10).Take(2)
                    select new
                    {
                        Cliente = c,
                        Ordenes = c.Orders
                    };

                foreach (var c in customersOrders)
                {
                    Console.WriteLine($"{c.Cliente.CustomerId}, {c.Cliente.ContactName}");
                    foreach (var o in c.Ordenes)
                    {
                        Console.WriteLine($"{o.OrderId}, {o.OrderDate}");
                    }
                }

                var customersOrders2 = from c in db.Customers.
                       Include(c => c.Orders).
                       Include("Orders.OrderDetails").
                       OrderBy(c => c.CustomerId).
                       Skip(10).Take(2) select c;

                var customersOrders2x = db.Customers.Include("Orders").Take(2);
                //               
                //Include("CustomerDemographics")

                foreach (var c in customersOrders2)
                {
                    Console.WriteLine($"{c.CustomerId}, {c.ContactName}");
                    foreach (var o in c.Orders)
                    {
                        Console.WriteLine($"{o.OrderId}, {o.OrderDate}");

                        foreach (var od in o.OrderDetails)
                        {
                            Console.WriteLine($"{od.ProductId}, {od.Quantity}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// https://dynamic-linq.net/
        /// dotnet add package Microsoft.EntityFrameworkCore.DynamicLinq
        /// using System.Linq.Dynamic.Core;
        /// </summary>
        static void LINQDynamic()
        {
            // Ordenamiento
            using (NWContext db = new NWContext())
            {
                string order = "Country DESC, City";

                var q1 = db.Customers.Where(c => c.Orders.Count > 5).OrderBy(order);

                Console.Clear();
                foreach (var item in q1)
                {
                    Console.WriteLine($"{item.CompanyName.PadRight(35)}, {item.Country}, {item.City}");
                }
            }
        }

        static void LINQToXML()
        {
            using (NWContext db = new NWContext())
            {
                List<Customer> customers = db.Customers.ToList();

                // LINQ to Objects
                var q1 = from c in customers
                         where c.Country == "Mexico"
                         select c;
                var q1x = customers.Where(c => c.Country == "Mexico");

                // LINQ to XML
                var docXML = new XElement("customers",
                    from c in customers
                    select new XElement("customer",
                        new XElement("id", c.CustomerId),
                        new XElement("companyName", c.CompanyName),
                        new XElement("contactName", c.ContactName))
                    );
                docXML.Save("customer.xml");

                var docXML2 = XElement.Load("customer.xml");
                var query = from c in docXML2.Descendants("customer")
                            where c.Element("companyName").Value.StartsWith("A", StringComparison.CurrentCulture)
                            select new Customer()
                            {
                                CustomerId = c.Element("id").Value,
                                CompanyName = c.Element("companyName").Value,
                                ContactName = c.Element("contactName").Value
                            };

                foreach (var item in query)
                {
                    Console.WriteLine(item.CompanyName);
                }
            }
        }

        static void PLINQ()
        {
            Console.WriteLine("Experimento PLINQ en proceso ...");

            // Parallel LINQ
            var nums = Enumerable.Range(1, 10000);

            var query = from n in nums.AsParallel()
                        where ToDo(n) == n
                        select ToDo(n);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var result = query.ToList();

            sw.Stop();

            Console.WriteLine($"Duración: {sw.ElapsedMilliseconds}");
        }

        static int ToDo(int n)
        {
            System.Threading.Thread.SpinWait(1000);
            return n;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/ef/core/saving/transactions
        /// </summary>
        static void Transactions()
        {
            using (var db = new NWContext())
            {
                var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    db.SaveChanges();
                    // Acciones
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }

            var tso = new TransactionOptions();
            tso.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

            using (var ts = new TransactionScope(TransactionScopeOption.Required, tso))
            {
                using (var db = new NWContext())
                {
                    // CRUD actions
                    db.SaveChanges();
                }

                using (var db = new NWContext())
                {
                    // CRUD actions
                    db.SaveChanges();
                }

                using (var db = new NWContext())
                {
                    // CRUD actions
                    db.SaveChanges();
                }

                ts.Complete();
            }
        }
        static void RawSQL()
        {
            using (var db = new NWContext())
            {
                // https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
                // Prior to version 3.0, FromSqlRaw and FromSqlInterpolated were two overloads named FromSql
                //var q2 = db.Products.FromSql("",null)
                var quantity = 12;
                int productsTotal = db.Database.ExecuteSqlInterpolated($"update [Order Details] set Quantity = {quantity} where OrderID = 10248 and ProductID = 11");

                var pQuantity = new SqlParameter("quantity", quantity);
                int affected = db.Database.ExecuteSqlRaw("update [Order Details] set Quantity = @quantity where OrderID = 10248 and ProductID = 11", pQuantity);

                var filter = 1;
                var products0 = db.Products.FromSqlInterpolated(@$"SELECT [ProductID],[ProductName],[SupplierID],[CategoryID]
                        ,[QuantityPerUnit],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]
                        FROM[dbo].[Products] WHERE [ProductID] = {filter}").Include("Supplier");

                var pFilter = new SqlParameter("filter", filter);
                var products1 = db.Products.FromSqlRaw(@$"SELECT [ProductID],[ProductName],[SupplierID],[CategoryID]
                        ,[QuantityPerUnit],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]
                        FROM[dbo].[Products] WHERE [ProductID] = @filter", pFilter);

                // Store Procedure
                //var blogs = db.Products.FromSqlRaw("EXECUTE dbo.GetMostPopularProducts").ToList();
            }
        }
        static void StoreProcedures()
        {
            using (var db = new NWContext())
            {
                var inicio = new DateTime(1997, 1, 1);
                var fin = DateTime.Now;
                var sales = db.SalesByYear(inicio, fin);

                foreach (var s in sales)
                {
                    Console.WriteLine($"{s.OrderID}\t{s.ShippedDate?.ToShortDateString()}\t{s.Subtotal}\t{s.Year}");
                }
            }
        }

        /// <summary>
        /// https://www.nuget.org/packages/Microsoft.Data.SqlClient/
        /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-code-examples#sqlclient
        /// </summary>
        static void RawSQLClient()
        {
            var connStr = LeerConfiguracion();
            var customerId = "VINET";

            using (var conn = new SqlConnection(connStr))
            {
                var comm = conn.CreateCommand();
                comm.CommandText = @"select o.*, 
                    (select sum(od.Quantity * od.UnitPrice)
                    from[Order Details] od
                    where od.OrderID = o.OrderID) OrderTotal
                from Orders o
                where o.CustomerID = @customerId";
                comm.Parameters.Add(new SqlParameter("customerId", customerId));

                conn.Open();

                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oId = reader.GetInt32(0);
                        var oTotal = Convert.ToDecimal(reader["OrderTotal"]);
                    }
                }

                var da = new SqlDataAdapter(comm);
                var dt = new DataTable();
                da.Fill(dt);

                // Se puede usar LINQ to DataSets
                var columns = dt.Columns;
                foreach (var dr in dt.AsEnumerable())
                {
                    var line = new StringBuilder();
                    foreach (DataColumn c in columns)
                    {
                        line.Append($"{dr.Field<int>("OrderID")}, ");
                    }
                    Console.WriteLine(line);
                }
            }
        }
        #endregion
    }

    public class MiGrupo : List<Product>
    {
        public Tuple<int, string> Llave { get; set; }
    }
}
