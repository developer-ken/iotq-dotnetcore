using CQ2IOT.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Events
{
    public class GroupMessageReceiveEventArgs : EventArgs
    {
        public Message message;
        public GroupMessageReceiveEventArgs(JObject json)
        {
            user = new Model.QQ()
            {
                qq = json.Value<long>("FromUserId"),
                name = json.Value<string>("FromNickName"),
            };
            throughgroup = new Model.Group()
            {
                id = json.Value<long>("FromGroupId"),
                name = json.Value<string>("FromGroupName")
            };
            switch (json.Value<string>("MsgType"))
            {
                case "PicMsg":
                    message = new PicMessage(json);
                    break;
                case "VoiceMsg":
                    message = new VoiceMessage(json);
                    break;
                case "TextMsg":
                default:
                    message = new Message(json);
                    break;
            }
        }
    }
}
