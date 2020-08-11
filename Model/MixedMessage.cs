using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Model
{
    public class MixedMessage : Message
    {
        public JObject subjson;
        public PictureFile picture;
        public MixedMessage(JObject json) : base(json)
        {
            subjson = (JObject)JsonConvert.DeserializeObject(content);
            picture = new PictureFile(subjson);
        }
    }
}
