using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtWabApiTutorial
{
    public class GraphValue
    {
        [Key]
        [JsonIgnore]
        public int GraphValueId { get; set; }
        public double Value { get; set; }
    }
}
