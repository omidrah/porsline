using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;
using System.Text.Json;

namespace porsOnlineApi.Services
{
    public class MappingService : IMappingService
    {
        public SurveyFolderEntity MapToEntity(SurveyFolder folder)
        {
            return new SurveyFolderEntity
            {
                Id = folder.Id,
                Name = folder.Name,
                Order = folder.Order,
                SharedBy = folder.SharedBy?.ToString(),
                SharedWith = folder.SharedWith?.ToString(),
                Surveys = folder.Surveys.Select(MapToEntity).ToList()
            };
        }

        public SurveyFolder MapFromEntity(SurveyFolderEntity entity)
        {
            return new SurveyFolder
            {
                Id = entity.Id,
                Name = entity.Name,
                Order = entity.Order,
                SharedBy = entity.SharedBy,
                SharedWith = entity.SharedWith,
                Surveys = entity.Surveys.Select(MapFromEntity).ToList()
            };
        }

        public SurveyEntity MapToEntity(Survey survey)
        {
            return new SurveyEntity
            {
                Id = survey.Id,
                Name = survey.Name,
                FolderId = survey.FolderId,
                Language = survey.Language,
                CreatedDate = survey.CreatedDate,
                Active = survey.Active,
                CanActive = survey.CanActive,
                IsStopped = survey.IsStopped,
                Views = survey.Views,
                SubmittedResponses = survey.SubmittedResponses,
                PreviewCode = survey.PreviewCode,
                ReportCode = survey.ReportCode,
                UrlSlug = survey.UrlSlug,
                IsTemplate = survey.IsTemplate,
                HasQuestion = survey.HasQuestion,
                Subdomain = survey.Subdomain,
                Domain = survey.Domain,
                LastResponseDateTime = survey.LastResponseDateTime,
                LabelsJson = survey.Labels != null ? JsonSerializer.Serialize(survey.Labels) : null,
                TagsJson = JsonSerializer.Serialize(survey.Tags)
            };
        }

        public Survey MapFromEntity(SurveyEntity entity)
        {
            return new Survey
            {
                Id = entity.Id,
                Name = entity.Name,
                FolderId = entity.FolderId,
                Language = entity.Language,
                CreatedDate = entity.CreatedDate,
                Active = entity.Active,
                CanActive = entity.CanActive,
                IsStopped = entity.IsStopped,
                Views = entity.Views,
                SubmittedResponses = entity.SubmittedResponses,
                PreviewCode = entity.PreviewCode,
                ReportCode = entity.ReportCode,
                UrlSlug = entity.UrlSlug,
                IsTemplate = entity.IsTemplate,
                HasQuestion = entity.HasQuestion,
                Subdomain = entity.Subdomain,
                Domain = entity.Domain,
                LastResponseDateTime = entity.LastResponseDateTime,
                Labels = !string.IsNullOrEmpty(entity.LabelsJson)
                    ? JsonSerializer.Deserialize<List<string>>(entity.LabelsJson)
                    : null,
                Tags = !string.IsNullOrEmpty(entity.TagsJson)
                    ? JsonSerializer.Deserialize<List<object>>(entity.TagsJson) ?? new List<object>()
                    : new List<object>(),
                Theme = entity.Theme != null ? MapThemeFromEntity(entity.Theme) : new SurveyTheme()
            };
        }

        public SurveyEntity MapToEntity(DetailedSurvey survey, int surveyId)
        {
            var entity = new SurveyEntity
            {
                Id = surveyId,
                Name = survey.Name,
                FolderId = survey.Folder.Id,
                Language = ParseLanguage(survey.Language),
                Active = survey.Active,
                PreviewCode = survey.PreviewCode,
                ReportCode = survey.ReportCode,
                UrlSlug = survey.UrlSlug,
                SubmittedResponses = survey.SubmittedResponses,
                Subdomain = survey.Subdomain,
                Domain = survey.Domain,
                LabelsJson = survey.Labels != null ? JsonSerializer.Serialize(survey.Labels) : null,
                CreatedDate = DateTime.UtcNow
            };

            if (survey.Theme != null)
            {
                entity.Theme = MapThemeToEntity(survey.Theme, surveyId);
            }

            if (survey.Settings != null)
            {
                entity.Settings = MapSettingsToEntity(survey.Settings, surveyId);
            }

            entity.Questions = survey.Questions.Select(q => MapQuestionToEntity(q, surveyId)).ToList();
            entity.WelcomePages = survey.Welcome.Select(w => MapWelcomeToEntity(w, surveyId)).ToList();
            entity.AppreciationPages = survey.Appreciations.Select(a => MapAppreciationToEntity(a, surveyId)).ToList();

            return entity;
        }

        public DetailedSurvey MapFromEntityToDetail(SurveyEntity entity)
        {
            return new DetailedSurvey
            {
                Name = entity.Name,
                Folder = new FolderReference { Id = entity.FolderId, Name = entity.Folder?.Name ?? "" },
                Language = GetLanguageName(entity.Language),
                Active = entity.Active,
                Closed = entity.IsStopped,
                Deleted = false,
                PreviewCode = entity.PreviewCode,
                ReportCode = entity.ReportCode,
                UrlSlug = entity.UrlSlug,
                SubmittedResponses = entity.SubmittedResponses,
                Subdomain = entity.Subdomain,
                Domain = entity.Domain,
                Labels = !string.IsNullOrEmpty(entity.LabelsJson)
                    ? JsonSerializer.Deserialize<List<string>>(entity.LabelsJson)
                    : null,
                Theme = entity.Theme != null ? MapThemeFromEntity(entity.Theme) : new SurveyTheme(),
                Settings = entity.Settings != null ? MapSettingsFromEntity(entity.Settings) : new SurveySettings(),
                Questions = entity.Questions.Select(MapQuestionFromEntity).ToList(),
                Welcome = entity.WelcomePages.Select(MapWelcomeFromEntity).ToList(),
                Appreciations = entity.AppreciationPages.Select(MapAppreciationFromEntity).ToList(),
                Variables = new List<object>(),
                ComputationalVariables = new List<object>()
            };
        }

        private SurveyThemeEntity MapThemeToEntity(SurveyTheme theme, int surveyId)
        {
            return new SurveyThemeEntity
            {
                Id = theme.Id,
                SurveyId = surveyId,
                Order = theme.Order,
                BackgroundColor = theme.BackgroundColor,
                QuestionColor = theme.QuestionColor,
                AnswerColor = theme.AnswerColor,
                ButtonColor = theme.ButtonColor,
                AccentColor = theme.AccentColor,
                FontFamily = theme.FontFamily,
                FontSize = theme.FontSize,
                IsPublic = theme.IsPublic,
                ThumbnailUrl = theme.ThumbnailUrl,
                BackgroundImageUrl = theme.BackgroundImage?.Url,
                BackgroundImageRepeat = theme.BackgroundImageRepeat,
                BackgroundImageBrightness = theme.BackgroundImageBrightness,
                BackgroundImageFit = theme.BackgroundImageFit,
                BackgroundImagePosition = theme.BackgroundImagePosition,
                BackgroundImageSizePercentage = theme.BackgroundImageSizePercentage
            };
        }

        private SurveyTheme MapThemeFromEntity(SurveyThemeEntity entity)
        {
            return new SurveyTheme
            {
                Id = entity.Id,
                Order = entity.Order,
                BackgroundColor = entity.BackgroundColor,
                QuestionColor = entity.QuestionColor,
                AnswerColor = entity.AnswerColor,
                ButtonColor = entity.ButtonColor,
                AccentColor = entity.AccentColor,
                FontFamily = entity.FontFamily,
                FontSize = entity.FontSize,
                IsPublic = entity.IsPublic,
                ThumbnailUrl = entity.ThumbnailUrl,
                BackgroundImage = !string.IsNullOrEmpty(entity.BackgroundImageUrl)
                    ? new BackgroundImage { Url = entity.BackgroundImageUrl }
                    : null,
                BackgroundImageRepeat = entity.BackgroundImageRepeat,
                BackgroundImageBrightness = entity.BackgroundImageBrightness,
                BackgroundImageFit = entity.BackgroundImageFit,
                BackgroundImagePosition = entity.BackgroundImagePosition,
                BackgroundImageSizePercentage = entity.BackgroundImageSizePercentage
            };
        }

        private SurveySettingsEntity MapSettingsToEntity(SurveySettings settings, int surveyId)
        {
            return new SurveySettingsEntity
            {
                SurveyId = surveyId,
                AuthenticationNeeded = settings.AuthenticationNeeded,
                PorslineAuth = settings.PorslineAuth,
                CodeAuth = settings.CodeAuth,
                PhoneNumberAuth = settings.PhoneNumberAuth,
                EditResponseEnabled = settings.EditResponseEnabled,
                ShowAnswerSheetEnabled = settings.ShowAnswerSheetEnabled,
                ShowAnswerSheetToResponder = settings.ShowAnswerSheetToResponder,
                ShowAnswerKeyAfterResponseSubmit = settings.ShowAnswerKeyAfterResponseSubmit,
                ShowAnswerKeyAfterSurveyStop = settings.ShowAnswerKeyAfterSurveyStop,
                RespondingDurationType = settings.RespondingDurationType,
                QuestionsRespondingDuration = settings.QuestionsRespondingDuration,
                LocalStorageIsEnabled = settings.LocalStorageIsEnabled,
                NoIndex = settings.NoIndex
            };
        }

        private SurveySettings MapSettingsFromEntity(SurveySettingsEntity entity)
        {
            return new SurveySettings
            {
                AuthenticationNeeded = entity.AuthenticationNeeded,
                PorslineAuth = entity.PorslineAuth,
                CodeAuth = entity.CodeAuth,
                PhoneNumberAuth = entity.PhoneNumberAuth,
                EditResponseEnabled = entity.EditResponseEnabled,
                ShowAnswerSheetEnabled = entity.ShowAnswerSheetEnabled,
                ShowAnswerSheetToResponder = entity.ShowAnswerSheetToResponder,
                ShowAnswerKeyAfterResponseSubmit = entity.ShowAnswerKeyAfterResponseSubmit,
                ShowAnswerKeyAfterSurveyStop = entity.ShowAnswerKeyAfterSurveyStop,
                RespondingDurationType = entity.RespondingDurationType,
                QuestionsRespondingDuration = entity.QuestionsRespondingDuration,
                LocalStorageIsEnabled = entity.LocalStorageIsEnabled,
                NoIndex = entity.NoIndex
            };
        }

        private QuestionEntity MapQuestionToEntity(Question question, int surveyId)
        {
            var entity = new QuestionEntity
            {
                Id = question.Id,
                SurveyId = surveyId,
                Title = question.Title,
                HtmlTitle = question.HtmlTitle,
                Type = question.Type,
                ImageVideoActive = question.ImageVideoActive,
                ImageOrVideo = question.ImageOrVideo,
                ImagePath = question.ImagePath,
                ImageName = question.ImageName,
                VideoUrl = question.VideoUrl,
                ShowCharts = question.ShowCharts,
                DescriptionTextActive = question.DescriptionTextActive,
                DescriptionText = question.DescriptionText,
                HtmlDescriptionText = question.HtmlDescriptionText,
                QuestionNumberIsHidden = question.QuestionNumberIsHidden,
                RespondingDuration = question.RespondingDuration,
                ImageBrightness = question.ImageBrightness,
                ImagePosition = question.ImagePosition,
                DesktopImageLayout = question.DesktopImageLayout,
                MobileImageLayout = question.MobileImageLayout,
                AnswerRequired = question.AnswerRequired,
                AllowMultipleSelect = question.AllowMultipleSelect,
                MaxSelectableChoices = question.MaxSelectableChoices,
                MinSelectableChoices = question.MinSelectableChoices,
                VerticalChoices = question.VerticalChoices,
                Randomize = question.Randomize,
                Steps = question.Steps,
                IconType = question.IconType,
                AnswerType = question.AnswerType,
                NumberMaxValue = question.NumberMaxValue,
                NumberMinValue = question.NumberMinValue,
                AnswerMaxLength = question.AnswerMaxLength,
                AnswerMinLength = question.AnswerMinLength,
                IsDecimal = question.IsDecimal,
                RegexType = question.RegexType,
                RegexValue = question.RegexValue,
                RegexPlaceholder = question.RegexPlaceholder,
                RegexValidationMessage = question.RegexValidationMessage,
                IsThousandsSeparationEnabled = question.IsThousandsSeparationEnabled
            };

            if (question.Choices != null)
            {
                entity.Choices = question.Choices.Select(c => new ChoiceEntity
                {
                    Id = c.Id,
                    QuestionId = question.Id,
                    Name = c.Name,
                    Hidden = c.Hidden,
                    AltName = c.AltName,
                    ChoiceType = c.ChoiceType
                }).ToList();
            }

            return entity;
        }

        private Question MapQuestionFromEntity(QuestionEntity entity)
        {
            return new Question
            {
                Id = entity.Id,
                Survey = entity.SurveyId,
                Title = entity.Title,
                HtmlTitle = entity.HtmlTitle,
                Type = entity.Type,
                ImageVideoActive = entity.ImageVideoActive,
                ImageOrVideo = entity.ImageOrVideo,
                ImagePath = entity.ImagePath,
                ImageName = entity.ImageName,
                VideoUrl = entity.VideoUrl,
                ShowCharts = entity.ShowCharts,
                DescriptionTextActive = entity.DescriptionTextActive,
                DescriptionText = entity.DescriptionText,
                HtmlDescriptionText = entity.HtmlDescriptionText,
                QuestionNumberIsHidden = entity.QuestionNumberIsHidden,
                RespondingDuration = entity.RespondingDuration,
                ImageBrightness = entity.ImageBrightness,
                ImagePosition = entity.ImagePosition,
                DesktopImageLayout = entity.DesktopImageLayout,
                MobileImageLayout = entity.MobileImageLayout,
                AnswerRequired = entity.AnswerRequired,
                AllowMultipleSelect = entity.AllowMultipleSelect,
                MaxSelectableChoices = entity.MaxSelectableChoices,
                MinSelectableChoices = entity.MinSelectableChoices,
                VerticalChoices = entity.VerticalChoices,
                Randomize = entity.Randomize,
                Steps = entity.Steps,
                IconType = entity.IconType,
                AnswerType = entity.AnswerType,
                NumberMaxValue = entity.NumberMaxValue,
                NumberMinValue = entity.NumberMinValue,
                AnswerMaxLength = entity.AnswerMaxLength,
                AnswerMinLength = entity.AnswerMinLength,
                IsDecimal = entity.IsDecimal,
                RegexType = entity.RegexType,
                RegexValue = entity.RegexValue,
                RegexPlaceholder = entity.RegexPlaceholder,
                RegexValidationMessage = entity.RegexValidationMessage,
                IsThousandsSeparationEnabled = entity.IsThousandsSeparationEnabled,
                Choices = entity.Choices.Select(c => new Choice
                {
                    Id = c.Id,
                    Name = c.Name,
                    Hidden = c.Hidden,
                    AltName = c.AltName,
                    ChoiceType = c.ChoiceType
                }).ToList()
            };
        }

        private WelcomePageEntity MapWelcomeToEntity(WelcomePage welcome, int surveyId)
        {
            return new WelcomePageEntity
            {
                Id = welcome.Id,
                SurveyId = surveyId,
                Title = welcome.Title,
                HtmlTitle = welcome.HtmlTitle,
                Type = welcome.Type,
                TitleActive = welcome.TitleActive,
                DescriptionActive = welcome.DescriptionActive,
                Description = welcome.Description,
                EnterText = welcome.EnterText,
                DescriptionText = welcome.DescriptionText
            };
        }

        private WelcomePage MapWelcomeFromEntity(WelcomePageEntity entity)
        {
            return new WelcomePage
            {
                Id = entity.Id,
                Survey = entity.SurveyId,
                Title = entity.Title,
                HtmlTitle = entity.HtmlTitle,
                Type = entity.Type,
                TitleActive = entity.TitleActive,
                DescriptionActive = entity.DescriptionActive,
                Description = entity.Description,
                EnterText = entity.EnterText,
                DescriptionText = entity.DescriptionText,
                ImageVideoActive = false,
                ImageOrVideo = 1,
                ImagePath = "",
                ImageName = "",
                VideoUrl = "",
                ShowCharts = true,
                QuestionNumberIsHidden = false,
                ImageBrightness = 100,
                ImagePosition = 5,
                DesktopImageLayout = 1,
                MobileImageLayout = 1
            };
        }

        private AppreciationPageEntity MapAppreciationToEntity(AppreciationPage appreciation, int surveyId)
        {
            return new AppreciationPageEntity
            {
                Id = appreciation.Id,
                SurveyId = surveyId,
                Title = appreciation.Title,
                HtmlTitle = appreciation.HtmlTitle,
                Type = appreciation.Type,
                ShareLinkActive = appreciation.ShareLinkActive,
                IsDefault = appreciation.IsDefault,
                LinkActive = appreciation.LinkActive,
                LinkButtonText = appreciation.LinkButtonText,
                LinkType = appreciation.LinkType,
                Link = appreciation.Link,
                ReloadActive = appreciation.ReloadActive,
                ReloadTime = appreciation.ReloadTime,
                DescriptionText = appreciation.DescriptionText
            };
        }

        private AppreciationPage MapAppreciationFromEntity(AppreciationPageEntity entity)
        {
            return new AppreciationPage
            {
                Id = entity.Id,
                Survey = entity.SurveyId,
                Title = entity.Title,
                HtmlTitle = entity.HtmlTitle,
                Type = entity.Type,
                ShareLinkActive = entity.ShareLinkActive,
                IsDefault = entity.IsDefault,
                LinkActive = entity.LinkActive,
                LinkButtonText = entity.LinkButtonText,
                LinkType = entity.LinkType,
                Link = entity.Link,
                ReloadActive = entity.ReloadActive,
                ReloadTime = entity.ReloadTime,
                DescriptionText = entity.DescriptionText,
                ImageVideoActive = false,
                ImageOrVideo = 1,
                ImagePath = "",
                ImageName = "",
                VideoUrl = "",
                ShowCharts = true,
                QuestionNumberIsHidden = false,
                ImageBrightness = 100,
                ImagePosition = 5,
                DesktopImageLayout = 1,
                MobileImageLayout = 1
            };
        }

        private int ParseLanguage(string language)
        {
            return language.ToLower() switch
            {
                "english" => 1,
                "persian" => 2,
                "farsi" => 2,
                "arabic" => 3,
                _ => 2
            };
        }

        private string GetLanguageName(int languageId)
        {
            return languageId switch
            {
                1 => "English",
                2 => "Persian",
                3 => "Arabic",
                _ => "Persian"
            };
        }
    }
}