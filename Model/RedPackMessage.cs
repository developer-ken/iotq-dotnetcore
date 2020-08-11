using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Model
{
    public class RedPackMessage : Message
    {
        public RedPackMessage(JObject json) : base(json)
        {

        }
    }
}
