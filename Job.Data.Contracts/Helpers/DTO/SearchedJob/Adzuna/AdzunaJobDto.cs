namespace Job.Data.Access.Helpers.JobDto.Adzuna;
public class AdzunaJobDto
{
    public string Id { get; set; }
    public AdzunaAreaDto Location { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Redirect_Url { get; set; }
    public string Created { get; set; }
    public AdzunaCategoryDto Category { get; set; }
    public AdzunaCompanyDto Company { get; set; }
    public string Contract_Time { get; set; }
}
