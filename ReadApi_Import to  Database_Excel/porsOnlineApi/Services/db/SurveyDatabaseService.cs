
using Microsoft.EntityFrameworkCore;
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;
using System.Text.Json;
namespace porsOnlineApi.Services
{
    public class SurveyDatabaseService : ISurveyDatabaseService
    {
        private readonly SurveyDbContext _context;
        private readonly IMappingService _mappingService;
        private readonly ILogger<SurveyDatabaseService> _logger;

        public SurveyDatabaseService(
            SurveyDbContext context,
            IMappingService mappingService,
            ILogger<SurveyDatabaseService> logger)
        {
            _context = context;
            _mappingService = mappingService;
            _logger = logger;
        }

        public async Task<SurveyFolderCollection> GetAllSurveyFoldersAsync()
        {
            try
            {
                var entities = await _context.SurveyFolders
                    .Include(f => f.Surveys)
                    .ThenInclude(s => s.Theme)
                    .OrderBy(f => f.Order)
                    .ToListAsync();

                var folders = new SurveyFolderCollection();
                foreach (var entity in entities)
                {
                    folders.Add(_mappingService.MapFromEntity(entity));
                }

                return folders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey folders from database");
                throw;
            }
        }

        public async Task<SurveyFolder?> GetSurveyFolderByIdAsync(int folderId)
        {
            try
            {
                var entity = await _context.SurveyFolders
                    .Include(f => f.Surveys)
                    .ThenInclude(s => s.Theme)
                    .FirstOrDefaultAsync(f => f.Id == folderId);

                return entity != null ? _mappingService.MapFromEntity(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey folder {FolderId} from database", folderId);
                throw;
            }
        }

        public async Task<Survey?> GetSurveyByIdAsync(int surveyId)
        {
            try
            {
                var entity = await _context.Surveys
                    .Include(s => s.Theme)
                    .Include(s => s.Folder)
                    .FirstOrDefaultAsync(s => s.Id == surveyId);

                return entity != null ? _mappingService.MapFromEntity(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey {SurveyId} from database", surveyId);
                throw;
            }
        }

        public async Task<DetailedSurvey?> GetDetailedSurveyByIdAsync(int surveyId)
        {
            try
            {
                var entity = await _context.Surveys
                    .Include(s => s.Folder)
                    .Include(s => s.Theme)
                    .Include(s => s.Settings)
                    .Include(s => s.Questions)
                    .ThenInclude(q => q.Choices)
                    .Include(s => s.WelcomePages)
                    .Include(s => s.AppreciationPages)
                    .FirstOrDefaultAsync(s => s.Id == surveyId);

                return entity != null ? _mappingService.MapFromEntityToDetail(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving detailed survey {SurveyId} from database", surveyId);
                throw;
            }
        }

        public async Task<DetailedSurvey?> GetDetailedSurveyByPreviewCodeAsync(string previewCode)
        {
            try
            {
                var entity = await _context.Surveys
                    .Include(s => s.Folder)
                    .Include(s => s.Theme)
                    .Include(s => s.Settings)
                    .Include(s => s.Questions)
                    .ThenInclude(q => q.Choices)
                    .Include(s => s.WelcomePages)
                    .Include(s => s.AppreciationPages)
                    .FirstOrDefaultAsync(s => s.PreviewCode == previewCode || s.ReportCode == previewCode);

                return entity != null ? _mappingService.MapFromEntityToDetail(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey by preview code {PreviewCode} from database", previewCode);
                throw;
            }
        }

        public async Task<List<Survey>> GetActiveSurveysAsync()
        {
            try
            {
                var entities = await _context.Surveys
                    .Include(s => s.Theme)
                    .Include(s => s.Folder)
                    .Where(s => s.Active)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync();

                return entities.Select(_mappingService.MapFromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active surveys from database");
                throw;
            }
        }

        public async Task<List<Survey>> GetSurveysByFolderIdAsync(int folderId)
        {
            try
            {
                var entities = await _context.Surveys
                    .Include(s => s.Theme)
                    .Include(s => s.Folder)
                    .Where(s => s.FolderId == folderId)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync();

                return entities.Select(_mappingService.MapFromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving surveys for folder {FolderId} from database", folderId);
                throw;
            }
        }

        public async Task<List<Survey>> GetSurveysByLabelAsync(string label)
        {
            try
            {
                var entities = await _context.Surveys
                    .Include(s => s.Theme)
                    .Include(s => s.Folder)
                    .Where(s => s.LabelsJson != null && s.LabelsJson.Contains($"\"{label}\""))
                    .ToListAsync();

                return entities.Select(_mappingService.MapFromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving surveys with label {Label} from database", label);
                throw;
            }
        }

        public async Task<List<Survey>> SearchSurveysAsync(string searchTerm)
        {
            try
            {
                var entities = await _context.Surveys
                    .Include(s => s.Theme)
                    .Include(s => s.Folder)
                    .Where(s => s.Name.Contains(searchTerm) ||
                               s.PreviewCode.Contains(searchTerm) ||
                               s.ReportCode.Contains(searchTerm))
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync();

                return entities.Select(_mappingService.MapFromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching surveys with term {SearchTerm} from database", searchTerm);
                throw;
            }
        }

        public async Task<int> SaveSurveyFolderAsync(SurveyFolder folder)
        {
            try
            {
                var entity = _mappingService.MapToEntity(folder);
                var existing = await _context.SurveyFolders.AsNoTracking()
                               .FirstOrDefaultAsync(s => s.Id == entity.Id);
                if (existing == null)
                {
                    _context.SurveyFolders.Add(entity);
                }
                else
                {
                    entity.UpdatedDate = DateTime.UtcNow;
                    _context.Entry(existing).CurrentValues.SetValues(entity);
                }               

                await _context.SaveChangesAsync();
                _logger.LogInformation("Survey folder {FolderId} saved successfully", entity.Id);

                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving survey folder to database");
                throw;
            }
        }


        public async Task<int> SaveSurveyAsync(Survey survey)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Saving survey {SurveyId}: {Name}", survey.Id, survey.Name);

                // پاک کردن تمام tracking براي جلوگيري از conflict
                _context.ChangeTracker.Clear();

                // بررسي اينکه survey وجود دارد يا نه
                var existingSurvey = await _context.Surveys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == survey.Id);

                if (existingSurvey == null)
                {
                    // ايجاد survey جديد
                    await CreateNewSurveyAsync(survey);
                }
                else
                {
                    // به‌روزرساني survey موجود
                    await UpdateExistingSurveyAsync(survey);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Survey {SurveyId} saved successfully", survey.Id);
                return survey.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving survey {SurveyId} to database", survey.Id);

                // لاگ اضافي براي مشکلات constraint
                if (ex.Message.Contains("PRIMARY KEY constraint"))
                {
                    await LogConstraintConflictAsync(survey.Id);
                }

                throw;
            }
        }
        private async Task CreateNewSurveyAsync(Survey survey)
        {
            var entity = new SurveyEntity
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
                LabelsJson = survey.Labels?.Any() == true ? JsonSerializer.Serialize(survey.Labels) : null,
                TagsJson = survey.Tags?.Any() == true ? JsonSerializer.Serialize(survey.Tags) : null
            };

            _context.Surveys.Add(entity);
            _logger.LogDebug("Created new survey {SurveyId}", survey.Id);
        }

        private async Task UpdateExistingSurveyAsync(Survey survey)
        {
            // بارگذاری survey موجود برای به‌روزرسانی
            var existingEntity = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingEntity != null)
            {
                // به‌روزرسانی فقط خصوصیات اصلی
                existingEntity.Name = survey.Name;
                existingEntity.FolderId = survey.FolderId;
                existingEntity.Active = survey.Active;
                existingEntity.CanActive = survey.CanActive;
                existingEntity.IsStopped = survey.IsStopped;
                existingEntity.Views = survey.Views;
                existingEntity.SubmittedResponses = survey.SubmittedResponses;
                existingEntity.UrlSlug = survey.UrlSlug;
                existingEntity.IsTemplate = survey.IsTemplate;
                existingEntity.HasQuestion = survey.HasQuestion;
                existingEntity.Subdomain = survey.Subdomain;
                existingEntity.Domain = survey.Domain;
                existingEntity.LastResponseDateTime = survey.LastResponseDateTime;
                existingEntity.LabelsJson = survey.Labels?.Any() == true ? JsonSerializer.Serialize(survey.Labels) : null;
                existingEntity.TagsJson = survey.Tags?.Any() == true ? JsonSerializer.Serialize(survey.Tags) : null;
                _logger.LogDebug("Updated existing survey {SurveyId}", survey.Id);
            }
        }
        private async Task LogConstraintConflictAsync(int surveyId)
        {
            try
            {
                // بررسی رکوردهای موجود برای debug
                var existingSettings = await _context.SurveySettings
                    .AsNoTracking()
                    .Where(s => s.SurveyId == surveyId)
                    .ToListAsync();

                var existingThemes = await _context.SurveyThemes
                    .AsNoTracking()
                    .Where(t => t.SurveyId == surveyId)
                    .ToListAsync();

                _logger.LogWarning("Constraint conflict for survey {SurveyId}", surveyId);
                _logger.LogWarning("Existing settings count: {Count}", existingSettings.Count);
                foreach (var setting in existingSettings)
                {
                    _logger.LogWarning("Settings - ID: {Id}, SurveyId: {SurveyId}", setting.Id, setting.SurveyId);
                }

                _logger.LogWarning("Existing themes count: {Count}", existingThemes.Count);
                foreach (var theme in existingThemes)
                {
                    _logger.LogWarning("Theme - ID: {Id}, SurveyId: {SurveyId}", theme.Id, theme.SurveyId);
                }

                // بررسی رکوردهای مشکل‌دار
                var problemSettings = await _context.SurveySettings
                    .AsNoTracking()
                    .Where(s => s.Id == 0)
                    .ToListAsync();

                var problemThemes = await _context.SurveyThemes
                    .AsNoTracking()
                    .Where(t => t.Id == 14)
                    .ToListAsync();

                _logger.LogError("Problem settings (ID=0): {Count}", problemSettings.Count);
                _logger.LogError("Problem themes (ID=14): {Count}", problemThemes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging constraint conflict details");
            }
        }
        public async Task CleanupProblematicRecordsAsync()
        {
            try
            {
                _logger.LogInformation("Cleaning up problematic records...");

                // پاک کردن settings با ID = 0
                var problematicSettings = await _context.SurveySettings
                    .Where(s => s.Id == 0)
                    .ToListAsync();

                if (problematicSettings.Any())
                {
                    _context.SurveySettings.RemoveRange(problematicSettings);
                    _logger.LogWarning("Removed {Count} problematic settings records", problematicSettings.Count);
                }

                // پاک کردن themes با ID = 14 که مشکل دارند
                var problematicThemes = await _context.SurveyThemes
                    .Where(t => t.Id == 14)
                    .ToListAsync();

                if (problematicThemes.Any())
                {
                    _context.SurveyThemes.RemoveRange(problematicThemes);
                    _logger.LogWarning("Removed {Count} problematic theme records", problematicThemes.Count);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleanup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup");
                throw;
            }
        }

        public async Task<int> SaveDetailedSurveyAsync(DetailedSurvey survey, int? surveyId = null)
        {
            var id = surveyId ?? 0;
            if (id == 0)
            {
                throw new ArgumentException("Survey ID must be provided for detailed survey");
            }

            // ابتدا survey اصلی را ذخیره کن
            var basicSurvey = new Survey
            {
                Id = id,
                Name = survey.Name,
                FolderId = survey.Folder?.Id ?? 1,
                Active = survey.Active,                
                SubmittedResponses = survey.SubmittedResponses,
                PreviewCode = survey.PreviewCode,
                ReportCode = survey.ReportCode,
                UrlSlug = survey.UrlSlug,                
                Subdomain = survey.Subdomain,
                Domain = survey.Domain,                
                Labels = survey.Labels                
            };
            await SaveSurveyAsync(basicSurvey);
            return id;
        }
        public async Task<int> SaveDetailedSurveyAsync_old(DetailedSurvey survey, int? surveyId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var id = surveyId ?? 0;
                if (id == 0)
                {
                    throw new ArgumentException("Survey ID must be provided for detailed survey");
                }

                // ?? Clear change tracker to avoid conflicts
                _context.ChangeTracker.Clear();

                // Check if survey exists
                var exists = await _context.Surveys.AsNoTracking().AnyAsync(s => s.Id == id);

                if (!exists)
                {
                    // Create new survey
                    var newEntity = _mappingService.MapToEntity(survey, id);
                    _context.Surveys.Add(newEntity);
                }
                else
                {
                    // Update existing survey
                    await UpdateExistingSurveyAsync(survey, id);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Detailed survey {SurveyId} saved successfully", id);
                return id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving detailed survey {SurveyId}", surveyId);
                throw;
            }
        }
        private async Task UpdateExistingSurveyAsync(DetailedSurvey survey, int surveyId)
        {
            var existingEntity = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (existingEntity != null)
            {
                // به‌روزرساني فقط خصوصيات اصلي
                existingEntity.Name = survey.Name;
                existingEntity.FolderId = survey.Folder.Id;
                existingEntity.Active = survey.Active;
                existingEntity.SubmittedResponses = survey.SubmittedResponses;
                existingEntity.UrlSlug = survey.UrlSlug;
                existingEntity.Subdomain = survey.Subdomain;
                existingEntity.Domain = survey.Domain;
                existingEntity.LabelsJson = survey.Labels?.Any() == true ? JsonSerializer.Serialize(survey.Labels) : null;

            }
        }        
        private void UpdateSurveyProperties(SurveyEntity existing, DetailedSurvey survey)
        {
            existing.Name = survey.Name;
            existing.FolderId = survey.Folder?.Id ?? existing.FolderId; // Use existing if null
            existing.Active = survey.Active;
            //existing.CanActive = survey.CanActive;
            //existing.IsStopped = survey.IsStopped;
            //existing.Views = survey.Views;
            existing.SubmittedResponses = survey.SubmittedResponses;
            existing.PreviewCode = survey.PreviewCode;
            existing.ReportCode = survey.ReportCode;
            existing.UrlSlug = survey.UrlSlug;
            //existing.IsTemplate = survey.IsTemplate;
            //existing.HasQuestion = survey.HasQuestion;
            existing.Subdomain = survey.Subdomain;
            existing.Domain = survey.Domain;            
            existing.LabelsJson = survey.Labels?.Any() == true ? JsonSerializer.Serialize(survey.Labels) : null;            
        }
        private async Task HandleSurveyThemeAsync(SurveyEntity existing, DetailedSurvey survey, int surveyId)
        {
            // Remove existing theme relationship
            if (existing.Theme != null)
            {
                _context.SurveyThemes.Remove(existing.Theme);
            }

            if (survey.Theme != null)
            {
                // Create new theme entity
                var themeEntity = new SurveyThemeEntity
                {
                    Id = survey.Theme.Id,
                    SurveyId = surveyId,
                    Order = survey.Theme.Order,
                    BackgroundColor = survey.Theme.BackgroundColor,
                    QuestionColor = survey.Theme.QuestionColor,
                    AnswerColor = survey.Theme.AnswerColor,
                    ButtonColor = survey.Theme.ButtonColor,
                    AccentColor = survey.Theme.AccentColor,
                    FontFamily = survey.Theme.FontFamily,
                    FontSize = survey.Theme.FontSize,
                    IsPublic = survey.Theme.IsPublic,
                    ThumbnailUrl = survey.Theme.ThumbnailUrl,
                    BackgroundImageUrl = survey.Theme.BackgroundImage?.ThumbnailUrl,
                    BackgroundImageRepeat = survey.Theme.BackgroundImageRepeat,
                    BackgroundImageBrightness = survey.Theme.BackgroundImageBrightness,
                    BackgroundImageFit = survey.Theme.BackgroundImageFit,
                    BackgroundImagePosition = survey.Theme.BackgroundImagePosition,
                    BackgroundImageSizePercentage = survey.Theme.BackgroundImageSizePercentage
                };

                _context.SurveyThemes.Add(themeEntity);
            }
        }
        private async Task HandleSurveySettingsAsync(SurveyEntity existing, DetailedSurvey survey, int surveyId)
        {
            // Remove existing settings
            if (existing.Settings != null)
            {
                _context.SurveySettings.Remove(existing.Settings);
            }

            if (survey.Settings != null)
            {
                var settingsEntity = new SurveySettingsEntity
                {
                    SurveyId = surveyId,
                    AuthenticationNeeded = survey.Settings.AuthenticationNeeded,
                    PorslineAuth = survey.Settings.PorslineAuth,
                    CodeAuth = survey.Settings.CodeAuth,
                    PhoneNumberAuth = survey.Settings.PhoneNumberAuth,
                    EditResponseEnabled = survey.Settings.EditResponseEnabled,
                    ShowAnswerSheetEnabled = survey.Settings.ShowAnswerSheetEnabled,
                    ShowAnswerSheetToResponder = survey.Settings.ShowAnswerSheetToResponder,
                    ShowAnswerKeyAfterResponseSubmit = survey.Settings.ShowAnswerKeyAfterResponseSubmit,
                    ShowAnswerKeyAfterSurveyStop = survey.Settings.ShowAnswerKeyAfterSurveyStop,
                    RespondingDurationType = survey.Settings.RespondingDurationType,
                    QuestionsRespondingDuration = survey.Settings.QuestionsRespondingDuration,
                    LocalStorageIsEnabled = survey.Settings.LocalStorageIsEnabled,
                    NoIndex = survey.Settings.NoIndex
                };

                _context.SurveySettings.Add(settingsEntity);
            }
        }
        private async Task HandleQuestionsAsync(SurveyEntity existing, DetailedSurvey survey, int surveyId)
        {
            // Remove all existing questions and their choices
            if (existing.Questions.Any())
            {
                foreach (var existingQuestion in existing.Questions.ToList())
                {
                    _context.Choices.RemoveRange(existingQuestion.Choices);
                }
                _context.Questions.RemoveRange(existing.Questions);
            }

            // Add new questions
            foreach (var question in survey.Questions)
            {
                var dd = question;
                var questionEntity = new QuestionEntity
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

                _context.Questions.Add(questionEntity);
                if (dd.Choices!=null)
                {
                    // Add choices for this question
                    foreach (var choice in dd.Choices)
                    {
                        var choiceEntity = new ChoiceEntity
                        {
                            Id = choice.Id,
                            QuestionId = questionEntity.Id,
                            Name = choice.Name,
                            Hidden = choice.Hidden,
                            AltName = choice.AltName,
                            ChoiceType = choice.ChoiceType
                        };

                        _context.Choices.Add(choiceEntity);
                    }
                }
            }
        }
        private async Task HandleWelcomePagesAsync(SurveyEntity existing, DetailedSurvey survey, int surveyId)
        {
            // Remove existing welcome pages
            if (existing.WelcomePages.Any())
            {
                _context.WelcomePages.RemoveRange(existing.WelcomePages);
            }

            // Add new welcome pages
            foreach (var welcomePage in survey.Welcome)
            {
                var entity = new WelcomePageEntity
                {
                    Id = welcomePage.Id,
                    SurveyId = surveyId,
                    Title = welcomePage.Title,
                    HtmlTitle = welcomePage.HtmlTitle,
                    Type = welcomePage.Type,
                    TitleActive = welcomePage.TitleActive,
                    DescriptionActive = welcomePage.DescriptionActive,
                    Description = welcomePage.Description,
                    EnterText = welcomePage.EnterText,
                    DescriptionText = welcomePage.DescriptionText
                };

                _context.WelcomePages.Add(entity);
            }
        }               
        private SurveyEntity CreateSurveyEntityFromDetailedSurvey(DetailedSurvey survey, int surveyId)
        {
            var entity = new SurveyEntity
            {
                Id = surveyId,
                Name = survey.Name,
                FolderId = survey.Folder?.Id ?? 1, // Default folder if null
                Language = 0, // Default value
                CreatedDate = DateTime.UtcNow,
                Active = survey.Active,
                SubmittedResponses = survey.SubmittedResponses,
                PreviewCode = survey.PreviewCode,
                ReportCode = survey.ReportCode,
                UrlSlug = survey.UrlSlug,
                Subdomain = survey.Subdomain,
                Domain = survey.Domain,                
                LabelsJson = survey.Labels?.Any() == true ? JsonSerializer.Serialize(survey.Labels) : null                
            };

            // Add theme if exists
            if (survey.Theme != null)
            {
                entity.Theme = new SurveyThemeEntity
                {
                    Id = survey.Theme.Id,
                    SurveyId = surveyId,
                    Order = survey.Theme.Order,
                    BackgroundColor = survey.Theme.BackgroundColor,
                    QuestionColor = survey.Theme.QuestionColor,
                    AnswerColor = survey.Theme.AnswerColor,
                    ButtonColor = survey.Theme.ButtonColor,
                    AccentColor = survey.Theme.AccentColor,
                    FontFamily = survey.Theme.FontFamily,
                    FontSize = survey.Theme.FontSize,
                    IsPublic = survey.Theme.IsPublic,
                    ThumbnailUrl = survey.Theme.ThumbnailUrl,
                    BackgroundImageUrl = survey.Theme.BackgroundImage.ThumbnailUrl,
                    BackgroundImageRepeat = survey.Theme.BackgroundImageRepeat,
                    BackgroundImageBrightness = survey.Theme.BackgroundImageBrightness,
                    BackgroundImageFit = survey.Theme.BackgroundImageFit,
                    BackgroundImagePosition = survey.Theme.BackgroundImagePosition,
                    BackgroundImageSizePercentage = survey.Theme.BackgroundImageSizePercentage
                };
            }

            // Add settings if exists
            if (survey.Settings != null)
            {
                entity.Settings = new SurveySettingsEntity
                {
                    SurveyId = surveyId,
                    AuthenticationNeeded = survey.Settings.AuthenticationNeeded,
                    PorslineAuth = survey.Settings.PorslineAuth,
                    CodeAuth = survey.Settings.CodeAuth,
                    PhoneNumberAuth = survey.Settings.PhoneNumberAuth,
                    EditResponseEnabled = survey.Settings.EditResponseEnabled,
                    ShowAnswerSheetEnabled = survey.Settings.ShowAnswerSheetEnabled,
                    ShowAnswerSheetToResponder = survey.Settings.ShowAnswerSheetToResponder,
                    ShowAnswerKeyAfterResponseSubmit = survey.Settings.ShowAnswerKeyAfterResponseSubmit,
                    ShowAnswerKeyAfterSurveyStop = survey.Settings.ShowAnswerKeyAfterSurveyStop,
                    RespondingDurationType = survey.Settings.RespondingDurationType,
                    QuestionsRespondingDuration = survey.Settings.QuestionsRespondingDuration,
                    LocalStorageIsEnabled = survey.Settings.LocalStorageIsEnabled,
                    NoIndex = survey.Settings.NoIndex
                };
            }

            // Add questions
            foreach (var question in survey.Questions)
            {
                var questionEntity = new QuestionEntity
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

                // Add choices for this question
                foreach (var choice in question.Choices)
                {
                    questionEntity.Choices.Add(new ChoiceEntity
                    {
                        Id = choice.Id,
                        QuestionId = question.Id,
                        Name = choice.Name,
                        Hidden = choice.Hidden,
                        AltName = choice.AltName,
                        ChoiceType = choice.ChoiceType
                    });
                }

                entity.Questions.Add(questionEntity);
            }

            // Add welcome pages
            foreach (var welcomePage in survey.Welcome)
            {
                entity.WelcomePages.Add(new WelcomePageEntity
                {
                    Id = welcomePage.Id,
                    SurveyId = surveyId,
                    Title = welcomePage.Title,
                    HtmlTitle = welcomePage.HtmlTitle,
                    Type = welcomePage.Type,
                    TitleActive = welcomePage.TitleActive,
                    DescriptionActive = welcomePage.DescriptionActive,
                    Description = welcomePage.Description,
                    EnterText = welcomePage.EnterText,
                    DescriptionText = welcomePage.DescriptionText
                });
            }

            return entity;
        }
        private async Task DeleteExistingRelatedDataAsync(int surveyId)
        {
            // Delete in correct order (children first)
            var choices = await _context.Choices
                .Where(c => c.Question.SurveyId == surveyId)
                .ToListAsync();
            _context.Choices.RemoveRange(choices);

            var questions = await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .ToListAsync();
            _context.Questions.RemoveRange(questions);

            var welcomePages = await _context.WelcomePages
                .Where(w => w.SurveyId == surveyId)
                .ToListAsync();
            _context.WelcomePages.RemoveRange(welcomePages);

            var theme = await _context.SurveyThemes
                .FirstOrDefaultAsync(t => t.SurveyId == surveyId);
            if (theme != null)
            {
                _context.SurveyThemes.Remove(theme);
            }

            var settings = await _context.SurveySettings
                .FirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (settings != null)
            {
                _context.SurveySettings.Remove(settings);
            }
        }
           
        public async Task<bool> DeleteSurveyAsync(int surveyId)
        {
            try
            {
                var entity = await _context.Surveys.FindAsync(surveyId);
                if (entity == null) return false;

                _context.Surveys.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Survey {SurveyId} deleted successfully", surveyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey {SurveyId} from database", surveyId);
                throw;
            }
        }

        public async Task<bool> DeleteSurveyFolderAsync(int folderId)
        {
            try
            {
                var entity = await _context.SurveyFolders.FindAsync(folderId);
                if (entity == null) return false;

                _context.SurveyFolders.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Survey folder {FolderId} deleted successfully", folderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey folder {FolderId} from database", folderId);
                throw;
            }
        }

        public async Task<int> ImportSurveyFoldersFromJsonAsync(string jsonData)
        {
            try
            {
                var folders = JsonSerializer.Deserialize<SurveyFolderCollection>(jsonData);
                if (folders == null) return 0;

                int importedCount = 0;
                foreach (var folder in folders)
                {
                    await SaveSurveyFolderAsync(folder);
                    importedCount++;
                }

                _logger.LogInformation("Imported {Count} survey folders from JSON", importedCount);
                return importedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing survey folders from JSON");
                throw;
            }
        }

        public async Task<int> ImportDetailedSurveyFromJsonAsync(string jsonData)
        {
            try
            {
                var survey = JsonSerializer.Deserialize<DetailedSurvey>(jsonData);
                if (survey == null) return 0;

                var surveyId = await SaveDetailedSurveyAsync(survey);
                _logger.LogInformation("Imported detailed survey {SurveyId} from JSON", surveyId);

                return surveyId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing detailed survey from JSON");
                throw;
            }
        }
    }
}
