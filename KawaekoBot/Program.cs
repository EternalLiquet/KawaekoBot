﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

using KawaekoBot.Util;
using KawaekoBot.EventHandlers;

using Serilog;

using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace KawaekoBot
{
    class Program
    {
        private DiscordSocketClient _discordClient;
        private CommandService _commandService;
        private CommandHandler _commandHandler;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            Support.StartupOperations();
            await LogIntoDiscord();
            await InstantiateCommandServices();
            _discordClient.Log += LogHandler.LogMessages;
            await Task.Delay(-1);
        }

        private async Task InstantiateCommandServices()
        {
            Log.Information("Instantiating Command Services");
            CreateCommandServiceWithOptions(ref _commandService);
            _commandHandler = new CommandHandler(_discordClient, _commandService);
            await _commandHandler.InitializeCommandsAsync();
        }

        private void CreateCommandServiceWithOptions(ref CommandService _commandService)
        {
            _commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
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
                    await _discordClient.SetGameAsync("UwU Watcha Saaaay? OwO when you only meant welllll", null, ActivityType.Playing);
                    _discordClient.Ready += () =>
                    {
                        Log.Information("Bean Bot successfully connected");
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
