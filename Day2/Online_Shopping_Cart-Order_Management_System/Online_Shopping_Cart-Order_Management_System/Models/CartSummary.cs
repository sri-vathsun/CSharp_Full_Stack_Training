namespace Online_Shopping_Cart_Order_Management_System.Models;

public readonly struct CartSummary
{
    public decimal Subtotal { get; }
    public int ItemCount { get; }

    public CartSummary(decimal subtotal, int itemCount)
    {
        Subtotal = subtotal;
        ItemCount = itemCount;
    }
}