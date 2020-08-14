using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CQ2IOT.Events
{
    public class GroupEnterRequestEventArgs : EventArgs
    {
        public JObject json;
        public string answer;
        public GroupEnterRequestEventArgs(JObject json)
        {
            throughgroup = new Model.Group()
            {
                id = json["EventData"].Value<long>("GroupId"),
                name = json["EventData"].Value<string>("GroupName")
            };
            user = new Model.QQ()
            {
                qq = json["EventData"].Value<long>("Who"),
                name = json["EventData"].Value<string>("WhoName")
            };
            this.json = (JObject)json["EventData"];
            answer = json["EventData"].Value<string>("Content");
        }
    }
}
