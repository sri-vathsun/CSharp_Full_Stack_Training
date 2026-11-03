using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Theatres;

public class UpdateTheatreRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Location { get; set; } = string.Empty;
}