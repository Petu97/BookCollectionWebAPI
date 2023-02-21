using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Book
    {
        [JsonPropertyName("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        [Required(AllowEmptyStrings =false)]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        [Required(AllowEmptyStrings = false)]
        public string Author { get; set; }

        [JsonPropertyName("year")]
        [Required(AllowEmptyStrings = false)]
        public int Year { get; set; }

        [JsonPropertyName("publisher")]
        public string? Publisher { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

    }
}
