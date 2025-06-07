using Microsoft.EntityFrameworkCore;
using porsOnlineApi.Models;
using porsOnlineApi.Services;
using porsOnlineApi.Services.Api;

namespace porsOnlineApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDb_InjectionnServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<SurveyDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging() //
                );
            

            // Services
            services.AddScoped<IMappingService, MappingService>();
            services.AddScoped<ISurveyDatabaseService, SurveyDatabaseService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();
            services.AddScoped<ISurveyManagementService, SurveyManagementService>();
            services.AddScoped<IApiClientService, ApiClientService>();
            services.AddScoped<ISyncManagementService, SyncManagementService>();


            return services;
        }
    }
}
