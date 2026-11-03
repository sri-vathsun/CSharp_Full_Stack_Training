
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows;

public class CreateShowRequest
{
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }

    [Range(1, int.MaxValue)]
    public int ScreenId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [Range(0, 999999)]
    public decimal TicketPrice { get; set; }
}
