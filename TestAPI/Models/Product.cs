
using System.Text.Json.Serialization;

namespace TestAPI.Models
{
    public class Product
    {
        public string? Id { get; set; }  
        public string? Name { get; set; }     
        public ProductData? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] // Only include if it has a value other than the default
        public DateTime CreatedAt { get; set; } 

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]  // Only include if it has a value other than the defaul
        public DateTime UpdatedAt { get; set; }



        // Constructor for POST method where Id not required
        public Product(string name)
        {
            Name = name;
        }

        // Parameterless constructor for Get requests where Id is required
        public Product() { }

    }
}
