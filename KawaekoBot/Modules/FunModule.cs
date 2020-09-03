using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Serilog;
using Discord.WebSocket;
using KawaekoBot.Services;

namespace KawaekoBot.Modules
{

    [Name("For Fun Commands")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private static HttpClient httpClient = new HttpClient();

        [Command("yuumi")]
        [Summary("Will reply with: \"meow adc-kun let me buff u uwu\"")]
        [Alias("meow")]
        [Remarks("yuumi")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Yuumi()
        {
            await Task.Factory.StartNew(() => { _ = ReplyAsync("meow adc-kun let me buff u uwu"); });
        }

        [Command("8ball")]
        [Summary("Will predict the future uwu")]
        [Alias("fortune")]
        [Remarks("8ball [question]")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EightBall([Remainder ]string question = null)
        {
            await Task.Factory.StartNew(() => { _ = InvokeEightBallApi(question); });
        }

        [Command("meme")]
        [Summary("Will give you a random meme from reddit")]
        [Remarks("meme")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Meme(string subreddit = "")
        {
            await Task.Factory.StartNew(() => { _ = InvokeMemeApi(subreddit); });
        }

        [Command("uwu")]
        [Summary("I've been tracking uwu's, I will give you a user's count or the server's count")]
        [Remarks("uwu [User (Optional)]")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task UwU(SocketGuildUser user = null)
        {
            await Task.Factory.StartNew(() => { _ = InvokeUwUServices(user); });
        }

        private async Task InvokeUwUServices(SocketGuildUser user)
        {
            int uwuCount;
            if (user != null && user.IsBot) 
            {
                await ReplyAsync("Cannot find the uwu score for a bot");
                return;
            }
            switch (Context.Guild) 
            {
                case null:
                    await ReplyAsync("Please use this command in a server");
                    break;
                default:
                    UwUCounterService uwuCounterService = new UwUCounterService();
                    uwuCount = await uwuCounterService.CountUwU(user, Context);
                    await HandleUwUCount(uwuCount, user);
                    break;
            }
        }

        private async Task HandleUwUCount(int uwuCount, SocketGuildUser user)
        {
            switch (uwuCount)
            {
                case -1:
                    await ReplyAsync("An critical error has occured while gathering your server's uwu score");
                    break;
                default:
                    await HandleUserOrGuild(uwuCount, user);
                    break;
            }
        }

        private async Task HandleUserOrGuild(int uwuCount, SocketGuildUser user)
        {
            switch (user)
            {
                case null:
                    await ReplyAsync($"{Context.Guild.Name}'s uwu score is: {uwuCount}");
                    break;
                default:
                    await ReplyAsync($"{user.Username}'s uwu score is: {uwuCount}");
                    break;
            }
        }

        private async Task InvokeMemeApi(string subreddit)
        {
            string uri;
            if (string.IsNullOrEmpty(subreddit)) uri = $"https://meme-api.herokuapp.com/gimme";
            else uri = $"https://meme-api.herokuapp.com/gimme/{HttpUtility.UrlEncode(subreddit)}";
            HttpResponseMessage response = await httpClient.GetAsync(requestUri: uri);
            if (!response.IsSuccessStatusCode) await ReplyAsync("The meme machine is down, quick, call 911!");
            else
            {
                MemeResponse meme = JsonConvert.DeserializeObject<MemeResponse>(await response.Content.ReadAsStringAsync());
                EmbedBuilder memeBuilder = new EmbedBuilder()
                {
                    Title = meme.title,
                    Description = $"/r/{meme.subreddit}",
                    ImageUrl = meme.url
                };
                await ReplyAsync(embed: memeBuilder.Build());
            }
        }

        private async Task InvokeEightBallApi(string question)
        {
            if (string.IsNullOrEmpty(question)) await ReplyAsync("Please ask a question");
            else
            {
                HttpResponseMessage response = await httpClient.GetAsync($"https://8ball.delegator.com/magic/JSON/{HttpUtility.UrlEncode(question)}");
                if (!response.IsSuccessStatusCode) await ReplyAsync("The magic eight ball service is offline, please ask again later (like actually later)");
                else 
                {
                    EightBallResponse answer = JsonConvert.DeserializeObject<EightBallResponse>(await response.Content.ReadAsStringAsync());
                    await ReplyAsync(answer.magic["answer"]);
                }
            }
        }
    }

    public class EightBallResponse
    {
        public Dictionary<string, string> magic { get; set; }
    }

    public class MemeResponse
    {
        public string postLink { get; set; }
        public string subreddit { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool nsfw { get; set; }
        public bool spoiler { get; set; }
    }
}
