using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace IRCBot
{
    class IRCResponse
    {
        public static void doWork(IRCBot bot, string[] ex, string data, string datal)
        {
            if (ex[0] == "PING")  //respond to pings
            {
                bot.sendData("PONG", ex[1]);
            }
            else if ((bot.ContainsP(datal, "hey") || bot.ContainsP(datal, "hello") || bot.ContainsP(datal, "hi")) && bot.ContainsP(datal, "parzibot"))
            {
                string sender = data.Split('!')[0].Replace(":", "");
                Console.WriteLine(data + "\n");
                bot.chat(bot.config.channel, "Hello, " + sender + "!");
            }
            else if (bot.ContainsP(datal, "functions") && bot.ContainsP(datal, "-p"))
            {
                bot.chat(bot.config.channel, "My functions:");
                bot.chat(bot.config.channel, "\\");
                string[] f = { "Time: just ask what time it is", "Copycat: 'parzibot copycat'", "Ping: 'parzibot, ping <server>'", "Power Level: 'parzibot, what's the power level' or similar", "Leave: 'parzibot, leave irc' or similar" };
                foreach (string func in f)
                {
                    bot.chat(bot.config.channel, " |- " + func);
                }
            }
            else if (bot.ContainsP(datal, "-!p "))
            {
                bot.me(bot.config.channel, datal.Split(new char[] { ':' }, 3)[2].Replace("-!p ", ""));
            }
            else if (bot.ContainsP(datal, "what time is it"))
            {
                bot.chat(bot.config.channel, "It is currently " + DateTime.Now.ToString("h:mm:ss tt") + " Eastern Time (US and Canada)");
            }
            else if (bot.ContainsP(datal, "parzibot, ping "))
            {
                string web = datal.Replace("parzibot, ping ", "\u1106").Split('\u1106')[1];
                try
                {
                    PingReply p = new Ping().Send(web);
                    bot.chat(bot.config.channel, "Pinged " + web + ": " + p.Status + ", " + p.RoundtripTime + "ms");
                }
                catch
                {
                    bot.chat(bot.config.channel, "Unable to ping " + web);
                }
            }
            else if (bot.ContainsP(datal, "parzibot") && bot.ContainsP(datal, "power level"))
            {
                string sender = data.Split('!')[0].Replace(":", "");
                bot.chat(bot.config.channel, sender + ": It's over 9000!");
            }
        }
    }
}
