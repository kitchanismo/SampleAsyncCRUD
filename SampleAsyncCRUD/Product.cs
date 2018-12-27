using ORM.SqlBuilder.Attributes;

namespace SampleAsyncCRUD
{
    partial class Program
    {

        [Schema("dbo")]
        [Alias("products")] 
        public class Product
        {
            [Key(Identity = true)] 
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
