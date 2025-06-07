using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace porsOnlineApi.Models
{
    [Table("SurveyFolders")]
    public class SurveyFolderEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public int Order { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(1000)]
        public string? SharedBy { get; set; }

        [MaxLength(1000)]
        public string? SharedWith { get; set; }

        // Navigation properties
        public virtual ICollection<SurveyEntity> Surveys { get; set; } = new List<SurveyEntity>();
    }
}