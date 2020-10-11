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
        public string twitchUsername { get; set; }
        public string streamAnnouncementMessage { get; set; }

        public TwitchMonitorRecord(SocketCommandContext context, string twitchUsername, string streamAnnouncementMessage)
        {
            this.twitchMonitorRecordId = $"{context.Channel.Id}{twitchUsername}";
            this.channelId = context.Channel.Id.ToString();
            this.twitchUsername = twitchUsername;
            this.streamAnnouncementMessage = streamAnnouncementMessage;
        }
    }
}
