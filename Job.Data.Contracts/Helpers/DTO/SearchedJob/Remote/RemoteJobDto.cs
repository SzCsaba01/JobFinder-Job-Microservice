namespace Job.Data.Access.Helpers.JobEntities.Remote;
public class RemoteJobDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Company_Name { get; set; }
    public string Company_Logo { get; set; }
    public string Category { get; set; }
    public ICollection<string> Tags { get; set; }
    public string Job_Type { get; set; }
    public string Publication_Date { get; set; }
    public string Candidate_Required_Location { get; set; }
    public string Description { get; set; }
}
