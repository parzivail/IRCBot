using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace IRCBot
{
    internal class IRCBot : IDisposable
    {
        private TcpClient IRCConnection = null;
        public IRCConfig config;
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
            sendData("PRIVMSG " + chan + " :" + msg + "\n", null);
        }

        public void me(string chan, string msg)
        {
            sendData("PRIVMSG " + chan + " :\u0001ACTION " + msg + "\u0001", null);
        }

        public bool ContainsP(string haystack, string needle)
        {
            foreach (string s in new string[] { " ", ",", ":", "!" })
            {
                if (haystack.Contains(s + needle) || haystack.Contains(needle + s)) return true;
            }
            return false;
        }

        public void IRCWork()
        {
            string[] ex;
            string data;
            bool shouldRun = true;
            string datal = "";
            while (shouldRun)
            {
                data = sr.ReadLine();
                datal = data.ToLower();
                Console.WriteLine(data + "\n"); //Used for debugging
                ex = data.Split(new char[] { ' ' }, 5); //Split the data into 5 parts
                if (!config.joined) //if we are not yet in the assigned channel
                {
                    if (ex[1] == "MODE") //Normally one of the last things to be sent (usually follows motd)
                    {
                        Console.Clear();
                        sendData("JOIN", config.channel); //join assigned channel
                        Console.Write("Password: ");
                        sendData("PRIVMSG NickServ :identify parzivail " + Console.ReadLine() + "\n", null);
                        config.joined = true;
                        me(config.channel, "whirrs idly");
                    }
                }
                IRCResponse.doWork(this, ex, data, datal);                
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
}
