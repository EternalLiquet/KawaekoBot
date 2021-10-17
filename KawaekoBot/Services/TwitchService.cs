using Discord;
using Discord.WebSocket;
using KawaekoBot.Entities;
using KawaekoBot.Entities.KawaekoBotEntities;
using KawaekoBot.Entities.TwitchEntities;
using KawaekoBot.Repository;
using KawaekoBot.Util;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace KawaekoBot.Services
{
    public class TwitchService
    {
        private HttpClient httpClient;
        private TwitchSettingsRepository twitchSettingsRepo;
        private DiscordSocketClient client;

        public TwitchService()
        {
            this.httpClient = new HttpClient();
            this.twitchSettingsRepo = new TwitchSettingsRepository();
        }

        public TwitchService(DiscordSocketClient client)
        {
            this.client = client;
            this.twitchSettingsRepo = new TwitchSettingsRepository();
        }

        public async Task<List<TwitchMonitorRecord>> GetTwitchMonitorList()
        {
            return await twitchSettingsRepo.GetAllTwitchMonitorRecords();
        }

        public async Task<List<TwitchMonitorRecord>> GetTwitchMonitorRecord(string id)
        {
            return await twitchSettingsRepo.GetTwitchMonitorRecordsById(id);
        }

        public async Task<TwitchHelixUserInfoResponse> TwitchUserExists(string username)
        {
            var accessToken = await CreateNewAccessToken();
            httpClient = new HttpClient();
            if (!httpClient.DefaultRequestHeaders.Contains("Client-ID"))
            {
                httpClient.DefaultRequestHeaders.Add("Client-ID", TwitchMonitor.API.Settings.ClientId);
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage userData = await httpClient.GetAsync($"https://api.twitch.tv/helix/users?login={username}");
            Log.Information($"Results: {userData}");
            if (!userData.IsSuccessStatusCode)
            {
                Log.Error($"Attempt to get twitch user id failed for reason: {await userData.Content.ReadAsStringAsync()}");
                return null;
            }
            else
            {
                TwitchHelixUserInfoResponse userInfo = JsonConvert.DeserializeObject<TwitchHelixUserInfoResponse>(await userData.Content.ReadAsStringAsync());
                if (userInfo.data.Count() != 0)
                {
                    Log.Information($"Twitch User {username} found");
                    return userInfo;
                }
                else
                {
                    Log.Error($"Twitch user {username} not found");
                    return null;
                }
            }
        }

        public async Task RemoveTwitchMonitorRecord(string twitchMonitorRecordId)
        {
            await twitchSettingsRepo.RemoveTwitchMonitorRecord(twitchMonitorRecordId);
            await TwitchMonitor.UpdateMonitorList();
        }

        public async Task<TwitchHelixUserInfoResponse> GetTwitchUserInfoFromUsername(string username)
        {
            var accessToken = await CreateNewAccessToken();
            httpClient = new HttpClient();
            if (!httpClient.DefaultRequestHeaders.Contains("Client-ID"))
            {
                httpClient.DefaultRequestHeaders.Add("Client-ID", TwitchMonitor.API.Settings.ClientId);
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage userData = await httpClient.GetAsync($"https://api.twitch.tv/helix/users?login={username}");
            Log.Information($"Results: {userData}");
            if (!userData.IsSuccessStatusCode)
            {
                Log.Error($"Attempt to get twitch user details failed for reason: {await userData.Content.ReadAsStringAsync()}");
                return null;
            }
            else
            {
                TwitchHelixUserInfoResponse userInfo = JsonConvert.DeserializeObject<TwitchHelixUserInfoResponse>(await userData.Content.ReadAsStringAsync());
                if (userInfo.data.Count() != 0)
                {
                    Log.Information($"Twitch User Details For {username} found");
                    return userInfo;
                }
                else
                {
                    Log.Error($"Twitch User Details For {username} not found");
                    return null;
                }
            }
        }

        public async Task<TwitchHelixUserInfoResponse> GetTwitchUserInfoFromId(string twitchId)
        {
            var accessToken = await CreateNewAccessToken();
            httpClient = new HttpClient();
            if (!httpClient.DefaultRequestHeaders.Contains("Client-ID"))
            {
                httpClient.DefaultRequestHeaders.Add("Client-ID", TwitchMonitor.API.Settings.ClientId);
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage userData = await httpClient.GetAsync($"https://api.twitch.tv/helix/users?id={twitchId}");
            Log.Information($"Results: {userData}");
            if (!userData.IsSuccessStatusCode)
            {
                Log.Error($"Attempt to get twitch user details failed for reason: {await userData.Content.ReadAsStringAsync()}");
                return null;
            }
            else
            {
                TwitchHelixUserInfoResponse userInfo = JsonConvert.DeserializeObject<TwitchHelixUserInfoResponse>(await userData.Content.ReadAsStringAsync());
                if (userInfo.data.Count() != 0)
                {
                    Log.Information($"Twitch User Details For ID {twitchId} found");
                    return userInfo;
                }
                else
                {
                    Log.Error($"Twitch User Details For ID {twitchId} not found");
                    return null;
                }
            }
        }

        public async Task<string> GetGameFromApi(string gameId)
        {
            var accessToken = await CreateNewAccessToken();
            httpClient = new HttpClient();
            if (!httpClient.DefaultRequestHeaders.Contains("Client-ID"))
            {
                httpClient.DefaultRequestHeaders.Add("Client-ID", TwitchMonitor.API.Settings.ClientId);
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage gameLookup = await httpClient.GetAsync($"https://api.twitch.tv/helix/games?id={gameId}");
            if (!gameLookup.IsSuccessStatusCode)
            {
                Log.Error($"Attempt to get twitch user id failed for reason: {await gameLookup.Content.ReadAsStringAsync()}");
                return null;
            }
            else
            {
                TwitchHelixGameInfoResponse gameData = JsonConvert.DeserializeObject<TwitchHelixGameInfoResponse>(await gameLookup.Content.ReadAsStringAsync());
                if (gameData.data != null)
                {
                    Log.Information($"Game: {gameData.data.FirstOrDefault().name} was found");
                    return gameData.data.FirstOrDefault().name;
                }
                else
                {
                    Log.Error($"Game Id {gameId} not found");
                    return null;
                }
            }

        }

        public async Task SaveOrUpdateTwitchMonitorRecord(TwitchMonitorRecord record)
        {
            var twitchMonitorSettingsList = await twitchSettingsRepo.GetTwitchMonitorRecord(record);
            if (twitchMonitorSettingsList == null) return;
            switch (twitchMonitorSettingsList.Count)
            {
                case 0:
                    await twitchSettingsRepo.InsertTwitchMonitorRecord(record);
                    break;
                default:
                    await twitchSettingsRepo.UpdateTwitchMonitorRecord(record);
                    break;
            }
            await TwitchMonitor.UpdateMonitorList();
        }

        public async void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            var streamLink = $"https://www.twitch.tv/{e.Stream.UserName}";
            var twitchUserInfo = await GetTwitchUserInfoFromUsername(e.Stream.UserName);
            var records = await GetTwitchMonitorRecord(twitchUserInfo.data[0].id);
            var profileImageLink = twitchUserInfo.data[0].profile_image_url;
            EmbedAuthorBuilder twitchStreamer = new EmbedAuthorBuilder
            {
                Name = $"{e.Stream.UserName}",
                IconUrl = profileImageLink
            };
            EmbedFieldBuilder gameField = new EmbedFieldBuilder
            {
                Name = "Game",
                Value = await GetGameFromApi(e.Stream.GameId),
                IsInline = true
            };
            EmbedFieldBuilder viewerCountField = new EmbedFieldBuilder
            {
                Name = "Viewers",
                Value = e.Stream.ViewerCount,
                IsInline = true
            };
            EmbedBuilder twitchAnnouncement = new EmbedBuilder
            {
                Title = e.Stream.Title,
                ImageUrl = $"https://static-cdn.jtvnw.net/previews-ttv/live_user_{e.Stream.UserName.ToLower()}-1920x1080.jpg",
                ThumbnailUrl = profileImageLink,
                Author = twitchStreamer,
                Fields = new List<EmbedFieldBuilder>() { gameField, viewerCountField },
                Url = streamLink
                
            };
            foreach (var record in records)
            {
                var channelToAnnounce = client.GetChannel(ulong.Parse(record.channelId)) as SocketTextChannel;
                var announcementMessage = record.streamAnnouncementMessage.Replace("[streamer]", e.Stream.UserName).Replace("[streamlink]", $"https://www.twitch.tv/{e.Stream.UserName}");
                await channelToAnnounce.SendMessageAsync(announcementMessage, false, twitchAnnouncement.Build());
            }
        }

        private async Task<string> CreateNewAccessToken()
        {
            string accessToken = null;
            httpClient = new HttpClient();
            HttpResponseMessage results = await httpClient.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={TwitchMonitor.API.Settings.ClientId}&client_secret={AppSettings.Settings["twitchClientSecret"]}&grant_type=client_credentials", new FormUrlEncodedContent(new KeyValuePair<string, string>[0]));
            if (!results.IsSuccessStatusCode) Log.Error($"Attempt to get Twitch Access Token Failed for reason: {await results.Content.ReadAsStringAsync()}");
            else
            {
                TwitchOAuthResponse twitchResponse = JsonConvert.DeserializeObject<TwitchOAuthResponse>(await results.Content.ReadAsStringAsync());
                Log.Information($"Twitch Access Token successfully retrieved");
                accessToken = twitchResponse.access_token;
            }
            return accessToken;
        }
    }
}
