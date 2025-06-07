using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("SurveyThemes")]
    public class SurveyThemeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public int Order { get; set; }

        [MaxLength(20)]
        public string BackgroundColor { get; set; } = string.Empty;

        [MaxLength(20)]
        public string QuestionColor { get; set; } = string.Empty;

        [MaxLength(20)]
        public string AnswerColor { get; set; } = string.Empty;

        [MaxLength(20)]
        public string ButtonColor { get; set; } = string.Empty;

        [MaxLength(20)]
        public string AccentColor { get; set; } = string.Empty;

        public int FontFamily { get; set; }
        public int FontSize { get; set; }
        public bool IsPublic { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        // Background image properties
        [MaxLength(500)]
        public string? BackgroundImageUrl { get; set; }
        public int BackgroundImageRepeat { get; set; }
        public int BackgroundImageBrightness { get; set; }
        public int BackgroundImageFit { get; set; }
        public int BackgroundImagePosition { get; set; }
        public int BackgroundImageSizePercentage { get; set; }

        // Navigation property
        public virtual SurveyEntity Survey { get; set; } = null!;
    }
}
