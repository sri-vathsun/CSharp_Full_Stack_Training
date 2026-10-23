namespace Online_Shopping_Cart_Order_Management_System.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int LoyaltyPoints { get; set; }

        public Customer(int customerId, string name, string email)
        {
            CustomerId = customerId;
            Name = name;
            Email = email;
            LoyaltyPoints = 0;
        }
    }
}
