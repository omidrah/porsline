using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("Questions")]
    public class QuestionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? HtmlTitle { get; set; }

        public int Type { get; set; }
        public bool ImageVideoActive { get; set; }
        public int ImageOrVideo { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ImageName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string VideoUrl { get; set; } = string.Empty;

        public bool ShowCharts { get; set; }
        public bool DescriptionTextActive { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DescriptionText { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? HtmlDescriptionText { get; set; }

        public bool QuestionNumberIsHidden { get; set; }
        public int? RespondingDuration { get; set; }
        public int ImageBrightness { get; set; }
        public int ImagePosition { get; set; }
        public int DesktopImageLayout { get; set; }
        public int MobileImageLayout { get; set; }
        public bool AnswerRequired { get; set; }

        // Multiple choice properties
        public bool? AllowMultipleSelect { get; set; }
        public int? MaxSelectableChoices { get; set; }
        public int? MinSelectableChoices { get; set; }
        public bool? VerticalChoices { get; set; }
        public bool? Randomize { get; set; }

        // Rating scale properties
        public int? Steps { get; set; }
        public int? IconType { get; set; }

        // Text input properties
        public int? AnswerType { get; set; }
        public long? NumberMaxValue { get; set; }
        public long? NumberMinValue { get; set; }
        public int? AnswerMaxLength { get; set; }
        public int? AnswerMinLength { get; set; }
        public bool? IsDecimal { get; set; }
        public int? RegexType { get; set; }

        [MaxLength(500)]
        public string? RegexValue { get; set; }

        [MaxLength(200)]
        public string RegexPlaceholder { get; set; } = string.Empty;

        [MaxLength(500)]
        public string RegexValidationMessage { get; set; } = string.Empty;

        public bool? IsThousandsSeparationEnabled { get; set; }

        // Navigation properties
        public virtual SurveyEntity Survey { get; set; } = null!;
        public virtual ICollection<ChoiceEntity> Choices { get; set; } = new List<ChoiceEntity>();
    }

}
