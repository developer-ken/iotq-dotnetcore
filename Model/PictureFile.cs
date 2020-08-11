using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.Model
{
    public class PictureFile : MessageMediaFile
    {
        public string id, md5, forwordbuf, forwordfield;
        public int size;

        public PictureFile() { }
        public PictureFile(JObject bjson)
        {
            url = bjson.Value<string>("Url");
            tip = bjson.Value<string>("Tips");
            id = bjson.Value<string>("FileId");
            md5 = bjson.Value<string>("FileMd5");
            forwordbuf = bjson.Value<string>("ForwordBuf");
            forwordfield = bjson.Value<string>("ForwordField");
            size = bjson.Value<int>("FileSize");
        }
    }
}
