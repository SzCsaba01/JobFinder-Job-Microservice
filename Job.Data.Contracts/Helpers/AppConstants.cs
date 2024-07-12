namespace Job.Data.Contracts.Helpers;
public static class AppConstants
{
    public const string FE_APP_FEEDBACK_URL = "https://localhost:4200/home/feedback";
    public const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";
    public const string MICROSERVICE_URL = "https://localhost:5130";
    public const string USER_DELETE_MESSAGE = "DeleteUser";
    public const string RECOMMEND_JOBS_MESSAGE = "RecommendJobs";
    public static string REGION_API_URL = "api/Region/";
    public static string COUNTRY_API_URL = "api/Country/";
    public static string STATE_API_URL = "api/State/";
    public static string CITY_API_URL = "api/City/";
    public static string RESOURCES = "Resources";
    public static string COMPANY_LOGOS = "CompanyLogos";
    public static string MICROSERVICE_NAME = "Job.Microservice";
}
