using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Services;
using Jellyfin.Plugin.TelegramNotification.Configuration;

namespace Jellyfin.Plugin.TelegramNotification.Api
{
    [Route("/Notification/Telegram/Test/{UserID}", "POST", Summary = "Tests Telegram")]
    public class TestNotification : IReturnVoid
    {
        [ApiMember(Name = "UserID", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserID { get; set; }
    }

    class ServerApiEndpoints : IService
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;

        public ServerApiEndpoints(ILogger logger, IHttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        private TeleGramOptions GetOptions(String userID)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.JellyfinUserId, userID, StringComparison.OrdinalIgnoreCase));
        }

        public void Post(TestNotification request)
        {
            var task = PostAsync(request);
            Task.WaitAll(task);
        }

        public async Task PostAsync(TestNotification request)
        {
            var options = GetOptions(request.UserID);
            string message = Uri.EscapeDataString("This is a Test");

            _logger.LogDebug("Telegram <TEST> to {0} - {1}", options.BotToken, options.ChatID);

            var httpRequestOptions = new HttpRequestOptions
            {
                Url = "https://api.telegram.org/bot" + options.BotToken + "/sendmessage?chat_id=" + options.ChatID + "&text=" + message,
                CancellationToken = CancellationToken.None
            };


            using (await _httpClient.Post(httpRequestOptions).ConfigureAwait(false))
            {

            }


        }
    }
}
