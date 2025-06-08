using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{

    [Table("AppreciationPages")]
    public class AppreciationPageEntity
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
        public bool ShareLinkActive { get; set; }
        public bool IsDefault { get; set; }
        public bool LinkActive { get; set; }

        [MaxLength(200)]
        public string LinkButtonText { get; set; } = string.Empty;

        public int LinkType { get; set; }

        [MaxLength(500)]
        public string Link { get; set; } = string.Empty;

        public bool ReloadActive { get; set; }
        public int ReloadTime { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DescriptionText { get; set; } = string.Empty;

        // Navigation property
        public virtual SurveyEntity Survey { get; set; } = null!;
    }
}
