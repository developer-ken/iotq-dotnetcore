using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Events
{
    public class FriendAddRequestEventArgs : EventArgs
    {
        public string message;
        public string source;
        public FriendAddRequestEventArgs(JObject json)
        {
            JObject evdata = (JObject)json["EventData"];
            user = new Model.QQ()
            {
                qq = evdata.Value<long>("UserID"),
                name = evdata.Value<string>("UserNick"),
            };
            throughgroup = new Model.Group()
            {
                id = evdata.Value<long>("FromGroupId"),
                name = evdata.Value<string>("FromGroupName")
            };
            message = evdata.Value<string>("Content");
            source = evdata.Value<string>("FromContent");
        }
    }
}
