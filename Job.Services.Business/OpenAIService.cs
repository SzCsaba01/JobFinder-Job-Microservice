using HtmlAgilityPack;
using Job.Data.Contracts.Helpers;
using Job.Data.Contracts.Helpers.DTO.Recommendation;
using Job.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Job.Services.Business;
public class OpenAIService : IOpenAIService
{
    private readonly IConfiguration _config;

    public OpenAIService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> RecommendJobsAsync(DetailsForJobRecommendationsDto detailsForJobRecommendations)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            string prompt = GeneratePrompt(detailsForJobRecommendations);

            var requestBody = new
            {
                //gpt-4o
                //"gpt-3.5-turbo-0125"
                model = "gpt-3.5-turbo-0125",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 500,
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(AppConstants.OPENAI_API_URL, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get recommended jobs!");
            }
            
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            return jsonResponse.choices[0].message.content.ToString();
        }
    }

    public string GeneratePrompt(DetailsForJobRecommendationsDto detailsForJobRecommendations)
    {
        StringBuilder prompt = new StringBuilder();
        prompt.AppendLine("User Profile:");
        prompt.AppendLine($"Country: {detailsForJobRecommendations.UserMessageDetails.Country}, " +
                            $"State: {detailsForJobRecommendations.UserMessageDetails.State}, " +
                            $"City: {detailsForJobRecommendations.UserMessageDetails.City}");
        prompt.AppendLine($"Education: {detailsForJobRecommendations.UserMessageDetails.Education}, " +
                            $"Experience: {detailsForJobRecommendations.UserMessageDetails.Experience}");

        if (detailsForJobRecommendations.UserMessageDetails.Skills != null && detailsForJobRecommendations.UserMessageDetails.Skills.Any())
        {
            prompt.AppendLine($"Skills: {string.Join(", ", detailsForJobRecommendations.UserMessageDetails.Skills)}");
        }

        if (detailsForJobRecommendations.SavedJobIds != null && detailsForJobRecommendations.SavedJobIds.Any())
        {
            prompt.AppendLine($"Saved Job Ids: {string.Join(", ", detailsForJobRecommendations.SavedJobIds)}");
        }

        if (detailsForJobRecommendations.ExternalSourceVisitIds != null && detailsForJobRecommendations.ExternalSourceVisitIds.Any())
        {
            prompt.AppendLine($"Applied Job Ids: {string.Join(", ", detailsForJobRecommendations.ExternalSourceVisitIds)}");
        }

        if (detailsForJobRecommendations.RecommendedJobIds != null && detailsForJobRecommendations.RecommendedJobIds.Any())
        {
            prompt.AppendLine($"Already Recommended Job Ids: {string.Join(", ", detailsForJobRecommendations.RecommendedJobIds)}");
        }

        prompt.AppendLine("Jobs:");
        foreach (var job in detailsForJobRecommendations.Jobs)
        {
            prompt.AppendLine($"Id: {job.Id}, Title: {job.Title}");
            prompt.AppendLine($"Company: {job.Company.Name}, Rating: {job.Company.Rating}");
            if (job.Locations != null && job.Locations.Any())
            {
                foreach (var location in job.Locations)
                {
                    prompt.Append("Location: ");
                    if (!string.IsNullOrEmpty(location.Country)) prompt.Append($"Country: {location.Country}, ");
                    if (!string.IsNullOrEmpty(location.State)) prompt.Append($"State: {location.State}, ");
                    if (!string.IsNullOrEmpty(location.City)) prompt.Append($"City: {location.City}");
                    prompt.AppendLine();
                }
            }
            prompt.AppendLine($"Remote: {job.isRemote}");

            if (job.ContractTypeName != null)
            {
                prompt.AppendLine($"Contract Type: {job.ContractTypeName}");
            }

            if (job.Categories != null && job.Categories.Any())
            {
                prompt.AppendLine($"Categories: {string.Join(", ", job.Categories)}");
            }

            if (job.Tags != null && job.Tags.Any())
            {
                prompt.AppendLine($"Tags: {string.Join(", ", job.Tags)}");
            }
        }

        prompt.AppendLine("Please provide job recommendations and return only the top 5 recommended job ids for the user exactly in the following format:");
        prompt.AppendLine("[Recommended Job Ids: id1, id2, id3, ...]");
        //prompt.AppendLine("Example: [Recommended Job Ids: 123, 456, 789]");

        return prompt.ToString();
    }

    private string ConvertHtmlToPlainText(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        string text = doc.DocumentNode.InnerText;

        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }
}
