using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Text;
using Telegram.Bot.Examples.AzureFunctions.WebHook.Services;
using TelegramBotFunction.Entities;

namespace TelegramBotFunction.Api
{
    public static class QueueRequests
    {
        private static readonly string _adminUrl = EnvironmentManager.GetAdminUrl()!;
        public async static Task<HttpStatusCode> AddPosititon(Position position)
        {
            using (var client = new HttpClient())
            {

                if (_adminUrl == "")
                {
                    return HttpStatusCode.BadRequest;
                }

                client.BaseAddress = new Uri(_adminUrl);
                client.DefaultRequestHeaders.Add("IsBot", "1");

                var values = new Dictionary<string, string>
                {   
                    {"id","00000000-0000-0000-0000-000000000000"},
                    {"authorId", $"{position.AuthorId}"},
                    {"botId", $"{position.BotId}"},
                    {"numberInTheQueue", "0"},
                    {"requester", $"{position.Requester}"},
                    {"registrationTime", "2022-05-18T11:58:54.3107239+00:00"},
                    {"description", $"{position.Description}"}
                };

                var json = JsonConvert.SerializeObject(values);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/Queue/CreatePositionPost", content);
                var responseError = response.StatusCode;
                string resultContent = await response.Content.ReadAsStringAsync();


                return response.StatusCode;
            }
        }

        public async static Task<string> GetUsersPosititonStatus(string authorId)
        {
            using (var client = new HttpClient())
            {
                if (_adminUrl == "")
                {
                    return "Bad connection with admin site. Try again later.";
                }

                client.BaseAddress = new Uri(_adminUrl);
                client.DefaultRequestHeaders.Add("IsBot", "1");

                var response = await client.GetAsync($"api/Queue/GetByAuthorId/{authorId}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody is null)
                {
                    return "you have not yet registered in the queue. Please, try again later.";
                }
                string result = responseBody.Replace("[", "").Replace("]", "");

                return result;
            }
        }

        public async static Task<string> GetLastPosition()
        {
            using (var client = new HttpClient())
            {
                if (_adminUrl == "")
                {
                    return "Unfortunately, bad connection with admin site. Try again later.";
                }

                client.BaseAddress = new Uri(_adminUrl);
                client.DefaultRequestHeaders.Add("IsBot", "1");

                var response = await client.GetAsync($"api/Queue/GetLastNumberFromQueue");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody is null)
                {
                    return "unknown. Server error.";
                }
                string result = responseBody.Replace("[", "").Replace("]", "");

                return result;
            }
        }

        public async static Task<HttpStatusCode> DeletePosition(string positionId)
        {
            using (var client = new HttpClient())
            {
                if (_adminUrl == "")
                {
                    return HttpStatusCode.BadRequest;
                }

                client.BaseAddress = new Uri(_adminUrl);
                client.DefaultRequestHeaders.Add("IsBot", "1");
                var values = positionId;

                var json = JsonConvert.SerializeObject(values);

                var content = new StringContent("", Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"api/Queue/DeletePosition/{positionId}", content);
                string resultContent = await response.Content.ReadAsStringAsync();

                return response.StatusCode;
            }
        }

        public async static Task<Dictionary<string, string>?> GetUserPositions(string authorId)
        {
            using (var client = new HttpClient())
            {
                if (_adminUrl == "")
                {
                    return null;
                }

                client.BaseAddress = new Uri(_adminUrl);
                client.DefaultRequestHeaders.Add("IsBot", "1");

                var response = await client.GetAsync($"api/Queue/GetPositionsByAuthorId/{authorId}");
                response.EnsureSuccessStatusCode();
                string responseBodyJson = await response.Content.ReadAsStringAsync();
                if (responseBodyJson is null)
                {
                    return default;
                }

                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBodyJson);

                return result;
            }
        }
    }
}
