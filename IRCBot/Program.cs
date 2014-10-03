using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Microsoft.Win32;

namespace IRCBot
{
    internal struct IRCConfig
    {
        public bool joined;
        public string server;
        public int port;
        public string nick;
        public string name;
        public string channel;

    }

    internal class IRCBot : IDisposable
    {
        private TcpClient IRCConnection = null;
        private IRCConfig config;
        private NetworkStream ns = null;
        private StreamReader sr = null;
        private StreamWriter sw = null;

        public IRCBot(IRCConfig config)
        {
            this.config = config;
        }

        public void Connect()
        {
            try
            {
                IRCConnection = new TcpClient(config.server, config.port);
            }
            catch
            {
                Console.WriteLine("Connection Error");
            }

            try
            {
                ns = IRCConnection.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sendData("USER", config.nick + " 0 * " + config.name);
                sendData("NICK", config.nick);
            }
            catch
            {
                Console.WriteLine("Communication error");
                throw;
            }
        }

        public void sendData(string cmd, string param)
        {
            if (param == null)
            {
                sw.WriteLine(cmd);
                sw.Flush();
                Console.WriteLine(cmd);
            }
            else
            {
                sw.WriteLine(cmd + " " + param);
                sw.Flush();
                Console.WriteLine(cmd + " " + param);
            }
        }

        public void chat(string chan, string msg)
        {
            sendData("PRIVMSG "+chan+" :"+msg, null);
        }

        public void me(string chan, string msg)
        {
            sendData("PRIVMSG " + chan + " :\u0001ACTION " + msg + "\u0001", null);
        }

        public void IRCWork()
        {
            string[] ex;
            string data;
            bool shouldRun = true;
            bool copycat = false;
            string datal = "";
            while (shouldRun)
            {
                data = sr.ReadLine();
                datal = data.ToLower();
                //Console.WriteLine(data + "\n"); //Used for debugging
                ex = data.Split(new char[] { ' ' }, 5); //Split the data into 5 parts
                if (!config.joined) //if we are not yet in the assigned channel
                {
                    if (ex[1] == "MODE") //Normally one of the last things to be sent (usually follows motd)
                    {
                        sendData("JOIN", config.channel); //join assigned channel
                        config.joined = true;
                    }
                }

                if (ex[0] == "PING")  //respond to pings
                {
                    sendData("PONG", ex[1]);
                }


                if ((datal.Contains("hey") || datal.Contains("hello") || datal.Contains("hi")) && datal.Contains("parzibot"))
                {
                    string sender = data.Split('!')[0].Replace(":", "");
                    Console.WriteLine(data+"\n");
                    chat(config.channel, "Hello, " + sender + "!");
                }

                if (datal.Contains("parzibot copycat"))
                {
                    copycat = !copycat;
                    chat(config.channel, "Copycat now " + copycat.ToString());
                }

                if (copycat)
                {
                    chat(config.channel, data.Split(new char[] { ':' }, 3)[2]);
                }

                if (datal.Contains("when parzibot"))
                {
                    me(config.channel, datal.Split(new char[] { ':' }, 3)[2].Replace("when parzibot ", ""));
                }

                if (datal.Contains("parzibot") && datal.Contains("leave") && (datal.Contains("irc") || datal.Contains("channel") || datal.Contains("chat")))
                {
                    sendData("QUIT", null);
                    shouldRun = false;
                }

                if (datal.Contains("parzibot") && datal.Contains("power level"))
                {
                    string sender = data.Split('!')[0].Replace(":", "");
                    chat(config.channel, sender + ": It's over 9000!");
                }
            }
        }

        public void Dispose()
        {
            if (sr != null)
                sr.Close();
            if (sw != null)
                sw.Close();
            if (ns != null)
                ns.Close();
            if (IRCConnection != null)
                IRCConnection.Close();
        }
    }


    internal class Program
    {
        private static void Main(string[] args)
        {
            IRCConfig conf = new IRCConfig();
            conf.name = "ParziBot";
            conf.nick = "ParziBot";
            conf.port = 6667;
            conf.channel = "#Capacity-Dev";
            conf.server = "irc.boltirc.net";
            using (var bot = new IRCBot(conf))
            {
                conf.joined = false;
                bot.Connect();
                bot.IRCWork();
            }
            Console.WriteLine("Bot closed");
            Console.ReadLine();
        }
    }
}