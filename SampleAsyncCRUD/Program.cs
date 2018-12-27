using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ORM.SqlBuilder;
using System.Data;
using System.Threading;

namespace SampleAsyncCRUD
{
    partial class Program
    {
        static SqlBuilder<Product> sql = new SqlBuilder<Product>();

        static SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Initial Catalog=crud;Integrated Security=True");

        static void Main(string[] args)
        {
            Console.WriteLine("Type: [C]-Create, [R]-Read, [U]-Update, [D]-Delete, [X]-Exit  then Hit Enter");
            while (true)
            {
                var key = Console.ReadLine().ToString().ToLower();

                DisplayCRUD(key);
            }
           
        }

        static async void DisplayCRUD(string key)
        {
            
            switch (key)
            {
                case "c":
                    await Create(new Product { Name = $"item {Guid.NewGuid().ToString()}" });
                    break;
                case "r":
                    await Read();
                    break;
                case "u":
                    await Update(new Product { Id = 2, Name = $"item updated" });
                    break;
                case "d":
                    await Delete(new Product { Id = 4 });
                    break;
                default:
                    break;
            }
            
        }

        static async Task Read()
        {
            var products = await GetProducts();

            foreach (var product in products)
            {
                Console.WriteLine(product.Name);
            }
        }

        static Task<List<Product>> GetProducts()
        {
            var task = Task.Factory.StartNew(() => 
            {
                var products = new List<Product>();

                conn.Open();

                var cmd = conn.CreateCommand();

                cmd.CommandText = sql.SelectAll;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(
                        new Product { Id = Convert.ToInt32(reader["id"]), Name = reader["name"].ToString() }
                        );
                }

                conn.Close();

                // assuming heavy query
                // GUI still responsive
                // read products on 3 sec
                Console.WriteLine("Reading ...");
                Thread.Sleep(3000);

                return products;
            });

            return task;
           
        }

        static Task Create(Product product)
        {
            var task = Task.Factory.StartNew(() =>
            {
                conn.Open();

                var parameters = new Dictionary<string, object>
                {
                    { "@name", product.Name }
                };

                GetCommand(sql.Insert, parameters).ExecuteNonQuery();

                conn.Close();

                Console.WriteLine("Created");
            });

            return task;

        }

        static Task Update(Product product)
        {
            var task = Task.Factory.StartNew(() =>
            {
                conn.Open();

                var parameters = new Dictionary<string, object>
                {
                    { "@id", product.Id },
                    { "@name", product.Name }
                };

                GetCommand(sql.Update, parameters).ExecuteNonQuery();

                conn.Close();

                Console.WriteLine("Updated");
            });

            return task;

        }

        static Task Delete(Product product)
        {
            var task = Task.Factory.StartNew(() =>
            {
                conn.Open();

                var parameters = new Dictionary<string, object>
                {
                    { "@id", product.Id }
                };

                GetCommand(sql.Delete, parameters).ExecuteNonQuery();

                conn.Close();

                Console.WriteLine("Deleted");
            });

            return task;

        }

        static IDbCommand GetCommand(string sql, Dictionary<string, object> parameters = null)
        {
            var cmd = conn.CreateCommand();

            cmd.CommandText = sql;

            if (parameters is null)
                return cmd;

            foreach (var p in parameters)
            {
                var parameter = cmd.CreateParameter();

                parameter.ParameterName = p.Key;
                parameter.Value = p.Value;

                cmd.Parameters.Add(parameter);
            }

            return cmd;
        }
    }
}
