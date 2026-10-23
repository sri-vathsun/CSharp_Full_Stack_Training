using Online_Shopping_Cart_Order_Management_System.Models;


Product[] products =
{
    new Product(101, "Laptop", 55000m, 5),
    new Product(102, "Headphones", 2000m, 10),
    new Product(103, "Keyboard", 1500m, 8),
    new Product(104, "Mouse", 700m, 15),
    new Product(105, "Monitor", 12000m, 4)
};

Customer customer = new Customer(
    1,
    "Arun",
    "arun@example.com");

ShoppingCart cart = new ShoppingCart();
ShoppingService shoppingService = new ShoppingService();
OrderService orderService = new OrderService();

while (true)
{
    Console.WriteLine("\n--- ONLINE SHOPPING SYSTEM ---");
    Console.WriteLine("1. Display all products");
    Console.WriteLine("2. Sort products by name");
    Console.WriteLine("3. Search product by name");
    Console.WriteLine("4. View customer profile");
    Console.WriteLine("5. Add product to cart");
    Console.WriteLine("6. Remove product from cart");
    Console.WriteLine("7. View shopping cart");
    Console.WriteLine("8. Calculate totals");
    Console.WriteLine("9. Checkout");
    Console.WriteLine("10. View last order summary");
    // here i need to add these datas to the text file 
    Console.WriteLine("11. Add data to text file");
    Console.WriteLine("12. Exit");

    Console.Write("Enter your choice: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            DisplayProducts(products);
            break;

        case "2":
            Array.Sort(products, (a, b) =>
                string.Compare(a.Name, b.Name,
                    StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Products sorted alphabetically.");
            DisplayProducts(products);
            break;

        case "3":
            Console.Write("Enter product name: ");
            string? searchName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchName))
            {
                Console.WriteLine("Product name cannot be empty.");
                break;
            }

            Product? foundProduct = Array.Find(
                products,
                p => string.Equals(
                    p.Name,
                    searchName.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (foundProduct != null)
            {
                Console.WriteLine(
                    $"{foundProduct.Id} | {foundProduct.Name} | " +
                    $"₹{foundProduct.Price:N2} | Stock: {foundProduct.Stock}");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }

            break;

        case "4":
            Console.WriteLine("\n--- CUSTOMER PROFILE ---");
            Console.WriteLine($"ID: {customer.CustomerId}");
            Console.WriteLine($"Name: {customer.Name}");
            Console.WriteLine($"Email: {customer.Email}");
            Console.WriteLine($"Loyalty Points: {customer.LoyaltyPoints}");
            break;

        case "5":
            DisplayProducts(products);

            Console.Write("Enter product ID: ");
            string? idInput = Console.ReadLine();

            Console.Write("Enter quantity: ");
            string? quantityInput = Console.ReadLine();

            if (!int.TryParse(idInput, out int productId))
            {
                Console.WriteLine("Enter a valid product ID.");
                break;
            }

            if (!InputHelper.TryParseQuantity(quantityInput, out int quantity))
            {
                Console.WriteLine("Enter a valid positive quantity.");
                break;
            }

            cart.AddProduct(productId, quantity, products);
            break;

        case "6":
            cart.DisplayCart();

            Console.Write("Enter product ID to remove: ");
            string? removeInput = Console.ReadLine();

            if (!int.TryParse(removeInput, out int removeId))
            {
                Console.WriteLine("Enter a valid product ID.");
                break;
            }

            cart.RemoveProduct(removeId);
            break;

        case "7":
            cart.DisplayCart();
            break;

        case "8":
            {
                if (cart.Items.Count == 0)
                {
                    Console.WriteLine("Your cart is empty. Add products first.");
                    break;
                }

                // Keep your existing case 8 code here.

                break;
            }


        case "9":
            if (cart.Items.Count == 0)
            {
                Console.WriteLine("Your cart is empty. Add products first.");
                break;
            }

            Console.WriteLine("\n--- SELECT SHIPPING ---");
            Console.WriteLine("1. Standard Delivery - ₹100");
            Console.WriteLine("2. Express Delivery - ₹250");
            Console.Write("Choose shipping: ");

            string? shippingChoice = Console.ReadLine();

            Order? order = shippingChoice switch
            {
                "1" => new StandardDeliveryOrder(cart),
                "2" => new ExpressDeliveryOrder(cart),
                _ => null
            };

            if (order == null)
            {
                Console.WriteLine("Invalid shipping option.");
                break;
            }

            Console.WriteLine("\n--- SELECT PAYMENT ---");
            Console.WriteLine("1. Card");
            Console.WriteLine("2. UPI");
            Console.Write("Choose payment method: ");

            string? paymentChoice = Console.ReadLine();

            IPaymentProcessor? paymentProcessor = paymentChoice switch
            {
                "1" => new CardPaymentProcessor(),
                "2" => new UpiPaymentProcessor(),
                _ => null
            };

            if (paymentProcessor == null)
            {
                Console.WriteLine("Invalid payment method.");
                break;
            }

            Console.Write("Apply discount? (Y/N): ");
            string? discountChoice = Console.ReadLine();

            bool applyDiscount = string.Equals(
                discountChoice?.Trim(),
                "Y",
                StringComparison.OrdinalIgnoreCase);

            orderService.Checkout(
                cart,
                paymentProcessor,
                order.CalculateShippingCharge(),
                applyDiscount,
                shoppingService);
                break;

        case "10":
            orderService.DisplayLastOrderSummary();
            break;

        case "11":
            Console.Write("Enter the file path to save order summary: ");
            string? filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path cannot be empty.");
                break;
            }
            try
            {
                File.WriteAllText(
                    filePath,
                    orderService.GetLastOrderSummary() ?? "No previous order found.");
                Console.WriteLine($"Order summary saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
            break;



        case "12":
            Console.WriteLine("Exiting the application.");
            return;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}

static void DisplayProducts(Product[] products)
    {
        Console.WriteLine("\n ID     Name       Price        Stock");
        Console.WriteLine("---------------------------------------------");

        foreach (Product product in products)
        {
            Console.WriteLine(
                $"{product.Id,-5} {product.Name,-14} {product.Price.ToString("N2"),-11} {product.Stock}");
        }
    }




public class OrderService
{
    private string? _lastOrderSummary;

    public void SetLastOrderSummary(string summary) =>
        _lastOrderSummary = summary;

    public string? GetLastOrderSummary() =>
        _lastOrderSummary;

    public void DisplayLastOrderSummary()
    {
        if (string.IsNullOrEmpty(_lastOrderSummary))
        {
            Console.WriteLine("No previous order found.");
            return;
        }

        Console.WriteLine("\n--- LAST ORDER SUMMARY ---");
        Console.WriteLine(_lastOrderSummary);
    }

    public void Checkout(
        ShoppingCart cart,
        IPaymentProcessor paymentProcessor,
        decimal shippingCharge,
        bool applyDiscount,
        ShoppingService shoppingService)
    {
        // Calculate totals using the ShoppingService
        var (subtotal, discount, shipping, finalAmount) =
            shoppingService.CalculateTotals(cart, applyDiscount, shippingCharge);

        Console.WriteLine("\n--- ORDER SUMMARY ---");
        Console.WriteLine($"Items: {cart.Items.Count}");
        Console.WriteLine($"Subtotal: ₹{subtotal:N2}");
        Console.WriteLine($"Discount: ₹{discount:N2}");
        Console.WriteLine($"Shipping: ₹{shipping:N2}");
        Console.WriteLine($"Total to pay: ₹{finalAmount:N2}");

        // Process payment
        bool paid = paymentProcessor.ProcessPayment(finalAmount);

        if (!paid)
        {
            Console.WriteLine("Payment failed. Order not placed.");
            return;
        }

        // On success, store a simple textual summary and clear the cart
        string summary = $"Items: {cart.Items.Count}\n" +
                         $"Subtotal: ₹{subtotal:N2}\n" +
                         $"Discount: ₹{discount:N2}\n" +
                         $"Shipping: ₹{shipping:N2}\n" +
                         $"Total Paid: ₹{finalAmount:N2}\n" +
                         $"Order Date: {DateTime.Now:G}";

        SetLastOrderSummary(summary);

        Console.WriteLine("Payment successful. Order placed.");
        cart.Items.Clear();
    }
}





