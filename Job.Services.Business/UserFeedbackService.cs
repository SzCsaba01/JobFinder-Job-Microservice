using AutoMapper;
using Job.Data.Contracts;
using Job.Data.Contracts.Helpers.DTO.Feedback;
using Job.Services.Contracts;
using Project.Services.Business.Exceptions;

namespace Job.Services.Business;
public class UserFeedbackService : IUserFeedbackService
{
    private readonly IUserFeedbackRepository _userFeedbackRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public UserFeedbackService
        (
            IUserFeedbackRepository userFeedbackRepository, 
            IMapper mapper, 
            ICompanyRepository companyRepository
        )
    {
        _userFeedbackRepository = userFeedbackRepository;
        _companyRepository = companyRepository;

        _mapper = mapper;
        
    }

    public async Task<UserFeedbackFilterResultDto> GetFilteredFeedbacksAsync(UserFeedbackFilterDto userFeedbackFilter)
    {
        var feedbacks = await _userFeedbackRepository.GetFilteredFeedbacksAsync(userFeedbackFilter);

        if (feedbacks == null)
        {
            throw new ModelNotFoundException("Feedbacks not found!");
        }

        var categories = new List<string>();
        var companies = new List<string>();
        var contractTypes = new List<string>();
        var jobDetails = new List<UserFeedbackJobDetailsDto>();

        feedbacks.ForEach(x =>
        {
            jobDetails.Add(_mapper.Map<UserFeedbackJobDetailsDto>(x.Job));
            categories.AddRange(x.Job.Categories.Where(y => y.CategoryName != null && y.CategoryName != string.Empty).Select(x => x.CategoryName).ToList());
            companies.Add(x.Job.CompanyName);
            if (x.Job.ContractType != null && x.Job.ContractType.Name != null && x.Job.ContractType.Name != string.Empty)
            {
                contractTypes.Add(x.Job.ContractType.Name);
            }
        });

        var userFeedbackFilterResult = new UserFeedbackFilterResultDto
        {
            Categories = categories.Distinct().ToList(),
            Companies = companies.Distinct().ToList(),
            ContractTypes = contractTypes.Distinct().ToList(),
            Feedbacks = _mapper.Map<List<UserFeedbackDto>>(feedbacks),
            Jobs = jobDetails.DistinctBy(x => x.JobId).ToList(),
        };

        return userFeedbackFilterResult;
    }

    public async Task<UserFeedbackDto> GetFeedbackByTokenAndUserProfileIdAsync(string token, Guid userProfileId)
    {
        var userFeedback = await _userFeedbackRepository.GetUserFeedbackByTokenAsync(token, userProfileId);

        if (userFeedback == null)
        {
            throw new ModelNotFoundException("Feedback not found!");
        }

        return _mapper.Map<UserFeedbackDto>(userFeedback);
    }

    public async Task UpdateFeedbackAsync(UserFeedbackDto feedback, Guid userProfileId)
    {
        var userFeedback = await _userFeedbackRepository.GetUserFeedbackByTokenAsync(feedback.Token, userProfileId);

        if (userFeedback == null)
        {
            throw new ModelNotFoundException("Feedback not found!");
        }

        var company = await _companyRepository.GetCompanyByNameAsync(feedback.Company.Name);

        if (company == null)
        {
            throw new ModelNotFoundException("Company not found!");
        }
        
        company.NumberOfRatings++;
        company.TotalRating = company.TotalRating + feedback.CompanyRating;
        company.Rating = company.TotalRating / company.NumberOfRatings;

        await _companyRepository.UpdateCompanyAsync(company);

        userFeedback.CompanyRating = feedback.CompanyRating;
        userFeedback.Feedback = feedback.Feedback;
        userFeedback.FeedbackDate = DateTime.Now;
        userFeedback.ApplicationStatus = feedback.ApplicationStatus;
        userFeedback.Job = null;

        await _userFeedbackRepository.UpdateUserFeedbackAsync(userFeedback);
    }
}
