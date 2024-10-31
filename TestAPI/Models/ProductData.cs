namespace TestAPI.Models
{
    public class ProductData
    {
        public string? Color { get; set; }                  
        public int? CapacityGB { get; set; }               
        public decimal? Price { get; set; }                
        public string? Generation { get; set; }              
        public int? Year { get; set; }                      
        public string? CPUModel { get; set; }                
        public string? HardDiskSize { get; set; }            
        public string? StrapColour { get; set; }             
        public string? CaseSize { get; set; }                
        public string? Description { get; set; }              
        public double? ScreenSize { get; set; }              

        // Constructor to initialize ProductData properties
        public ProductData(int year, decimal price, string cpuModel, string hardDiskSize)
        {
            Year = year;
            Price = price;
            CPUModel = cpuModel;
            HardDiskSize = hardDiskSize;
        }

        public ProductData(decimal price, string color)
        {
            Price = price;
            Color = color;
        }
        public ProductData() { }
    }
}
