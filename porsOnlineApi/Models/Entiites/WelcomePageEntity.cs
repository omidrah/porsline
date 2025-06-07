using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("WelcomePages")]
    public class WelcomePageEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? HtmlTitle { get; set; }

        public int Type { get; set; }
        public bool TitleActive { get; set; }
        public bool DescriptionActive { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string EnterText { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DescriptionText { get; set; } = string.Empty;

        // Navigation property
        public virtual SurveyEntity Survey { get; set; } = null!;
    }
}
