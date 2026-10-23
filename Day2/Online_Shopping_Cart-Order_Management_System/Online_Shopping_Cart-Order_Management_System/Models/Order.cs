namespace Online_Shopping_Cart_Order_Management_System.Models;

public abstract class Order
{
    public ShoppingCart Cart { get; }

    protected Order(ShoppingCart cart)
    {
        Cart = cart;
    }

    public abstract decimal CalculateShippingCharge();
}