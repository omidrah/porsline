using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("Surveys")]
    public class SurveyEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Name { get; set; } = string.Empty;

        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        public int Language { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public bool CanActive { get; set; }
        public bool IsStopped { get; set; }
        public int Views { get; set; }
        public int SubmittedResponses { get; set; }

        [MaxLength(50)]
        public string PreviewCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ReportCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? UrlSlug { get; set; }

        public bool IsTemplate { get; set; }
        public bool HasQuestion { get; set; }

        [MaxLength(200)]
        public string? Subdomain { get; set; }

        [MaxLength(200)]
        public string? Domain { get; set; }

        public DateTime? LastResponseDateTime { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? LabelsJson { get; set; } // Store as JSON

        [Column(TypeName = "nvarchar(max)")]
        public string? TagsJson { get; set; } // Store as JSON

        // Navigation properties
        public virtual SurveyFolderEntity Folder { get; set; } = null!;
        public virtual SurveyThemeEntity? Theme { get; set; }
        public virtual SurveySettingsEntity? Settings { get; set; }
        public virtual ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
        public virtual ICollection<WelcomePageEntity> WelcomePages { get; set; } = new List<WelcomePageEntity>();
        public virtual ICollection<AppreciationPageEntity> AppreciationPages { get; set; } = new List<AppreciationPageEntity>();
    }
}
