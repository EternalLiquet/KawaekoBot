using Discord.WebSocket;
using KawaekoBot.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KawaekoBot.Repository;
using KawaekoBot.Entities;
using Discord.Commands;

namespace KawaekoBot.Services
{
    public class UwUCounterService
    {
        public UwUCounterService()
        {

        }

        public async Task HandleMessage(SocketMessage messageEvent) 
        {
            var discordMessage = messageEvent as SocketUserMessage;
            if (MessageIsSystemMessage(discordMessage))
                return; //Return and ignore if the message is a discord system message
            if (discordMessage.Channel == null)
                return; //Return and ignore if the message is not in a guild
            if (discordMessage.Author.IsBot)
                return; //Return and ignore if the message is from a bot
            if (!discordMessage.Content.ToLower().Contains("uwu"))
                return; //Return and ignore if the message doesn't even have uwu, don't waste time
            var discordMessageChannel = messageEvent.Channel as SocketTextChannel;
            
            var uwuCounterRepo = new UwURepository(discordMessageChannel, messageEvent);
            var uwuList = await uwuCounterRepo.GetUwURecord();
            if (uwuList == null) return;
            switch (uwuList.Count)
            {
                case 0:
                    await uwuCounterRepo.InsertNewUwURecord();
                    break;
                default:
                    await uwuCounterRepo.IncrementUwURecord(uwuList.First());
                    break;
            }
        }

        public async Task<int> CountUwU(SocketGuildUser user, SocketCommandContext commandContext)
        {
            var uwuCollection = MongoDbClient.kawaekoDatabase.GetCollection<UwURecord>("UwUCounter");
            UwURepository uwuCounterRepo;
            switch (user) 
            {
                case null:
                    uwuCounterRepo = new UwURepository(commandContext);
                    return await uwuCounterRepo.GetServerUwUCount();
                default:
                    uwuCounterRepo = new UwURepository(user, commandContext);
                    return await uwuCounterRepo.GetUserUwUCount();
            }
        }

        internal bool MessageIsSystemMessage(SocketUserMessage discordMessage)
        {
            if (discordMessage == null)
                return true;
            else
                return false;
        }
    }
}
