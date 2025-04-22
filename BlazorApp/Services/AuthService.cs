using BlazorApp.Models;
using System.Net.Http.Json;

namespace BlazorApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponse> Register(RegisterModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", model);
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse> Login(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", model);
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse> ChangePassword(ChangePasswordModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/ChangePassword", model);
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse> ForgotPassword(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/ForgotPassword", email);
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse> ResetPassword(ResetPasswordModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/ResetPassword", model);
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
    }
}
