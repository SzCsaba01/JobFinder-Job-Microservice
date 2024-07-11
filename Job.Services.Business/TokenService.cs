using Job.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Job.Services.Business;
public class TokenService : ITokenService
{
    public async Task<string> GenerateRandomTokenAsync()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32];
            rng.GetBytes(randomBytes);

            var base64UrlToken = Base64UrlEncoder.Encode(randomBytes);

            return await Task.FromResult(base64UrlToken);
        }
    }
}
