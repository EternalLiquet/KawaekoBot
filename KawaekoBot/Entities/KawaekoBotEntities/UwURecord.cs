using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KawaekoBot.Entities
{
    public class UwURecord
    {
        [BsonId]
        public string kawaekoId { get; set; }
        public string guildId { get; set; }
        public string userId { get; set; }
        public int uwuCount { get; set; }
    }
}
