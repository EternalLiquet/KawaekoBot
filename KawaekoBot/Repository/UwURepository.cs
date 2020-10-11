using Discord.WebSocket;
using KawaekoBot.Util;
using MongoDB.Bson;
using MongoDB.Driver;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KawaekoBot.Entities;
using Discord.Commands;

namespace KawaekoBot.Repository
{
    public class UwURepository
    {
        private SocketTextChannel _channel;
        private SocketMessage _message;
        private string _kawaekoId;
        private string _guildId;
        private string _userId;

        private IMongoCollection<UwURecord> _collection = MongoDbClient.kawaekoDatabase.GetCollection<UwURecord>("UwUCounter");

        public UwURepository(SocketCommandContext commandContext)
        {
            this._kawaekoId = $"{commandContext.Guild.Id}{commandContext.User.Id}";
            this._guildId = commandContext.Guild.Id.ToString();
            this._userId = commandContext.User.Id.ToString();
        }

        public UwURepository(SocketUser user, SocketCommandContext commandContext)
        {
            this._kawaekoId = $"{commandContext.Guild.Id}{user.Id}";
            this._guildId = commandContext.Guild.Id.ToString();
            this._userId = user.Id.ToString();
        }

        public UwURepository(SocketTextChannel channel, SocketMessage message)
        {
            this._channel = channel;
            this._message = message;
            this._kawaekoId = $"{_channel.Guild.Id}{_message.Author.Id}";
            this._guildId = _channel.Guild.Id.ToString();
            this._userId = _message.Author.Id.ToString();
        }

        public async Task<List<UwURecord>> GetUwURecord()
        {
            try
            {
                var filterOne = Builders<UwURecord>.Filter.Eq("kawaekoId", _kawaekoId);
                var results = await _collection.FindAsync<UwURecord>(filterOne);
                return await results.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"User {_message.Author} with KawaekoId {_kawaekoId} failed retrieving from database for reason: {e.Message}");
                return null;
            }
        }

        public async Task InsertNewUwURecord()
        {
            var uwuDocument = new UwURecord
            {
                kawaekoId = _kawaekoId,
                guildId = _guildId,
                userId = _userId,
                uwuCount = 1
            };
            try
            {
                await _collection.InsertOneAsync(uwuDocument);
                Log.Information($"User {_message.Author} with KawaekoId {_kawaekoId} has been inserted into the database");
            }
            catch (Exception e)
            {
                Log.Error($"User {_message.Author} with KawaekoId {_kawaekoId} failed inserting into database for reason: {e.Message}");
            }
        }

        public async Task<int> GetUserUwUCount()
        {
            try
            {
                var filterUser = Builders<UwURecord>.Filter.Eq("kawaekoId", _kawaekoId);
                var results = await _collection.FindAsync<UwURecord>(filterUser);
                var uwuRecord = await results.FirstOrDefaultAsync();
                switch (uwuRecord)
                {
                    case null:
                        return 0;
                    default:
                        Log.Verbose(uwuRecord.ToString());
                        Log.Verbose(uwuRecord.uwuCount.ToString());
                        return uwuRecord.uwuCount;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error retrieving uwu count for user with KawaekoId {_kawaekoId} for reason: {e.Message}");
                return -1;
            }
        }

        public async Task<int> GetServerUwUCount()
        {
            try
            {
                int uwuCount = 0;
                var filterGuild = Builders<UwURecord>.Filter.Eq("guildId", _guildId);
                var results = await _collection.FindAsync<UwURecord>(filterGuild);
                foreach (var uwuRecord in await results.ToListAsync())
                {
                    uwuCount += uwuRecord.uwuCount;
                }
                return uwuCount;
            }
            catch (Exception e)
            {
                Log.Error($"Error retrieving uwu count for server for reason: {e.Message}");
                return -1;
            }
        }

        public async Task IncrementUwURecord(UwURecord uwuRecord)
        {
            var filterOne = Builders<UwURecord>.Filter.Eq("kawaekoId", _kawaekoId);
            var incrementUwUCount = Builders<UwURecord>.Update.Set("uwuCount", uwuRecord.uwuCount += 1);
            try
            {
                await _collection.UpdateOneAsync(filterOne, incrementUwUCount);
                Log.Information($"User {_message.Author} with KawaekoId {_kawaekoId} has been updated in the database");
            }
            catch (Exception e)
            {
                Log.Error($"User {_message.Author} with KawaekoId {_kawaekoId} failed updating database for reason: {e.Message}");
            }
        }
    }
}
