using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FreakyFashion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ICustomerService customerService;
        private readonly IAuthenticateUserService authenticateUserService;

        public LoginController(ICustomerService customerService, IAuthenticateUserService authenticateUserService)
        {
            this.customerService = customerService;
            this.authenticateUserService = authenticateUserService;
        }


        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.PassWord))
                return Unauthorized(new
                {
                    status = 401,
                    title = "Unauthorized",
                    message = "The username or password is empty."
                });

            var user = authenticateUserService.LoginToAuthenticate(loginRequest.UserName, loginRequest.PassWord, customerService);
            if (user != null)
            {
                var authenUser = authenticateUserService.GenerateToken(user);
                return Ok(authenUser);
            }
            else
            {
                return Unauthorized(new
                {
                    status = 401,
                    title = "Unauthorized",
                    message = "The username or password you entered is incorrect."
                });
            }
        }
    }
}
