namespace Job.Data.Access.Helpers.JobEntities.Jobicy;
public class JobicyJobDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public string CompanyLogo { get; set; }
    public ICollection<string> JobIndustry { get; set; }
    public ICollection<string> JobType { get; set; }
    public string JobGeo { get; set; }
    public string JobLevel { get; set; }
    public string JobExcerpt { get; set; }
    public string JobDescription { get; set; }
    public string PubDate { get; set; }
}
