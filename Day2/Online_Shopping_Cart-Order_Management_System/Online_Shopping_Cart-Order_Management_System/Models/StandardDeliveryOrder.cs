namespace Online_Shopping_Cart_Order_Management_System.Models;

public class StandardDeliveryOrder : Order
{
    public StandardDeliveryOrder(ShoppingCart cart)
        : base(cart)
    {
    }

    public override decimal CalculateShippingCharge()
    {
        return Cart.Items.Count > 0 ? 100m : 0m;
    }
}