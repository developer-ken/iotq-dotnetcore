using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT
{
    /// <summary>
    /// 协议初始化包
    /// 标准格式：
    /// 0{JSON}
    /// </summary>
    public class InitPack : NetPack
    {
        public int hbinterval, timeout;
        public string sid;

        public InitPack(byte[] pack)
        {
            depack(Encoding.UTF8.GetString(pack));
        }
        public InitPack(string pack)
        {
            depack(pack);
        }

        private void depack(string pack)
        {
            if (!isInitPack(pack)) throw new Exception("Not an initPack.");
            base.code = 0;
            eventstr = "INIT";
            try
            {
                payload = (JObject)JsonConvert.DeserializeObject(pack.Substring(1));
                hbinterval = payload.Value<int>("pingInterval");
                timeout = payload.Value<int>("pingTimeout");
                sid = payload.Value<string>("sid");
            }
            catch
            {
                throw new Exception("Not an initPack.");
            }
        }
        
        /// <summary>
        /// 判断是否为一个InitPack
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static bool isInitPack(string pack)
        {
            if (pack.Length < 1) return false;
            string code = pack.Substring(0, 1);
            return (code == "0");
        }
    }
}
