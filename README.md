# iotq-dotnetcore
Interact with iotbot through websocket and http

通过Websocket和Http与iotbot交互

This project is for my own use. Due to my poor coding skill, it will be very hard to maintain.
Think twice if you want to have a dependency on it in your project. Just use it like a black box.

这个工程是我自用的。因为技术不佳，这些代码很难维护。使用前请三思。当黑箱用就好。

Do not forget to star my projcet if you like it.记得右上角给颗星星哦

#How To Use 如何使用

        static void Main(string[] args)
        {
            pThreadPool pool = new pThreadPool();
            EventHost host = new EventHost("192.168.1.107", 3098118320, pool, logger);//这里改成你的机器人ip和QQ号
            //上面的代码初始化了IOTQQ的通讯库

            IOTQEventHandler evh = new IOTQEventHandler(host);//创建事件管理器，用于把来自IOTQQ的事件信息分类

            evh.onGroupMessageReceive += Evh_onGroupMessageReceive;//表示接收的群消息传一份给Evh_onGroupMessageReceive

            while (true) { Thread.Sleep(10000); }//卡死主线程
        }

        private static void Evh_onGroupMessageReceive(object sender, GroupMessageReceiveEventArgs args)//用于接收消息的方法
        {
            Console.WriteLine(args.message.plaintext);//显示收到的文本消息
        }

        public static void logger(string cat,string msg,ConsoleColor b,ConsoleColor f)//日志处理器  生成日志的时候会被调用
        {
            Console.BackgroundColor = b;
            Console.ForegroundColor = f;
            Console.WriteLine("[" + cat + "]" + msg);//简单的把日志显示在屏幕上
        }
