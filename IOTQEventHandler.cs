using CQ2IOT.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CQ2IOT
{
    public class IOTQEventHandler
    {
        public EventHost host;
        public event FriendAddRequestEvent onFriendAddRequest;
        public event GroupMessageReceiveEvent onGroupMessageReceive;
        public event GroupMessageReceiveEvent onGroupMessageSendOkay;
        public event GroupMemberIncreaseEvent onGroupMemberIncrease;
        public event GroupMemberDecreaseEvent onGroupMemberDecrease;
        public event GroupEnterRequestEvent onGroupEnterRequest;

        public event UnknownEventEvent onUnknownEventReceived;

        public delegate void FriendAddRequestEvent(object sender, FriendAddRequestEventArgs args);
        public delegate void GroupMessageReceiveEvent(object sender, GroupMessageReceiveEventArgs args);
        public delegate bool UnknownEventEvent(object sender, NetPack pack);
        public delegate void GroupMemberIncreaseEvent(object sender, GroupMemberIncreaseEventArgs args);
        public delegate void GroupMemberDecreaseEvent(object sender, GroupMemberDecreaseEventArgs args);
        public delegate void GroupEnterRequestEvent(object sender, GroupEnterRequestEventArgs args);

        public IOTQEventHandler(EventHost host)
        {
            this.host = host;
            while (!host.ready) ;
            host.onEventTrigger += RecvPacket;
            host.logger("IOTQ", "Event Handler Ready.", ConsoleColor.DarkGreen);
        }
        public void RecvPacket(NetPack pack)
        {
            switch (pack.eventstr)
            {
                case "OnGroupMsgs"://群消息
                    GroupMessageReceiveEventArgs e = new GroupMessageReceiveEventArgs(pack.payload);
                    if (e.user.qq != host.qq)//不是自己
                        onGroupMessageReceive.Invoke(this, e);
                    else
                        onGroupMessageSendOkay.Invoke(this, e);
                    break;
                case "OnFriendMsgs"://好友私聊

                    break;
                case "OnEvents"://群其它消息
                    {
                        string eventname = pack.payload.Value<string>("EventName");
                        switch (eventname)
                        {
                            case "ON_EVENT_GROUP_REVOKE"://消息撤回
                                break;
                            case "ON_EVENT_GROUP_SHUT"://禁言
                                break;
                            case "ON_EVENT_GROUP_ADMINSYSNOTIFY"://加群申请
                                onGroupEnterRequest.Invoke(this, new GroupEnterRequestEventArgs(pack.payload));
                                break;
                            case "ON_EVENT_GROUP_JOIN"://入群成功
                                onGroupMemberIncrease.Invoke(this, new GroupMemberIncreaseEventArgs(pack.payload));
                                break;
                            case "ON_EVENT_GROUP_EXIT"://退群
                                onGroupMemberDecrease.Invoke(this, new GroupMemberDecreaseEventArgs(pack.payload));
                                break;
                            case "ON_EVENT_GROUP_ADMIN"://管理员变更
                                break;
                            case "ON_EVENT_FRIEND_ADDED"://被加好友
                                onFriendAddRequest.Invoke(this, new FriendAddRequestEventArgs(pack.payload));
                                break;
                            default:
                                if (!onUnknownEventReceived.Invoke(this, pack)) host.logger("IOTQ_WARN", "Unrecognized subevent name. Ignore the pack..", ConsoleColor.DarkYellow);
                                break;
                        }
                    }
                    break;
                default:
                    if (!onUnknownEventReceived.Invoke(this, pack)) host.logger("IOTQ_WARN", "Unrecognized event name.", ConsoleColor.DarkYellow);
                    break;
            }
        }
    }
}
