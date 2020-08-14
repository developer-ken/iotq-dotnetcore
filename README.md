# iotq-dotnetcore
Interact with iotbot through websocket and http

通过Websocket和Http与iotbot交互

# WARNING 警告
This project is for my own use. Due to my poor coding skill, it will be very hard to maintain.
Think twice if you want to have a dependency on it in your project. Just use it like a black box.

这个工程是我自用的。因为技术不佳，这些代码很难维护。使用前请三思。当黑箱用就好。

Do not forget to star my projcet if you like it.记得右上角给颗星星哦

# Functions 功能

_Receivers 接收类

- 接收群消息 Receive group messages         □ Not fully supported
-  ->文本 Text    √ Supported
-  ->图片 Picture √ Supported
-  ->语音 Voice   √ Supported
-  ->特殊 Special □ Not fully supported 不完全支持
- 接收入群申请 Receive group enter requests □ Not fully supported
-  ->获得入群者信息 get applicant info      √ Supported
-  ->获得加群答案回复 get answers           × Not supported by IOTBOT
- 接收私聊 Receive private messages         × Not available yet
-  ->文本 Text    × Not available yet
-  ->图片 Picture × Not available yet
-  ->语音 Voice   × Not available yet
-  ->特殊 Special × Not available yet
- 接收进群退群消息 Member enter/quit event  √ Supported   

_Actions 操作类

- 发送群消息 Send group messages            √ Fully supported
- 发送私聊消息 Send private messages        √ Fully supported
- 撤回群消息 Delete group messages          √ Fully supported
- 获取加入群列表 Get group list             √ Fully supported
- 获取群成员列表 Get group member list      √ Unstable but supported
- 获取登录Cookies Get login cookies         √ Fully supported
- 群全员禁言 Group SHUTUP ALL               √ Fully supported
- 群成员禁言 Group SHUTUP member            √ Fully supported
- 处理入群申请 Answer group enter req       □ Not fully supported
- ->同意/拒绝  Accept/Refuse      √ Supported
- ->拒绝理由 Reason for refuse    √ Supported β
- ->不再接收申请 Ingore forever   √ Supported β

# Demo: Print the message received from group 使用示例：输出来自群的消息

        static void Main(string[] args)
        {
            pThreadPool pool = new pThreadPool();
            EventHost host = new EventHost("192.168.1.233333", 1234567890, pool, logger);//这里改成你的机器人ip和QQ号 Change this to the IP and QQ UIN of your bot

            IOTQEventHandler evh = new IOTQEventHandler(host);//创建事件管理器 Create the event handler

            evh.onGroupMessageReceive += Evh_onGroupMessageReceive;//注册接收群消息的事件 Register the event we need

            while (true) { Thread.Sleep(10000); }//卡死主线程不让程序直接退出 Block main thread so our programe won't quit
        }

        private static void Evh_onGroupMessageReceive(object sender, GroupMessageReceiveEventArgs args)//用于接收消息的方法 Methold to receive messages
        {
            Console.WriteLine(args.message.plaintext);//显示收到的文本消息 Print the message received
        }

        public static void logger(string cat,string msg,ConsoleColor b,ConsoleColor f)//日志处理器  输出日志的时候会被调用 Log manager, called when printing log
        {
            Console.BackgroundColor = b;
            Console.ForegroundColor = f;
            Console.WriteLine("[" + cat + "]" + msg);//简单的把日志显示在屏幕上 Just print the log on the console
        }

# CQ2IOT? Anything to do with CQ?  为啥叫CQ2IOT？和CQ有必然联系吗？

I used to write plugins for CQ. At first, I intened to write an adapter so I can use my old plugins on IOTBOT.

我本来是写CQ插件的，打算写个adapter让老插件直接能用，所以起了这个名字

Then I realized that it is much more easy to modify the plugins as well. (lol)

后来发现插件一起改比较简单(懒...)

So this project WON'T run CQ plugins directly, and now it has nothing to do with CQ.

所以它并不能直接运行CQ的插件，而且现在这个工程和CQ半毛钱关系都没有。
