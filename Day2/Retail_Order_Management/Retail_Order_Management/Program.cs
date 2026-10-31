
using Retail_Order_Management.Models;

Product product = new Product
{
    ProductId = 101,
    ProductName = "Keyboard",
    Category = "Electronics",
    Price = 1200.00m,
    StockQuantity = 25
};

Console.WriteLine("=== Product Details ===");
Console.WriteLine($"Product ID: {product.ProductId}");
Console.WriteLine($"Product Name: {product.ProductName}");
Console.WriteLine($"Category: {product.Category}");
Console.WriteLine($"Price: ₹{product.Price:F2}");
Console.WriteLine($"Stock Quantity: {product.StockQuantity}");

decimal stockValue = product.GetStockValue();

Console.WriteLine($"Total Stock Value: ₹{stockValue:F2}");
