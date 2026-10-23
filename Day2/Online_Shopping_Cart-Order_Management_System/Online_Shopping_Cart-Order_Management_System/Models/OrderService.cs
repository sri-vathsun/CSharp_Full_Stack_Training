namespace Online_Shopping_Cart_Order_Management_System.Models;

public class OrderService
{
    public bool Checkout(
        ShoppingCart cart,
        IPaymentProcessor paymentProcessor,
        decimal shippingCharge,
        bool applyDiscount,
        ShoppingService shoppingService)
    {
        // 1. Validate the cart
        if (cart.Items.Count == 0)
        {
            Console.WriteLine("Checkout failed: your cart is empty.");
            return false;
        }

        // 2. Validate stock for every cart item
        foreach (CartItem item in cart.Items)
        {
            if (item.Quantity <= 0 ||
                item.Quantity > item.Product.Stock)
            {
                Console.WriteLine(
                    $"Checkout failed: invalid quantity or insufficient stock for {item.Product.Name}.");

                return false;
            }
        }

        // 3. Calculate the final amount
        var totals = shoppingService.CalculateTotals(
            cart,
            applyDiscount,
            shippingCharge);

        Console.WriteLine("\n--- PAYMENT SUMMARY ---");
        Console.WriteLine($"Subtotal: ₹{totals.Subtotal:N2}");
        Console.WriteLine($"Discount: ₹{totals.Discount:N2}");
        Console.WriteLine($"Shipping: ₹{totals.Shipping:N2}");
        Console.WriteLine($"Amount to pay: ₹{totals.FinalAmount:N2}");

        // 4. Process payment
        bool paymentSuccessful =
            paymentProcessor.ProcessPayment(totals.FinalAmount);

        if (!paymentSuccessful)
        {
            Console.WriteLine("Payment failed. Stock has not been changed.");
            return false;
        }

        // 5. Update stock only after successful payment
        foreach (CartItem item in cart.Items)
        {
            item.Product.Stock -= item.Quantity;
        }

        // 6. Confirm order and clear the cart
        Console.WriteLine("Payment successful!");
        Console.WriteLine("Order placed successfully.");

        foreach (CartItem item in cart.Items)
        {
            Console.WriteLine(
                $"{item.Product.Name} x {item.Quantity}");
        }

        Console.WriteLine($"Amount paid: ₹{totals.FinalAmount:N2}");

        cart.Items.Clear();

        return true;
    }
}