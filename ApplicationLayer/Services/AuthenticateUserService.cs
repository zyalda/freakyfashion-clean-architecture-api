using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ApplicationLayer.Services
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly IConfiguration configuration;

        public AuthenticateUserService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UserManagerResponse GenerateToken(DtoCustomer dtoCustomer)
        {
            string issuer = configuration["Jwt:Issuer"];
            string audience = configuration["Jwt:Audience"];
            string myKey = configuration["Jwt:Key"];

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myKey));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                       
            var tokenOptions = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: signinCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var user = new UserManagerResponse
            {
                Id = dtoCustomer.Id,
                Name = dtoCustomer.Name,
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = DateTime.Now.AddMinutes(60).ToString()
            };
            return user;
        }

        public DtoCustomer LoginToAuthenticate(string userName, string passWord, ICustomerService customerService)
        {
            var user = customerService.GetCustomerToLogin(userName, passWord);

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
