using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Threading;
using CQ2IOT.JsonModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Runtime.CompilerServices;

namespace CQ2IOT
{
    public class WebAPI
    {
        public long me_qquid;
        public string host;
        public logger_ logger;

        public delegate void logger_(string cato, string msg, ConsoleColor backcolor = ConsoleColor.Black, ConsoleColor frontcolor = ConsoleColor.White);

        /// <summary>
        /// 发送get请求
        /// </summary>
        /// <param name="url">目标网址</param>
        /// <returns></returns>
        public static string _get(string url)
        {
                string retString = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
        }

        /// <summary>
        /// 发送post请求
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="postData">发送的json</param>
        /// <returns></returns>
        public static string _post(string url, string postData)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 调用LuaApi
        /// </summary>
        /// <param name="funcname">函数名</param>
        /// <param name="jsonstring">传入json</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public string LuaApiCaller(string funcname, string jsonstring, int timeout = 30)
        {
            string url = "http://" + host + ":8888/v1/LuaApiCaller?qq=" + me_qquid + "&funcname=" + funcname + "&timeout=" + timeout;
            return _post(url, jsonstring);
        }

        /// <summary>
        /// 调用LuaApi
        /// </summary>
        /// <param name="funcname">Api名称</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public string LuaApiCaller(string funcname, int timeout = 30)
        {
            string url = "http://" + host + ":8888/v1/LuaApiCaller?qq=" + me_qquid + "&funcname=" + funcname + "&timeout=" + timeout;
            return _get(url);
        }


        /// <summary>
        /// 发送一条文字信息
        /// </summary>
        /// <param name="target">发送到的目标QQ号或群号</param>
        /// <param name="msg">消息文字内容(支持iotqq指令)</param>
        /// <param name="target_type">目标类型</param>
        /// <param name="through_group">通过指定群发动临时聊天(如果有)</param>
        /// <param name="at_user">要at的人的QQ(如果有)</param>
        /// <returns></returns>
        public JsonModel.ReqResult sendTextMessage(long target, string msg, int target_type, long through_group = 0, long at_user = 0)
        {
            JsonModel.TextMessage msgi = new JsonModel.TextMessage()
            {
                atUser = at_user,
                content = msg,
                groupid = through_group,
                sendMsgType = "TextMsg",
                sendToType = target_type,
                toUser = target
            };
            string json = System.Text.Json.JsonSerializer.Serialize(msgi);
            string str = LuaApiCaller("SendMsg", json);
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(str);
            if (retv == null) return null;
            if (retv.Ret == 0) logger("debug_sendmsg_result", "Succeed.");
            else logger("debug_sendmsg_result", "Fail.");
            return retv;
        }

        public JsonModel.ReqResult sendXmlMessage(long target, string xml, int target_type, long through_group = 0, long at_user = 0)
        {
            JObject jb = new JObject();
            jb.Add("atUser", at_user);
            jb.Add("content", xml);
            jb.Add("groupid", through_group);
            jb.Add("sendMsgType", "XmlMsg");
            jb.Add("sendToType", target_type);
            jb.Add("toUser", target);
            string json = jb.ToString();
            json = json.Replace("<", "\\u003c").Replace(">", "\\u003e");
            string str = LuaApiCaller("SendMsg", json);
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(str);
            if (retv.Ret == 0) logger("debug_sendmsg_result", "Succeed.");
            else logger("debug_sendmsg_result", "Fail.");
            return retv;
        }

        /// <summary>
        /// 发图片
        /// </summary>
        /// <param name="target">目标QQ号或群号</param>
        /// <param name="msg">文本消息内容</param>
        /// <param name="target_type">目标类型</param>
        /// <param name="picurl">图片地址</param>
        /// <param name="through_group">临时聊天通过的群号(0=不通过临时聊天)</param>
        /// <param name="at_user">被at的人</param>
        /// <returns></returns>
        public JsonModel.ReqResult sendPicMessage(long target, string msg, int target_type, string picurl, long through_group = 0, long at_user = 0)
        {
            JsonModel.PicMessage msgi = new JsonModel.PicMessage()
            {
                atUser = at_user,
                content = msg,
                groupid = through_group,
                sendMsgType = "PicMsg",
                sendToType = target_type,
                toUser = target,
                picUrl = picurl,
                fileMd5 = "",
                picBase64Buf = ""
            };
            string json = System.Text.Json.JsonSerializer.Serialize(msgi);
            logger("debug_sendmsg_reqjson", json);
            logger("debug_sendmsg_step", "Requesting iotbot server...");
            string str = LuaApiCaller("SendMsg", json);
            logger("debug_sendmsg_step", "End requesting iotbot server. Done.");
            logger("debug_sendmsg_retjson", str);
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(str);
            if (retv.Ret == 0) logger("debug_sendmsg_result", "Succeed.");
            else logger("debug_sendmsg_result", "Fail.");
            return retv;
        }

        /// <summary>
        /// 获得当前帐号所加入的群列表
        /// </summary>
        /// <returns></returns>
        public List<JsonModel.Group> getGroupList()
        {
            string nexttoken = "";
            JsonModel.GroupListRet data_get = new JsonModel.GroupListRet();
            List<JsonModel.Group> gplist = new List<JsonModel.Group>();
            do
            {
                JsonModel.GetListReq reqdata = new JsonModel.GetListReq()
                {
                    GroupUin = 0,
                    LastUin = 0,
                    NextToken = nexttoken
                };
                string json = System.Text.Json.JsonSerializer.Serialize(reqdata);
                logger("debug_listgroup_reqjson", json);
                logger("debug_listgroup_step", "Requesting iotbot server...");
                string str = LuaApiCaller("GetGroupList", json);
                logger("debug_listgroup_step", "End requesting iotbot server. Done.");
                logger("debug_listgroup_retjson", str);
                if (str == null) continue;
                data_get = System.Text.Json.JsonSerializer.Deserialize<JsonModel.GroupListRet>(str);
                if (data_get == null) continue;
                gplist.AddRange(data_get.TroopList);
                nexttoken = data_get.NextToken;
            } while (nexttoken.Length > 0);
            return gplist;
        }

        /// <summary>
        /// 获得群成员列表
        /// ！不可靠操作 请在成功获取后缓存 不要重复调用
        /// </summary>
        /// <param name="groupuin">群号</param>
        /// <returns>群列表</returns>
        public List<JsonModel.QQUser> getGroupMemberList(long groupuin)
        {
            long lastuin = 1;
            JsonModel.GroupMemberListRet data_get = new JsonModel.GroupMemberListRet();
            List<JsonModel.QQUser> gplist = new List<JsonModel.QQUser>();
            do
            {
                JsonModel.GetListReq reqdata = new JsonModel.GetListReq()
                {
                    GroupUin = groupuin,
                    LastUin = lastuin,
                    NextToken = ""
                };
                string json = System.Text.Json.JsonSerializer.Serialize(reqdata);
                //logger("debug_listgroup_reqjson", json);
                logger("debug_listgroup_step", "Requesting iotbot server...");
                string str = LuaApiCaller("GetGroupUserList", json);
                logger("debug_listgroup_step", "End requesting iotbot server. Done.");
                //logger("debug_listgroup_retjson", str);
                if (str == null || str.Length < 7)
                {
                    logger("debug_listgroup_warn", "Failure: Fail to fetch member list. Try again...");
                    Thread.Sleep(1000);
                    continue;
                }
                data_get = System.Text.Json.JsonSerializer.Deserialize<JsonModel.GroupMemberListRet>(str);
                logger("debug_listgroup_step", "Check for dumplicate items...");
                if (data_get.MemberList != null) foreach (JsonModel.QQUser qu in data_get.MemberList)
                    {
                        bool match = false;
                        foreach (QQUser qq in gplist)
                        {
                            if (qq.Equals(qu))
                            {
                                match = true;
                                logger("debug_listgroup_step", "HIT  " + qu.MemberUin);
                                break;
                            }
                        }
                        if (!match) gplist.Add(qu);
                    }
                lastuin = data_get.LastUin;
                if (lastuin > 0)
                {
                    logger("debug_listgroup_step", "List not complete. Wait 1 sec(s) to fetch next part.");
                    Thread.Sleep(1500);
                }
            } while (lastuin > 0);
            return gplist;
        }

        /// <summary>
        /// 禁言用户
        /// </summary>
        /// <param name="group">群号</param>
        /// <param name="qq">qq号</param>
        /// <param name="len">时长(0=解禁)</param>
        /// <returns></returns>
        public JsonModel.ReqResult shutUserUp(long group, long qq, int len)
        {
            JsonModel.ShutupReq req = new JsonModel.ShutupReq()
            {
                GroupID = group,
                ShutUID = qq,
                ShutTime = len,
                ShutUpType = 0
            };
            string json = System.Text.Json.JsonSerializer.Serialize(req);
            logger("debug_shutupuser_step", "Requesting iotbot server...");
            string ret = LuaApiCaller("ShutUp", json);
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(ret);
            logger("debug_shutupuser_step", "Done");
            return retv;
        }

        /// <summary>
        /// 全体禁言
        /// </summary>
        /// <param name="group">群号</param>
        /// <param name="enable">true=禁,false=解禁</param>
        /// <param name="len">时长(min),此处通常无效</param>
        /// <returns></returns>
        public JsonModel.ReqResult shutAll(long group, bool enable = true, int len = 0)
        {
            JsonModel.ShutupReq req = new JsonModel.ShutupReq()
            {
                GroupID = group,
                ShutUID = enable ? 15 : 0,
                ShutTime = len,
                ShutUpType = 1
            };
            string json = System.Text.Json.JsonSerializer.Serialize(req);
            logger("debug_shutupuser_step", "Requesting iotbot server...");
            string ret = LuaApiCaller("ShutUp", json);
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(ret);
            logger("debug_shutupuser_step", "Done");
            return retv;
        }

        /// <summary>
        /// 获取登录QQ对应网页所需的Cookie
        /// </summary>
        /// <returns></returns>
        public CookieCollection getLoginCookies()
        {
            string ret = LuaApiCaller("GetUserCook");
            JObject json = (JObject)JsonConvert.DeserializeObject(ret);
            string cookiestr = json.Value<string>("Cookies");
            CookieCollection ck = new CookieCollection();
            string[] parts = cookiestr.Split(';');
            foreach(string st in parts)
            {
                if (st == null || st.Length <= 1) continue;
                string k = st.Split('=')[0];
                string v = st.Split('=')[1];
                ck.Add(new Cookie(k, v));
            }
            return ck;
        }

        /// <summary>
        /// 处理入群申请
        /// </summary>
        /// <param name="seq">申请信息(接收到申请时的完整EventData)</param>
        /// <param name="pass">true=同意,false=拒绝</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool doGroupEnterReq(JObject seq, bool pass, string msg = null)
        {
            JObject jb = (JObject)seq.DeepClone();
            jb["Action"] = pass ? 11 : 12;//14忽略
            jb["RefuseContent"] = msg;
            string ret = LuaApiCaller("AnswerInviteGroup", jb.ToString());
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(ret);
            return retv.Ret == 0;
        }

        public JsonModel.ReqResult RevokeMessage(long group, int seq, long rand, bool retry = true)
        {
            JObject jb = new JObject();
            jb.Add("GroupID", group);
            jb.Add("MsgSeq", seq);
            jb.Add("MsgRandom", rand);
            string ret = LuaApiCaller("RevokeMsg", jb.ToString());
            if (ret == null && retry) return RevokeMessage(group, seq, rand, false);
            if (ret == null) return null;
            JsonModel.ReqResult retv = System.Text.Json.JsonSerializer.Deserialize<JsonModel.ReqResult>(ret);
            return retv;
        }
    }
}