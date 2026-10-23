namespace Online_Shopping_Cart_Order_Management_System.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal
        {
            get
            {
                return Product.Price * Quantity;
            }
        }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
    }

