using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using KawaekoBot.Entities;
using KawaekoBot.Entities.KawaekoBotEntities;
using KawaekoBot.Services;
using Para.bot.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KawaekoBot.Modules
{
    [Name("Twitch Commands")]
    public class TwitchModule : InteractiveBase
    {
        private TwitchService twitchService;

        [Command("add twitch listener", RunMode = RunMode.Async)]
        [Summary("Will add a username to listen to on Twitch")]
        [Remarks("add twitch listener")]
        [RequireGuild]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddTwitchListener()
        {
            await Task.Factory.StartNew(() => { _ = InvokeAddTwitchListener(); });
        }

        [Command("check twitch listener", RunMode = RunMode.Async)]
        [Summary("Will check what twitch listeners are on this channel")]
        [Remarks("check twitch listener")]
        [RequireGuild]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CheckTwitchListener()
        {
            await Task.Factory.StartNew(() => { _ = InvokeCheckTwitchListener(); });
        }

        [Command("remove twitch listener", RunMode = RunMode.Async)]
        [Summary("Will remove a twitch listener from this channel")]
        [Remarks("remove twitch listener")]
        [RequireGuild]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveTwitchListener()
        {
            await Task.Factory.StartNew(() => { _ = InvokeRemoveTwitchListener(); });
        }

        private async Task InvokeRemoveTwitchListener()
        {
            List<IMessage> messagesInInteraction = new List<IMessage>();
            try
            {
                twitchService = new TwitchService();
                messagesInInteraction.Add(await ReplyAsync("Which twitch channel do you want to remove?"));
                var channelToRemove = await NextMessageAsync(timeout: TimeSpan.FromSeconds(60));
                if (!await IsChannelNotNullAndValidTwitchListener(messagesInInteraction, channelToRemove)) return;
                await twitchService.RemoveTwitchMonitorRecord($"{Context.Channel.Id}{channelToRemove.Content}");
                messagesInInteraction.Add(Context.Message);
            }
            finally
            {
                await CleanUpMessagesAfterFiveSeconds(messagesInInteraction);
            }
        }

        private async Task<bool> IsChannelNotNullAndValidTwitchListener(List<IMessage> messagesInInteraction, SocketMessage channelToRemove)
        {
            if (channelToRemove != null)
            {
                messagesInInteraction.Add(channelToRemove);
                var results = await twitchService.GetTwitchMonitorRecord(channelToRemove.Content);
                if (results.Count > 0)
                {
                    Log.Information("Twitch user found");
                    messagesInInteraction.Add(await ReplyAsync($"Twitch listener {channelToRemove.Content} was found for removal, removing this listener"));
                    return true;
                }
                else
                {
                    messagesInInteraction.Add(await ReplyAsync($"Twitch listener {channelToRemove.Content} was not found for removal"));
                    return false;
                }
            }
            else
            {
                messagesInInteraction.Add(await ReplyAsync("Time has expired, please try again"));
                return false;
            }
        }

        private async Task InvokeCheckTwitchListener()
        {
            List<IMessage> messagesInInteraction = new List<IMessage>();
            try
            {
                twitchService = new TwitchService();
                var twitchMonitorList = await twitchService.GetTwitchMonitorList();
                await SendTwitchListenerListEmbed(twitchMonitorList);
                messagesInInteraction.Add(Context.Message);
            }
            finally
            {
                await CleanUpMessagesAfterFiveSeconds(messagesInInteraction);
            }
        }

        private async Task SendTwitchListenerListEmbed(List<TwitchMonitorRecord> twitchMonitorList)
        {
            if (twitchMonitorList.Count > 0)
            {
                EmbedBuilder twitchListenerListEmbed = new EmbedBuilder()
                {
                    Title = "Twitch Listeners",
                    Description = "Kawaeko Bot is listening for these channels"
                };
                int channelListenCount = 0;
                foreach (var record in twitchMonitorList)
                {
                    if (Context.Channel.Id.ToString() == record.channelId)
                    {
                        channelListenCount++;
                        var username = await twitchService.GetTwitchUserInfoFromId(record.twitchId);
                        var usernameToEmbed = username?.data[0].display_name ?? "Something has gone wrong, please contact Liquet";
                        twitchListenerListEmbed.AddField(field =>
                        {
                            field.Name = usernameToEmbed;
                            field.Value = username != null ? record.streamAnnouncementMessage : "Error";
                        });
                    }
                }
                if (channelListenCount == 0) 
                {
                    twitchListenerListEmbed.AddField(field => 
                    {
                        field.Name = "No Twitch Listeners are active in this channel!";
                        field.Value = "You can add a new Twitch Listener to this channel by using ~> add twitch listener";
                    });
                }
                await ReplyAndDeleteAsync(null, false, embed: twitchListenerListEmbed.Build(), timeout: TimeSpan.FromSeconds(30));
            }
            else
            {
                await ReplyAndDeleteAsync("No Twitch Listeners active in this channel");
            }
        }

        private async Task InvokeAddTwitchListener()
        {
            List<IMessage> messagesInInteraction = new List<IMessage>();
            try
            {
                messagesInInteraction.Add(Context.Message);
                twitchService = new TwitchService();
                messagesInInteraction.Add(await ReplyAsync("Which twitch channel do you want to monitor for stream activity?"));
                var channelToWatch = await NextMessageAsync(timeout: TimeSpan.FromSeconds(60));
                string twitchUserId = await getTwitchUserIdFromChannelName(messagesInInteraction, channelToWatch);
                if (twitchUserId == null) return;
                messagesInInteraction.Add(await ReplyAsync("What would you like to say when this channel goes online? You can use `[streamer]` and `[streamlink]` and I will replace it with the streamer's name or their stream link respectively!"));
                var announcementText = await NextMessageAsync(timeout: TimeSpan.FromSeconds(120));
                if (!await IsAnnouncementNotNull(messagesInInteraction, announcementText)) return;
                await twitchService.SaveOrUpdateTwitchMonitorRecord(new TwitchMonitorRecord(Context, twitchUserId, announcementText.Content));
                await SendConfirmationEmbed(messagesInInteraction, channelToWatch, announcementText);
            }
            finally
            {
                await CleanUpMessagesAfterFiveSeconds(messagesInInteraction);
            }
        }

        private async Task SendConfirmationEmbed(List<IMessage> messagesInInteraction, SocketMessage channelToWatch, SocketMessage announcementText)
        {
            EmbedBuilder confirmationEmbed = new EmbedBuilder()
            {
                Title = "Twitch Monitor",
                Description = $"Just to confirm, here are the settings I'm saving:\nTwitch Username: {channelToWatch.Content}\nAnnouncement Text: {announcementText.Content}\nChannel to Announce: {Context.Channel.Name}"
            };
            var builtEmbed = confirmationEmbed.Build();
            messagesInInteraction.Add(await ReplyAsync(embed: builtEmbed));
        }

        private async Task<bool> IsAnnouncementNotNull(List<IMessage> messagesInInteraction, SocketMessage announcementText)
        {
            messagesInInteraction.Add(announcementText);
            if (announcementText != null)
            {
                return true;
            }
            else
            {
                messagesInInteraction.Add(await ReplyAsync("Time has expired, please try again"));
                return false;
            }
        }

        private async Task<string> getTwitchUserIdFromChannelName(List<IMessage> messagesInInteraction, SocketMessage channelToWatch)
        {
            if (channelToWatch != null)
            {
                messagesInInteraction.Add(channelToWatch);
                TwitchHelixUserInfoResponse userInfo = await twitchService.TwitchUserExists(channelToWatch.Content) ?? null;
                if (userInfo != null)
                {
                    Log.Information("Twitch user found");
                    messagesInInteraction.Add(await ReplyAsync($"Twitch user {channelToWatch.Content} was found"));
                    return userInfo.data[0].id;
                }
                else
                {
                    messagesInInteraction.Add(await ReplyAsync($"Twitch user {channelToWatch.Content} was not found"));
                    return null;
                }
            }
            else
            {
                messagesInInteraction.Add(await ReplyAsync("Time has expired, please try again"));
                return null;
            }
        }

        private async Task CleanUpMessagesAfterFiveSeconds(List<IMessage> messagesInInteraction)
        {
            Thread.Sleep(5000);
            IEnumerable<IMessage> filteredMessages = messagesInInteraction;
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
        }

        private async Task SendHelpEmbed(List<IMessage> messagesInInteraction)
        {
            EmbedBuilder helpEmbed = new EmbedBuilder()
            {
                Title = "Please confirm the message you want to appear when the stream starts",
                Description = $"You have a list of variables available to you, please select from below"
            };
            var finishedProduct = helpEmbed.Build();
            messagesInInteraction.Add(await ReplyAsync(embed: finishedProduct));
        }
    }
}
