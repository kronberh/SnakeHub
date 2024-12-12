using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using SnakeHub.FormModels;
using SnakeHub.Models.Response;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SnakeHub.Controllers
{
    public class AccountController(IConfiguration configuration): Controller
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IHtmlContent> RegisterAsync(RegisterFormModel formModel)
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = new();
                StringContent requestContent = new(JsonConvert.SerializeObject(new
                {
                    formModel.Login,
                    Password = Hash(formModel.Password)
                }), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:RegisterSlug"]}", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    AuthResponse? authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        Response.Cookies.Append("AuthToken", authResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTimeOffset.UtcNow.AddDays(1)
                        });
                        return new HtmlString(string.Empty);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, $"Login \"{formModel.Login}\" is already taken. Please choose another one.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            StringBuilder sb = new();
            foreach (ModelError error in ModelState.Values.SelectMany(e => e.Errors))
            {
                sb.Append(error.ErrorMessage);
                sb.Append("<br>");
            }
            if (sb.Length >= 4)
            {
                sb.Remove(sb.Length - 4, 4);
            }
            return new HtmlString(sb.ToString());
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IHtmlContent> LoginAsync(LoginFormModel formModel)
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = new();
                StringContent requestContent = new(JsonConvert.SerializeObject(new
                {
                    formModel.Login,
                    Password = Hash(formModel.Password),
                }), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:LoginSlug"]}", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    AuthResponse? authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        Response.Cookies.Append("AuthToken", authResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTimeOffset.UtcNow.AddDays(1)
                        });
                        return new HtmlString(string.Empty);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, $"Incorrect login or password.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            StringBuilder sb = new();
            foreach (ModelError error in ModelState.Values.SelectMany(e => e.Errors))
            {
                sb.Append(error.ErrorMessage);
                sb.Append("<br>");
            }
            if (sb.Length >= 4)
            {
                sb.Remove(sb.Length - 4, 4);
            }
            return new HtmlString(sb.ToString());
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IHtmlContent Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return new HtmlString(string.Empty);
        }

        static private string Hash(string password)
        {
            return Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(password)));
        }
    }
}
