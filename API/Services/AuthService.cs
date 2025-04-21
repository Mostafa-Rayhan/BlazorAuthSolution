using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            if (model == null)
                throw new NullReferenceException("Register model is null");

            if (model.Password != model.Password)
                return new AuthResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false
                };

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Username,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new AuthResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return new AuthResponse
                {
                    Message = "There is no user with that Username",
                    IsSuccess = false
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new AuthResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };

            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                Token = tokenAsString,
                Expiration = token.ValidTo
            };
        }

        public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Password changed successfully"
                };
            }

            return new AuthResponse
            {
                IsSuccess = false,
                Message = "Something went wrong",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResponse> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with this email"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new AuthResponse
            {
                IsSuccess = true,
                Message = "Password reset token generated",
                Token = token
            };
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with this email"
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully"
                };
            }

            return new AuthResponse
            {
                IsSuccess = false,
                Message = "Something went wrong",
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}
