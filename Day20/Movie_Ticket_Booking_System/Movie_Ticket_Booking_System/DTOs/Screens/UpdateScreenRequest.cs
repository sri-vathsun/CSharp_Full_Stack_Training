using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Screens;

public class UpdateScreenRequest
{
    [Required]
    public int TheatreId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ScreenName { get; set; } = string.Empty;
}