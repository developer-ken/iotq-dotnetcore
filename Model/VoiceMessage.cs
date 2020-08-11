using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Model
{
    class VoiceMessage : Message
    {
        public JObject subjson;
        public VoiceFile voice;

        public VoiceMessage(JObject json) : base(json)
        {
            subjson = (JObject)JsonConvert.DeserializeObject(content);
            voice = new VoiceFile(subjson);
        }
    }
}
