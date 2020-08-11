using System;
using System.Collections.Generic;
using System.Text;

namespace CQ2IOT.JsonModel
{
    public class MsgType
    {
        public const int FRIEND = 1;
        public const int GROUP = 2;
        public const int STRANGER = 3;
    }
    public class ReqResult
    {
        public string Msg { get; set; }
        public int Ret { get; set; }
    }
    public class Message
    {
        public int MessageType;
        public long toUser { get; set; }
        public long groupid { get; set; }
        public long atUser { get; set; }
        public int sendToType { get; set; }
        public string content { get; set; }
        public string sendMsgType { get; set; }
    }

    public class TextMessage : Message
    {

    }

    public class tTextMessage : Message
    {
    }

    public class PicMessage : Message
    {
        public string picUrl { get; set; }
        public string picBase64Buf { get; set; }
        public string fileMd5 { get; set; }
    }

    public class Group
    {
        public long GroupId { get; set; }
        /// <summary>
        /// 当前成员数量
        /// </summary>
        public int GroupMemberCount { get; set; }
        public string GroupName { get; set; }
        /// <summary>
        /// 最近的公告
        /// </summary>
        public string GroupNotice { get; set; }
        public long GroupOwner { get; set; }
        /// <summary>
        /// 群大小(总人数)
        /// </summary>
        public int GroupTotalCount { get; set; }
    }

    public class GroupListRet
    {
        public List<Group> TroopList { get; set; }
        public string NextToken { get; set; }
        public int Count { get; set; }
    }

    public class GetListReq
    {
        public string NextToken { get; set; }
        public long GroupUin { get; set; }
        public long LastUin { get; set; }
    }

    public class GroupMemberListRet
    {
        /*
        "Count": 250,
        "GroupUin": 1040080499,
        "LastUin": 1709235675,
        "MemberList": <LIST>
        */
        public List<QQUser> MemberList { get; set; }
        public string NextToken { get; set; }
        public long GroupUin { get; set; }
        public long LastUin { get; set; }
        public int Count { get; set; }
    }

    public class QQUser
    {
        public string AutoRemark { get; set; }
        public string Email { get; set; }
        public string GroupCard { get; set; }
        public string Memo { get; set; }
        public string NickName { get; set; }
        public string ShowName { get; set; }
        public string SpecialTitle { get; set; }
        public int Age { get; set; }
        public int CreditLevel { get; set; }
        public int FaceId { get; set; }
        public int Gender { get; set; }
        public int GroupAdmin { get; set; }
        public int JoinTime { get; set; }
        public int LastSpeakTime { get; set; }
        public int MemberLevel { get; set; }
        public long MemberUin { get; set; }
        public int Status { get; set; }

        public override bool Equals(object obj)
        {
            QQUser qu = (QQUser)obj;
            return MemberUin.Equals(qu.MemberUin);
        }
        public override int GetHashCode()
        {
            return MemberUin.GetHashCode();
        }
    }

    public class ShutupReq
    {
        public long GroupID { get; set; }
        public long ShutUID { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int ShutUpType { set; get; }
        /// <summary>
        /// 禁言时长，单位为分钟
        /// </summary>
        public int ShutTime { set; get; }
    }
}
