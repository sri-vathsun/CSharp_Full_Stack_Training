namespace Online_Shopping_Cart_Order_Management_System.Models;

public static class CartSummaryService
{
    public static void DisplaySummary(in CartSummary summary)
    {
        Console.WriteLine($"Subtotal: ₹{summary.Subtotal:N2}");
        Console.WriteLine($"Number of cart items: {summary.ItemCount}");
    }
}