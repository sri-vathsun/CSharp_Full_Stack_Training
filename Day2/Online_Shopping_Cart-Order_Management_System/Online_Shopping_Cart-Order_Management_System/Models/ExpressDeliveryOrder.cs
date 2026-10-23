namespace Online_Shopping_Cart_Order_Management_System.Models;

public class ExpressDeliveryOrder : Order
{
    public ExpressDeliveryOrder(ShoppingCart cart)
        : base(cart)
    {
    }

    public override decimal CalculateShippingCharge()
    {
        return Cart.Items.Count > 0 ? 250m : 0m;
    }
}