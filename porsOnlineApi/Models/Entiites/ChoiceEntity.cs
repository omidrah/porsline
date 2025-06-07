using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("Choices")]
    public class ChoiceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Name { get; set; } = string.Empty;

        public bool Hidden { get; set; }

        [MaxLength(1000)]
        public string? AltName { get; set; }

        public int ChoiceType { get; set; }

        // Navigation property
        public virtual QuestionEntity Question { get; set; } = null!;
    }
}
