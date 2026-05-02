using Newtonsoft.Json;

namespace Common.Utilities.Helpers;

public static class CaptchaHelper
{
    public static async Task<bool> GoogleCaptchaValidation(string secret, string token, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var dict = new Dictionary<string, string> { { "secret", secret }, { "response", token } };

        HttpResponseMessage response;
        using (var req = new HttpRequestMessage(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify") { Content = new FormUrlEncodedContent(dict) })
        {
            response = await client.SendAsync(req, cancellationToken);
        }

        var responseString = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(responseString);
        return jsonResponse?.Success == true && jsonResponse.Score > 0.4d;
    }

    private sealed class GoogleRecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
