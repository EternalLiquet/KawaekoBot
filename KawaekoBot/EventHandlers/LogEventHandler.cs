using Discord.WebSocket;
using KawaekoBot.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KawaekoBot.EventHandlers
{
    public class LogEventHandler
    {
        private readonly DiscordSocketClient _discordClient;

        public LogEventHandler(DiscordSocketClient discordClient)
        {
            Log.Information("Instantiating Log Event Handler");
            this._discordClient = discordClient;
        }

        public void InitializeLogDependentServices()
        {
            InstantiateLogEventServices();
        }

        private void InstantiateLogEventServices()
        {
            _discordClient.Log += LogHandler.LogMessages;
        }
    }
}
