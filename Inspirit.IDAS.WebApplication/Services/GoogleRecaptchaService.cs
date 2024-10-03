using Inspirit.IDAS.WebApplication.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Services
{
    public class GoogleRecaptchaService
    {
        private ReCAPTCHASettings  _settings;
        private ILogger<GoogleRecaptchaService> logger;

        public GoogleRecaptchaService(IOptions<ReCAPTCHASettings> settings, ILogger<GoogleRecaptchaService> logger)
        {
            _settings = settings.Value;
            this.logger = logger;
        }
        public string getReCAPTCHAKeys()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build().GetSection("GoogleRecaptcha").GetSection("ReCAPTCHA_Secrete_Key").Value;
        }


        public async Task<GoogleRensponse> VerifyResponse(string _Token)
        {
            var result = false;
            var captchaVerfication = new GoogleRensponse();
            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
            string secret2 = getReCAPTCHAKeys();

;            GoogleRecaptchaData _MyData = new GoogleRecaptchaData
            {
                response = _Token,
                secret = secret2
            };

            try
            {
                HttpClient client = new HttpClient();

                var response = await client.PostAsync($"{googleVerificationUrl}?secret={_MyData.secret}&response={_MyData.response}", null);
                var jsonString = await response.Content.ReadAsStringAsync();
                captchaVerfication = JsonConvert.DeserializeObject<GoogleRensponse>(jsonString);

                 result = captchaVerfication.success;
       
            }
            catch (Exception e)
            {
                // fail gracefully, but log
                logger.LogError("Failed to process captcha validation", e);
            }


            return captchaVerfication;
        }
    }

    public class GoogleRecaptchaData
    {
        public string response { get; set; }
        public string secret { get; set; }
    }

    public class GoogleRensponse
    {
        public bool success { get; set; }
        public double score { get; set; }
        public string action { get; set; }
        public DateTime response { get; set; }
        public string hostname { get; set; }
    }
}
