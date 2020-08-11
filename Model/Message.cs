using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQ2IOT.Model
{
    public class Message
    {
        public string content;
        public Group throughgroup;
        public QQ fromqq;
        public string type;
        public int timestamp;
        public DateTime time;
        public int msgseq;
        public long msgrandom;
        public string plaintext;
        public long[] at;
        public Message(JObject json)
        {
            fromqq = new Model.QQ()
            {
                qq = json.Value<long>("FromUserId"),
                name = json.Value<string>("FromNickName"),
            };
            throughgroup = new Model.Group()
            {
                id = json.Value<long>("FromGroupId"),
                name = json.Value<string>("FromGroupName")
            };
            content = json.Value<string>("Content");
            type = json.Value<string>("MsgType");
            timestamp = json.Value<int>("MsgTime");
            switch (type)
            {
                case "TextMsg":
                    plaintext = content;
                    break;
                case "AtMsg":
                    JObject subjson = (JObject)JsonConvert.DeserializeObject(content);
                    plaintext = subjson.Value<string>("Content");
                    //try {
                    JArray jr = (JArray)subjson["UserID"];
                    at = new long[jr.Count];
                    int i = 0;
                    foreach (JValue q in jr)
                    {
                        string qq = q.ToString();
                        at[i] = (long.Parse(qq));
                        i++;
                    }
                    //} catch { }
                    break;
            }
            if (json["MsgSeq"] != null) msgseq = json.Value<int>("MsgSeq");
            if (json["MsgRandom"] != null) msgrandom = json.Value<long>("MsgRandom");
        }
    }
}
