namespace Online_Shopping_Cart_Order_Management_System.Models;

public static class LoyaltyService
{
    public static void AddLoyaltyPoints(
        ref int loyaltyPoints,
        decimal amountPaid)
    {
        int earnedPoints = (int)(amountPaid / 100);

        loyaltyPoints += earnedPoints;
    }
}