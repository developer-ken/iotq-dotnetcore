using CQ2IOT.JsonModel;
using System.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CQ2IOT.Events;

namespace CQ2IOT
{
    public class EventHost
    {
        /// <summary>
        /// 事件主机维护的连接
        /// ⚠外部操作可能会使事件主机工作不正常
        /// </summary>
        public ClientWebSocket client;
        public logger_ logger;
        public pThreadPool threadpool;

        /// <summary>
        /// 事件主机异步操作销毁handle
        /// 可以用它终止事件主机所有的异步操作
        /// ⚠外部操作可能会使事件主机工作不正常
        /// </summary>
        public List<IDisposable> disposhandle;
        /// <summary>
        /// 任意数据包传入时触发
        /// ⚠任何时候不允许清空 事件主机内部也使用本事件传递数据
        /// </summary>
        public event EventTriggerEvent onEventTrigger;
        /// <summary>
        /// 连接初始化结束时触发
        /// ⚠任何时候不允许清空 事件主机内部也使用本事件传递数据
        /// </summary>
        public event ConnectionInitEvent onConnectionInit;
        /// <summary>
        /// EventHost是否已就绪
        /// “就绪”状态指的是EventHost已完成了所有的协商且可以接受事件
        /// </summary>
        public bool ready = false;

        private Task heartbeat;
        private Task receiver;
        private CancellationToken ctoken;
        public long qq;
        int hbtime = 5000;
        string conn_sid;
        string host;

        public delegate void EventTriggerEvent(NetPack pack);
        public delegate void ConnectionInitEvent(InitPack pack);

        public delegate void logger_(string cato, string msg, ConsoleColor backcolor = ConsoleColor.Black, ConsoleColor frontcolor = ConsoleColor.White);


        private DateTime start;
        /// <summary>
        /// IOTQ协议 事件主机
        /// </summary>
        /// <param name="host">目标IP或主机地址</param>
        /// <param name="qq">当前登录的qq</param>
        /// <param name="threadpool">事件发生时用于处理的线程池</param>
        /// <param name="logger">日志管理器，将处理所有日志信息</param>
        /// <param name="buffersize">接收缓冲区大小  过大导致内存浪费，过小导致多次循环接收</param>
        public EventHost(string host, long qq, pThreadPool threadpool, logger_ logger, int buffersize = 1024)
        {
            start = DateTime.Now;
            this.logger = logger;
            this.threadpool = threadpool;
            this.host = host;
            heartbeat = new Task(() =>
            {
                logger("eventhost_connection", "HB manager started.");
                while (!ctoken.IsCancellationRequested)
                {
                    try
                    {
                        logger("eventhost_connection", "<heartbeat> sent");
                        client.SendAsync(Encoding.UTF8.GetBytes("2"), WebSocketMessageType.Text, true, ctoken).Wait();
                        Thread.Sleep(hbtime);
                    }
                    catch (Exception err)
                    {
                        Die();
                        logger("eventhost_fatal", "heartbeat manager crashed:" + err.Message, ConsoleColor.DarkRed);
                        logger("eventhost_fatal", "recovering...");
                        conn(host, logger);
                        Thread.Sleep(1000);
                    }
                }
                logger("eventhost_connection", "HB manager cancelled by token.");
            });
            receiver = new Task(() =>
            {
                logger("eventhost_receiver", "started.");
                while (!ctoken.IsCancellationRequested)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        Task<WebSocketReceiveResult> t;
                        do
                        {
                            var buffer = new byte[buffersize];
                            t = client.ReceiveAsync(buffer, ctoken);
                            t.Wait();
                            logger("eventhost_receiver_debug", "new pack comming in, SIZE=" + t.Result.Count);
                            byte[] bRec = new byte[t.Result.Count];
                            Array.Copy(buffer, bRec, t.Result.Count);
                            sb.Append(Encoding.UTF8.GetString(bRec));
                            if (!t.Result.EndOfMessage)
                            {
                                logger("eventhost_receiver_debug", " - Large pack truncated, receiving next part...", ConsoleColor.DarkBlue);
                            }
                        } while (!t.Result.EndOfMessage);
                        if (InitPack.isInitPack(sb.ToString()))
                        {
                            threadpool.submitWorkload(() =>
                            {
                                InitPack i = new InitPack(sb.ToString());
                                onConnectionInit.Invoke(i);
                            });
                        }
                        else
                        {
                            threadpool.submitWorkload(() =>
                            {
                                NetPack packet = new NetPack(sb.ToString());
                                if (packet.code == 3 && (packet.eventstr == null || packet.eventstr.Length < 1))
                                {
                                    //心跳包
                                }
                                else
                                    onEventTrigger.Invoke(packet);
                            });
                        }
                        /*
                        if (pack.IndexOf("[\"OK\"]") > -1)
                        {
                            logger("eventhost_connection", "ready.");
                            ready = true;
                        }
                        if(pack.IndexOf("[\"当前已存在活动的WebSocket 已为您切换当前Socket\"]") > -1)
                        {
                            logger("eventhost_connection", "TARGET_SWITCHED - ready.");
                            ready = true;
                        }
                        if (pack.IndexOf("[\"尚未在线\"]") > -1) {
                            logger("eventhost_fatal", "Target not online. Will deleberately crash.");
                            ready = false;
                            throw new Exception("Target not online. Try again later.");
                        }
                        */
                        //logger("eventhost_receiver", "pack=" + pack);
                    }
                    catch (Exception err)
                    {
                        Die();
                        logger("eventhost_fatal", "main receiver crashed:" + err.Message, ConsoleColor.DarkRed);
                        logger("eventhost_fatal", "recovering...");
                        conn(host, logger);
                        Thread.Sleep(500);
                    }
                }
                logger("eventhost_receiver", "cancelled by ctoken.", ConsoleColor.DarkBlue);
            });
            this.qq = qq;
            conn(host, logger);
            onConnectionInit += ConnectionInit;
            onEventTrigger += PackRecv;
            if (heartbeat.Status != TaskStatus.Running) heartbeat.Start();
            if (receiver.Status != TaskStatus.Running) receiver.Start();
        }

        private void ConnectionInit(InitPack pack)
        {
            hbtime = pack.hbinterval;
            conn_sid = pack.sid;
            logger("eventhost_connection", "basic connection good");
            client.SendAsync(Encoding.UTF8.GetBytes("421[\"GetWebConn\",\"" + qq + "\"]"), WebSocketMessageType.Text, true, ctoken).Wait();
            logger("eventhost_connection", "iotq handshake sent");
        }

        private void PackRecv(NetPack pack)
        {
            switch (pack.eventstr)
            {
                case "OK":
                case "当前已存在活动的WebSocket 已为您切换当前Socket":
                    //连接成功
                    logger("eventhost_connection", "iotq handshake done.");
                    logger("eventhost_connection", "hbinterval=" + hbtime + "  sid=" + conn_sid, ConsoleColor.DarkBlue);
                    logger("eventhost", "Done.(" + (DateTime.Now - start).ToString() + ")", ConsoleColor.DarkGreen);
                    ready = true;
                    break;
            }
        }

        /// <summary>
        /// 关闭事件主机到目标的连接
        /// </summary>
        public void Die()
        {
            ready = false;
            try
            {
                logger("eventhost_connection_fatal", "connection is DEAD", ConsoleColor.DarkRed);
                client.CloseAsync(WebSocketCloseStatus.ProtocolError, "Fatal damage happened to the protocol.", ctoken).Wait();
            }
            catch { }
        }

        public void reconnect()
        {
            Die();
            conn(host, logger);
        }

        private void conn(string host, logger_ logger)
        {
            ready = false;
            try
            {
                client = new ClientWebSocket();
                logger("eventhost_connection", "connecting...");
                client.ConnectAsync(new Uri("ws://" + host + ":8888/socket.io/"), ctoken).Wait();
            }
            catch (Exception err)
            {
                Die();
                logger("eventhost_fatal", "crashed:" + err.Message, ConsoleColor.DarkRed);
                logger("eventhost_fatal", "recovering...");
                conn(host, logger);
            }
        }
    }
}
