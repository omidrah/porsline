
using porsOnlineApi.JsonModel;

namespace porsOnlineApi.Extensions
{
    public static class QuestionExtensions
    {
        public static bool IsMultipleChoice(this Question question)
        {
            return question.Type == 3;
        }

        public static bool IsTextInput(this Question question)
        {
            return question.Type == 2;
        }

        public static bool IsRatingScale(this Question question)
        {
            return question.Type == 7;
        }

        public static bool HasChoices(this Question question)
        {
            return question.Choices != null && question.Choices.Any();
        }

        public static int GetChoiceCount(this Question question)
        {
            return question.Choices?.Count ?? 0;
        }

        public static bool AllowsMultipleSelection(this Question question)
        {
            return question.AllowMultipleSelect ?? false;
        }

        public static string GetQuestionTypeDescription(this Question question)
        {
            return question.Type switch
            {
                1 => "Welcome/Information",
                2 => "Text/Numeric Input",
                3 => "Multiple Choice",
                7 => "Rating Scale",
                9 => "Appreciation/Thank You",
                _ => "Unknown"
            };
        }
    }
}
