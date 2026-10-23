namespace Online_Shopping_Cart_Order_Management_System.Models;

public class CardPaymentProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing card payment of ₹{amount:N2}...");

        Console.Write("Simulate successful payment? (Y/N): ");
        string? response = Console.ReadLine();

        return string.Equals(
            response?.Trim(),
            "Y",
            StringComparison.OrdinalIgnoreCase);
    }
}