using Discord;
using Discord.Commands;
using Discord.WebSocket;

using KawaekoBot.Util;
using KawaekoBot.EventHandlers;

using Serilog;

using System.IO;
using System.Net;
using System.Threading.Tasks;
using KawaekoBot.Services;
using System;
using TwitchLib.Api.Services;
using TwitchLib.Api;
using System.Net.Http;
using System.Collections.Generic;
using KawaekoBot.Entities;

using Newtonsoft.Json;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using KawaekoBot.Entities.TwitchEntities;

namespace KawaekoBot
{
    class Program
    {
        private DiscordSocketClient _discordClient;
        private LiveStreamMonitorService Monitor;
        private TwitchAPI API;
        private static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            Support.StartupOperations();
            await LogIntoDiscord();
            await InitializeEventHandlers();
            await TwitchMonitor.StartLiveMonitorAsync(_discordClient);
            //await ConfigLiveMonitorAsync();
            await Task.Delay(-1);
        }

        private async Task ConfigLiveMonitorAsync()
        {
            API = new TwitchAPI();
            string clientId = AppSettings.Settings["twitchClientId"];
            string clientSecret = AppSettings.Settings["twitchClientSecret"];
            string accessToken = null;
            API.Settings.ClientId = clientId;
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("", "")
            });
            HttpResponseMessage results = await httpClient.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", content);
            Log.Information($"Results: {results.ToString()}");
            if (!results.IsSuccessStatusCode) Log.Error($"Attempt to get Twitch Access Token Failed for reason: {await results.Content.ReadAsStringAsync()}");
            else
            {
                TwitchOAuthResponse twitchResponse = JsonConvert.DeserializeObject<TwitchOAuthResponse>(await results.Content.ReadAsStringAsync());
                Log.Information($"Twitch Access Token is: {twitchResponse.access_token}");
                accessToken = twitchResponse.access_token;
            }
            API.Settings.AccessToken = accessToken;
            httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            //var myVariableNamingIsShitty = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("Client-ID", clientId),
            //    new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
            //});
            //HttpResponseMessage results2PleaseDontJudgeThisIsATest = await httpClient.GetAsync($"https://api.twitch.tv/helix/users?login=pandabual");
            //Log.Information($"Results2: {results2PleaseDontJudgeThisIsATest}");
            //string userId = null;
            //if (!results.IsSuccessStatusCode) Log.Error($"Attempt to get twitch user id failed for reason: {await results2PleaseDontJudgeThisIsATest.Content.ReadAsStringAsync()}");
            //else
            //{
            //    TwitchHelixUserInfoResponse userInfo = JsonConvert.DeserializeObject<TwitchHelixUserInfoResponse>(await results2PleaseDontJudgeThisIsATest.Content.ReadAsStringAsync());
            //    Log.Information($"Twitch User Id is: {userInfo.data[0].id}");
            //    userId = userInfo.data[0].id;
            //}
            Monitor = new LiveStreamMonitorService(API, 15);
            List<string> userIdList = new List<string>{ "pandabual" };
            Monitor.SetChannelsByName(userIdList);
            Monitor.Start();
            await Task.Delay(-1);
        }

        

        private async Task InitializeEventHandlers()
        {
            Log.Information("Initializing Event Handlers");
            MessageHandler messageHandler = new MessageHandler(_discordClient);
            await messageHandler.InitializeMessageDependentServices();
            LogEventHandler logEventHandler = new LogEventHandler(_discordClient);
            logEventHandler.InitializeLogDependentServices();
        }

        private async Task LogIntoDiscord()
        {
            CreateNewDiscordSocketClientWithConfigurations();
            bool loggedIn = false;
            while (loggedIn == false)
            {
                try
                {
                    await _discordClient.LoginAsync(TokenType.Bot, AppSettings.Settings["botToken"]);
                    await _discordClient.StartAsync();
                    await _discordClient.SetGameAsync("For a list of my commands, type ~> help", null, ActivityType.Playing);
                    _discordClient.Ready += () =>
                    {
                        Log.Information("Kawaeko Bot successfully connected");
                        return Task.CompletedTask;
                    };
                    loggedIn = true;
                }
                catch (Discord.Net.HttpException e)
                {
                    Log.Error(e.ToString());
                    Log.Error($"Bot Token was incorrect, please review the settings file in {Path.GetFullPath(AppSettings.settingsFilePath)}");
                    if (e.HttpCode == HttpStatusCode.Unauthorized)
                    {
                        AppSettings.FixToken();
                    }
                }
            }
        }

        private void CreateNewDiscordSocketClientWithConfigurations()
        {
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 50
            });
        }
    }
}
