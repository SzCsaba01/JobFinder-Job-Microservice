using Job.Data.Access.Helpers.JobEntities.Jobicy;

namespace Job.Data.Access.Helpers.JobDto.Jobicy;
public class JobicyJobListDto
{
    public ICollection<JobicyJobDto> Jobs { get; set; }
}
