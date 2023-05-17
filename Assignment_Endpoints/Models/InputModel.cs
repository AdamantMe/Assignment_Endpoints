using System.ComponentModel.DataAnnotations;

namespace Assignment_Endpoints.Models
{
    public class InputModel
    {
        [Required]
        public string Input { get; set; }
    }
}
