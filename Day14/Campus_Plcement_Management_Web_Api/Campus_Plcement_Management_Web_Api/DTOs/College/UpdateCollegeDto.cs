
using System.ComponentModel.DataAnnotations;

namespace Campus_Plcement_Management_Web_Api.DTOs.College;

public class UpdateCollegeDto
{
    [Required]
    [StringLength(200)]
    public string CollegeName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? UniversityName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string OfficialEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Url]
    [StringLength(255)]
    public string? Website { get; set; }
}
