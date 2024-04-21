using Newtonsoft.Json;
using System;
using System.Security.Policy;
using System.Text;

namespace DotnetJobs.Application.Services
{
    public class ConvertKit
    {
        private string _apiSecret = "";
        private string _apiKey = "";
        private string _signupForm = "";
        private static readonly HttpClient client = new HttpClient();

        public ConvertKit()
        {
            _apiSecret = Environment.GetEnvironmentVariable("CONVERT_KIT_SECRET");
            _apiKey = Environment.GetEnvironmentVariable("CONVERT_KIT_KEY");
            _signupForm = Environment.GetEnvironmentVariable("CONVERT_KIT_FORM_ID");

            if (_apiSecret == null || _apiKey == null)
            {
                throw new Exception("ConvertKit keys not found");
            }
        }

        public async Task Subscribe(string email)
        {
            var data = new Dictionary<string, string>
            {
                {"api_key", _apiKey},
                {"email", email},
            };
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://api.convertkit.com/v3/forms/{_signupForm}/subscribe";
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Console.Write("Success");
            }
        }

        public void SendEmail(string email)
        {

        }
    }

    public enum ConvertKitEmailType
    {
        DeveloperSignup,
        CompanySignup,
        NoProfileReminder,
        Newsletter,
        DailyJobBoard,
        WeeklyJobBoard
    }
}
