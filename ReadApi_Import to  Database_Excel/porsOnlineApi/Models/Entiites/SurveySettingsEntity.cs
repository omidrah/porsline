using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace porsOnlineApi.Models
{
    [Table("SurveySettings")]
    public class SurveySettingsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }

        public bool AuthenticationNeeded { get; set; }
        public bool PorslineAuth { get; set; }
        public bool CodeAuth { get; set; }
        public bool PhoneNumberAuth { get; set; }
        public bool EditResponseEnabled { get; set; }
        public bool ShowAnswerSheetEnabled { get; set; }
        public bool ShowAnswerSheetToResponder { get; set; }
        public bool ShowAnswerKeyAfterResponseSubmit { get; set; }
        public bool ShowAnswerKeyAfterSurveyStop { get; set; }
        public int RespondingDurationType { get; set; }
        public int? QuestionsRespondingDuration { get; set; }
        public bool LocalStorageIsEnabled { get; set; }
        public bool NoIndex { get; set; }

        // Navigation property
        public virtual SurveyEntity Survey { get; set; } = null!;
    }
}
