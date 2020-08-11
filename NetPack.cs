using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT
{
    /// <summary>
    /// 网络协议包
    /// 标准格式：
    /// code["EVENTSTR",{JSON}]
    /// </summary>
    public class NetPack
    {
        /// <summary>
        /// 数据包包含的数据
        /// </summary>
        public JObject payload;
        /// <summary>
        /// 数据包状态码
        /// </summary>
        public int code;
        /// <summary>
        /// 数据包触发事件名
        /// </summary>
        public string eventstr;

        public NetPack() { }

        /// <summary>
        /// socket.io 数据包实例
        /// </summary>
        /// <param name="package">接收到的完整数据包字节数组</param>
        public NetPack(byte[] package)
        {
            string str = Encoding.UTF8.GetString(package);
            depack(str);
        }

        /// <summary>
        /// socket.io 数据包实例
        /// </summary>
        /// <param name="package">接收到的完整数据包字符串</param>
        public NetPack(string package)
        {
            depack(package);
        }

        /// <summary>
        /// 执行解包
        /// </summary>
        /// <param name="pack">数据包字符串</param>
        private void depack(string pack)
        {
            string codepart = pack.Split('[')[0];
            code = int.Parse((codepart.Length < 1) ? "-1" : codepart);
            if (codepart == pack) return;
            int ests = pack.IndexOf("[\"") + 2;
            string tmp = pack.Substring(ests);
            int este = tmp.IndexOf("\"");
            eventstr = tmp.Substring(0, este);
            int sjson = pack.IndexOf("{");
            if (sjson < 0) return;
            tmp = pack.Substring(sjson);
            int ejosn = tmp.Length - 1;
            string json = tmp.Substring(0, ejosn);
            payload = (JObject)((JObject)JsonConvert.DeserializeObject(json))["CurrentPacket"]["Data"];
        }
    }
}
