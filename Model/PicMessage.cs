using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQ2IOT.Model
{
    public class PicMessage : Message
    {
        public JObject subjson;
        public PictureFile[] picture;
        public PicMessage(JObject json) : base(json)
        {
            subjson = (JObject)JsonConvert.DeserializeObject(content);
            JArray bjson = (JArray)(subjson["GroupPic"] != null ? subjson["GroupPic"] : subjson["FriendPic"]);
            try
            {
                JArray jr = (JArray)subjson["UserID"];
                at = new long[jr.Count];
                int ii = 0;
                foreach (JValue q in jr)
                {
                    string qq = q.ToString();
                    at[ii] = (long.Parse(qq));
                    ii++;
                }
            }
            catch { }
            try
            {
                if (bjson == null)
                    bjson = new JArray();
                bjson.Add(subjson);//闪照在Content里
            }
            catch
            {
                throw new Exception("Not a picmessage pack.");
            }
            picture = new PictureFile[bjson.Count];
            int i = 0;
            foreach (JObject j in bjson)
            {
                if (j["FileMd5"] == null) continue;//不是图片
                picture[i] = (new PictureFile(j));
                i++;
            }
            plaintext = (subjson["Content"] == null ? null : subjson.Value<string>("Content"));
        }
    }
}
