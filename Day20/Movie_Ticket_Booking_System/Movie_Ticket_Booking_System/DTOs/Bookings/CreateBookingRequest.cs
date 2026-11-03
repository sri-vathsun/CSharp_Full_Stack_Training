
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings;

public class CreateBookingRequest
{
    [Required]
    [Phone]
    [MaxLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ShowId { get; set; }

    [Required]
    [MinLength(1)]
    public List<int> SeatIds { get; set; } = new();
}
