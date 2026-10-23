namespace Online_Shopping_Cart_Order_Management_System.Models;

public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount);
}