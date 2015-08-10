using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRCBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IRCConfig conf = new IRCConfig();
            conf.name = "ParziBot";
            conf.nick = "ParziBot";
            conf.port = 6667;
            conf.channel = "#zerocubed";
            conf.server = "irc.esper.net";
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