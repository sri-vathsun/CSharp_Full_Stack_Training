namespace Online_Shopping_Cart_Order_Management_System.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; } = new List<CartItem>();

        public bool AddProduct(Product product, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be greater than zero.");
                return false;
            }

            CartItem? existingItem = Items.Find(
                item => item.Product.Id == product.Id);

            int quantityAlreadyAdded = existingItem?.Quantity ?? 0;

            if (quantityAlreadyAdded + quantity > product.Stock)
            {
                Console.WriteLine(
                    $"Insufficient stock. Available stock: {product.Stock - quantityAlreadyAdded}");

                return false;
            }

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
            }
            else
            {
                Items.Add(new CartItem(product, quantity));
            }

            Console.WriteLine($"{product.Name} added to cart.");
            return true;
        }

        public bool AddProduct(int productId, int quantity, Product[] products)
        {
            Product? product = Array.Find(
                products, p => p.Id == productId);

            if (product == null)
            {
                Console.WriteLine("Product ID not found.");
                return false;
            }

            return AddProduct(product, quantity);

        }

        public bool RemoveProduct(int productId)
        {
            CartItem? item = Items.Find(
               item => item.Product.Id == productId);

            if (item == null)
            {
                Console.WriteLine("Product is not in the cart.");
                return false;
            }
            Items.Remove(item);
            Console.WriteLine($"{item.Product.Name} removed from the cart.");
            return true;
        }

        public void DisplayCart()
        {
            if (Items.Count == 0)
            {
                Console.WriteLine("Your shopping cart is empty.");
                return;
            }

            decimal subtotal = 0;

            Console.WriteLine("\n--- SHOPPING CART ---");

            foreach (CartItem item in Items)
            {
                Console.WriteLine(
            $"{item.Product.Name} | Quantity: {item.Quantity} | " +
            $"Unit Price: ₹{item.Product.Price:N2} | " +
            $"Line Total: ₹{item.LineTotal:N2}");

                subtotal += item.LineTotal;
            }
            Console.WriteLine($"Cart subtotal: ₹{subtotal:N2}");
            Console.WriteLine($"Total items: {Items.Sum(item => item.Quantity)}");

        }
    }
}
