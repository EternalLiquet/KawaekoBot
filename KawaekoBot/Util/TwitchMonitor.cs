using Discord.WebSocket;
using KawaekoBot.Entities;
using KawaekoBot.Entities.TwitchEntities;
using KawaekoBot.Services;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace KawaekoBot.Util
{
    public static class TwitchMonitor
    {
        public static TwitchAPI API;
        public static LiveStreamMonitorService Monitor;
        public static List<string> userIdList;
        private static TwitchService twitchService;
        private static bool twitchMonitorStarted = false;
        private static HttpClient httpClient = new HttpClient();
        private static DiscordSocketClient _client;
        

        public async static Task UpdateMonitorList()
        {
            twitchService = new TwitchService(_client);
            var records = await twitchService.GetTwitchMonitorList();
            List<string> userNameList = new List<string>();
            foreach (var record in records)
            {
                userNameList.Add(record.twitchUsername);
            }
            userIdList = userNameList;

            if (userIdList.Count >= 1)
            {
                try 
                {
                    Monitor.SetChannelsByName(userIdList);
                }
                catch (Exception e)
                {
                    Log.Error($"Twitch User Not FoundL ${e.Message}");
                }
                if (!twitchMonitorStarted)
                {
                    Log.Information($"Starting Twitch Monitor for {userIdList.Count} users");
                    Monitor.Start();
                    Monitor.OnStreamOnline += twitchService.Monitor_OnStreamOnline;
                    twitchMonitorStarted = true;
                }
            }
            else
            {
                if (twitchMonitorStarted)
                    Monitor.Stop();
            }
        }

        public async static Task StartLiveMonitorAsync(DiscordSocketClient client)
        {
            _client = client;
            API = new TwitchAPI();
            API.Settings.ClientId = AppSettings.Settings["twitchClientId"]; 
            HttpResponseMessage results = await httpClient.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={API.Settings.ClientId}&client_secret={AppSettings.Settings["twitchClientSecret"]}&grant_type=client_credentials", new FormUrlEncodedContent(new KeyValuePair<string, string>[0]));
            Log.Information($"Results: {results.ToString()}");
            if (!results.IsSuccessStatusCode) Log.Error($"Attempt to get Twitch Access Token Failed for reason: {await results.Content.ReadAsStringAsync()}");
            else
            {
                TwitchOAuthResponse twitchResponse = JsonConvert.DeserializeObject<TwitchOAuthResponse>(await results.Content.ReadAsStringAsync());
                Log.Information($"Twitch Access Token successfully retrieved");
                API.Settings.AccessToken = twitchResponse.access_token;
            }
            Monitor = new LiveStreamMonitorService(API, 15);
            await UpdateMonitorList();
            await Task.Delay(-1);
        }
    }
}
