
using System;

class Program
{
    static void Main()
    {
        int orderId;
        string customerName;
        string productName;
        int quantity;
        decimal unitPrice;
        decimal totalPrice;
        while (true)
        {
            Console.WriteLine("Enter Order ID:");

            if (int.TryParse(Console.ReadLine(), out orderId)
                && orderId > 0)
            {
                break;
            }

            Console.WriteLine(
                "Invalid Order ID. Enter a positive integer.");
        }
        while (true)
        {
            Console.WriteLine("Enter Customer Name:");
            customerName = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                break;
            }

            Console.WriteLine(
                "Customer Name cannot be empty.");
        }

        while (true)
        {
            Console.WriteLine("Enter Product Name:");
            productName = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(productName))
            {
                break;
            }

            Console.WriteLine(
                "Product Name cannot be empty.");
        }

        while (true)
        {
            Console.WriteLine("Enter Quantity:");

            if (int.TryParse(Console.ReadLine(), out quantity)
                && quantity > 0)
            {
                break;
            }

            Console.WriteLine(
                "Invalid Quantity. Enter a positive integer.");
        }

        while (true)
        {
            Console.WriteLine("Enter Unit Price:");

            if (decimal.TryParse(Console.ReadLine(), out unitPrice)
                && unitPrice >= 0)
            {
                break;
            }

            Console.WriteLine(
                "Invalid Unit Price. Enter a non-negative decimal.");
        }

        if (unitPrice != 0 &&
            quantity > (decimal.MaxValue / unitPrice))
        {
            Console.WriteLine(
                "Error: Total price exceeds the allowed range.");
            return;
        }

        totalPrice = quantity * unitPrice;
  
        Console.WriteLine("\n--- Order Details ---");
        Console.WriteLine($"Order ID: {orderId}");
        Console.WriteLine($"Customer Name: {customerName}");
        Console.WriteLine($"Product Name: {productName}");
        Console.WriteLine($"Quantity: {quantity}");
        Console.WriteLine($"Unit Price: {unitPrice:C}");
        Console.WriteLine($"Total Price: {totalPrice:C}");

        if (totalPrice < 1000)
        {
            Console.WriteLine("Order Type: Small Order");
        }
        else if (totalPrice < 5000)
        {
            Console.WriteLine("Order Type: Medium Order");
        }
        else if (totalPrice < 10000)
        {
            Console.WriteLine("Order Type: Large Order");
        }
        else
        {
            Console.WriteLine("Order Type: Bulk Order");
        }

        Console.WriteLine("\nOrder processing finished.");
    }
}
