using Job.Data.Access;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers;
using Job.Services.Business;
using Job.Services.Contracts;
using Job.Services.Quartz;
using Quartz;

namespace Job.Microservice.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IContractTypeRepository, ContractTypeRepository>();
        services.AddScoped<IExternalSourceVisitClickRepository, ExternalSourceVisitClickRepository>();
        services.AddScoped<IJobRecommendationMappingRepository, JobRecommendationMappingRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IRecommendationRepository, RecommendationRepository>();
        services.AddScoped<ISavedJobRepository, SavedJobRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IContractTypeService, ContractTypeService>();
        services.AddScoped<IExternalSourceVisitClickService, ExternalSourceVisitClickService>();
        services.AddScoped<IJobRecommendationService, JobRecommendationService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISavedJobService, SavedJobService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IUserFeedbackService, UserFeedbackService>();
        services.AddScoped<IJobAPIService, JobAPIService>();
        services.AddScoped<IOpenAIService, OpenAIService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IQueueMessageProcesserService, QueueMessageProcesserService>();
        services.AddScoped<IQueueMessageSenderService, QueueMessageSenderService>();

        services.AddHostedService<QueueMessageReceiverService>();

        string locationMicroserviceURL = Environment.GetEnvironmentVariable("LOCATION_MICROSERVICE_URL") ?? "http://localhost:5204";
        services.AddHttpClient<ILocationCommunicationService, LocationCommunicationService>(client => {
            client.BaseAddress = new Uri(locationMicroserviceURL);
        });

        services.AddAutoMapper(typeof(Mapper));

        services.AddQuartz(q =>
        {
            var addJobsFromAPIs = new JobKey("AddJobsFromAPIsJob");
            var jobsDeactivation = new JobKey("JobsDeactivationJob");
            var sendUserFeedbackEmails = new JobKey("SendUserFeedbackEmailsJob");

            q.AddJob<AddJobsFromAPIsJob>(j => j.WithIdentity(addJobsFromAPIs));
            q.AddJob<JobsDeactivationJob>(j => j.WithIdentity(jobsDeactivation));
            q.AddJob<SendUserFeedbackEmailsJob>(j => j.WithIdentity(sendUserFeedbackEmails));

            q.AddTrigger(t => t
                .ForJob(addJobsFromAPIs)
                .WithIdentity("AddJobsFromAPIsTrigger")
                .WithCronSchedule("0 0 6 ? * MON"));


            q.AddTrigger(t => t
                .ForJob(jobsDeactivation)
                .WithIdentity("JobsDeactivationTrigger")
                .WithCronSchedule("0 0 6 ? * *"));

            q.AddTrigger(t => t
                .ForJob(sendUserFeedbackEmails)
                .WithIdentity("SendUserFeedbackEmailsTrigger")
                .WithCronSchedule("0 0 6 ? * *"));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        services.AddCors(options => options.AddPolicy(
            name: "NgOrigins",
            policy => {
                policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));

        return services;
    }
}
