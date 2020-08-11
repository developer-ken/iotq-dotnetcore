using CQ2IOT.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Events
{
    public class PrivateMessageReceiveEventArgs : EventArgs
    {
        public Message message;
        public PrivateMessageReceiveEventArgs(JObject json)
        {
            user = new Model.QQ()
            {
                qq = json.Value<long>("FromUin"),
            };
            if (json["TempUin"] != null)
                throughgroup = new Model.Group()
                {
                    id = json.Value<long>("TempUin")
                };
            switch (json.Value<string>("MsgType"))
            {
                case "PicMsg":
                    message = new PicMessage(json);
                    break;
                case "VoiceMsg":
                    message = new VoiceMessage(json);
                    break;
                case "TempSessionMsg":
                    
                    break;
                case "TextMsg":
                default:
                    message = new Message(json);
                    break;
            }
        }
    }
}
