using KawaekoBot.Entities.KawaekoBotEntities;
using KawaekoBot.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KawaekoBot.Repository
{
    public class TwitchSettingsRepository
    {
        private IMongoCollection<TwitchMonitorRecord> _collection = MongoDbClient.kawaekoDatabase.GetCollection<TwitchMonitorRecord>("TwitchMonitorSettings");

        public async Task InsertTwitchMonitorRecord(TwitchMonitorRecord twitchMonitorRecord)
        {
            try
            {
                await _collection.InsertOneAsync(twitchMonitorRecord);
                //Log.Information($"Twitch Monitor Record with username {twitchMonitorRecord.twitchUsername} saved successfully");
            }
            catch (Exception e)
            {
                //Log.Error($"Twitch Monitor Record with username {twitchMonitorRecord.twitchUsername} failed saving for reason: {e.Message}");
            }
        }

        public async Task<List<TwitchMonitorRecord>> GetTwitchMonitorRecordsById(string id)
        {
            try
            {
                var filterByUser = Builders<TwitchMonitorRecord>.Filter.Eq("twitchId", id);
                var results = await _collection.FindAsync<TwitchMonitorRecord>(filterByUser);
                return await results.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"Twitch Monitor Records with username {id} failed fetching for reason: {e.Message}");
                return null;
            }
        }

        public async Task<List<TwitchMonitorRecord>> GetTwitchMonitorRecord(TwitchMonitorRecord twitchMonitorRecord)
        {
            try
            {
                var filterOne = Builders<TwitchMonitorRecord>.Filter.Eq("twitchMonitorRecordId", twitchMonitorRecord.twitchMonitorRecordId);
                var results = await _collection.FindAsync<TwitchMonitorRecord>(filterOne);
                return await results.ToListAsync();
            }
            catch (Exception e)
            {
               // Log.Error($"Twitch Monitor Record with username {twitchMonitorRecord.twitchUsername} failed fetching for reason: {e.Message}");
                return null;
            }
        }

        public async Task UpdateTwitchMonitorRecord(TwitchMonitorRecord twitchMonitorRecord)
        {
            var filterOne = Builders<TwitchMonitorRecord>.Filter.Eq("twitchMonitorRecordId", twitchMonitorRecord.twitchMonitorRecordId);
            try
            {
                await _collection.ReplaceOneAsync(filterOne, twitchMonitorRecord);
               // Log.Information($"Twitch Monitor Record with username {twitchMonitorRecord.twitchUsername} saved successfully");
            }
            catch (Exception e)
            {
               // Log.Error($"Twitch Monitor Record with username {twitchMonitorRecord.twitchUsername} failed saving for reason: {e.Message}");
            }
        }

        public async Task<List<TwitchMonitorRecord>> GetAllTwitchMonitorRecords()
        {
            try
            {
                var results = await _collection.FindAsync<TwitchMonitorRecord>(new BsonDocument());
                return await results.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"Twitch Monitor Records cannot be found for reason: {e.Message}");
                return null;
            }
        }

        public async Task RemoveTwitchMonitorRecord(string twitchMonitorRecordId)
        {
            var filterOne = Builders<TwitchMonitorRecord>.Filter.Eq("twitchMonitorRecordId", twitchMonitorRecordId);
            try
            {
                await _collection.DeleteOneAsync(filterOne);
                Log.Information($"Twitch Monitor Record with ID {twitchMonitorRecordId} has been deleted");
            }
            catch (Exception e)
            {
                Log.Error($"Twitch Monitor Record cannot be deleted for reason: {e.Message}");
            }
        }
    }
}
