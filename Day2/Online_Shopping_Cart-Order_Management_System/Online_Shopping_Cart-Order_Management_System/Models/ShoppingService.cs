namespace Online_Shopping_Cart_Order_Management_System.Models;

public class ShoppingService
{
    // Default discount calculation
    public decimal CalculateDiscount(decimal subtotal)
    {
        if (subtotal >= 5000)
        {
            return CalculateDiscount(subtotal, 10);
        }

        return 0;
    }

    // Overloaded method for a custom discount percentage
    public decimal CalculateDiscount(decimal subtotal, decimal percentage)
    {
        if (subtotal < 5000 || percentage <= 0 || percentage > 100)
        {
            return 0;
        }

        return subtotal * percentage / 100;
    }

    // Calculate subtotal, discount, shipping and final amount
    public (decimal Subtotal, decimal Discount, decimal Shipping, decimal FinalAmount)
     CalculateTotals(
         ShoppingCart cart,
         bool applyDiscount = true,
         decimal shippingCharge = 100m)
    {
        decimal subtotal = 0;

        foreach (CartItem item in cart.Items)
        {
            subtotal += item.LineTotal;
        }

        decimal discount = applyDiscount
            ? CalculateDiscount(subtotal)
            : 0m;

        decimal shipping = cart.Items.Count > 0
            ? shippingCharge
            : 0m;

        decimal finalAmount = subtotal - discount + shipping;

        return (subtotal, discount, shipping, finalAmount);
    }
}