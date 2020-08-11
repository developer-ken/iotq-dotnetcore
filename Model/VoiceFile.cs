using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Model
{
    public class VoiceFile : MessageMediaFile
    {
        public VoiceFile(JObject json){
            url = json.Value<string>("Url");
            tip = json.Value<string>("Tips");
        }
    }
}
