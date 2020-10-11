using System;
using System.Collections.Generic;
using System.Text;

namespace KawaekoBot.Entities
{
    public class TwitchHelixUserInfoResponse
    {
        public UserData[] data { get; set; }
    }

    public class UserData
    {
        public string id { get; set; }
        public string login { get; set; }
        public string display_name { get; set; }
        public string type { get; set; }
        public string broadcaster_type { get; set; }
        public string description { get; set; }
        public string profile_image_url { get; set; }
        public string offline_image_url { get; set; }
        public string view_count { get; set; }
    }
}
