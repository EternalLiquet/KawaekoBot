using System;
using System.Collections.Generic;
using System.Text;

namespace KawaekoBot.Entities.TwitchEntities
{
    public class TwitchHelixGameInfoResponse
    {
        public GameData[] data;
        public Dictionary<string, string> pagination;
    }

    public class GameData
    {
        public string box_art_url { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
}
