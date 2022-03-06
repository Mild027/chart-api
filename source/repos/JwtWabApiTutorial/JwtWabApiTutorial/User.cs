using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtWabApiTutorial
{
    public class User
    {
        [Key]
    
        public int UserId { get; set; }
        [StringLength(20)]
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
