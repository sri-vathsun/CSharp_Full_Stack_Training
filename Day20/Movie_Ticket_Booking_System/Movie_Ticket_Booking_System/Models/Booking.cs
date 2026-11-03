
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Models;

public class Booking
{
    public int BookingId { get; set; }

    [Required]
    [MaxLength(30)]
    public string BookingReference { get; set; } = string.Empty;

    public int ShowId { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    [Range(0, 99999999)]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Confirmed";

    public Show Show { get; set; } = null!;

    public User User { get; set; } = null!;

    public ICollection<BookingSeat> BookingSeats { get; set; }
        = new List<BookingSeat>();
}
