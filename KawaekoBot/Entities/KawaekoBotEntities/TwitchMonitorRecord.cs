using Discord.Commands;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KawaekoBot.Entities.KawaekoBotEntities
{
    public class TwitchMonitorRecord
    {
        [BsonId]
        public string twitchMonitorRecordId { get; set; }
        public string channelId { get; set; }
        public string twitchId { get; set; }
        public string streamAnnouncementMessage { get; set; }

        public TwitchMonitorRecord(SocketCommandContext context, string twitchId, string streamAnnouncementMessage)
        {
            this.twitchMonitorRecordId = $"{context.Channel.Id}{twitchId}";
            this.channelId = context.Channel.Id.ToString();
            this.twitchId = twitchId;
            this.streamAnnouncementMessage = streamAnnouncementMessage;
        }
    }
}
