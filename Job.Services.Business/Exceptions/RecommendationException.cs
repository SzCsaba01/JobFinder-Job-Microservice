using Project.Services.Business.Exceptions;
using System.Net;

namespace Job.Services.Business.Exceptions;
public class RecommendationException : ApiExceptionBase
{
    public RecommendationException() : base(HttpStatusCode.PreconditionFailed, "Recommendation Exception") {}

    public RecommendationException(string message) : base(HttpStatusCode.PreconditionFailed, message, "Recommendation Exception") {}

    public RecommendationException(string message, Exception innerException) : base(HttpStatusCode.PreconditionFailed, message, innerException, "Recommendation Exception") {}
}
