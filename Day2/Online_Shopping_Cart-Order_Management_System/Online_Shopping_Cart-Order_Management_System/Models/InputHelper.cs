namespace Online_Shopping_Cart_Order_Management_System.Models;

public static class InputHelper
{
    public static bool TryParseQuantity(
        string? input,
        out int quantity)
    {
        if (int.TryParse(input, out quantity) && quantity > 0)
        {
            return true;
        }

        quantity = 0;
        return false;
    }
}