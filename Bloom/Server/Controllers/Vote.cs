using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using System.Text;
using Bloom.Shared;
using System.Xml.Linq;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.Xml.Serialization;
using System.IO;

namespace Bloom.Server.Controllers
{
    public class Vote :Hub
    {
        static DateTime limit = new DateTime(2022, 9, 23, 9, 0, 0);
        public async Task ThrowVote(string str)
        {
#if DEBUG
            await RegistVoteDev(str);
#endif
#if !DEBUG
                await RegistVote(str);
#endif
        }
        private async Task RegistVote(string mess)
        {
            if(DateTime.Now < limit)
            {
                await Clients.Caller.SendAsync("BeforResist");
            }
            /*var filePath = DirectoryManeger.GetAbsotoblePath("/vote/ip.txt");
            */
            string str = mess.Split(",")[0];
            string listPath = string.Empty;
            if (mess.Split(",")[1] == "cm")
            {
                listPath = DirectoryManeger.GetAbsotoblePath("/vote/cmlist.csv");
            }else if (mess.Split(",")[1] == "poster")
            {
                listPath = DirectoryManeger.GetAbsotoblePath("/vote/posterlist.csv");
            }

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader(listPath, Encoding.UTF8))
            {
                try
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        valuePairs.Add(line.Split(",")[0], line.Split(",")[1]);
                    }
                }
                catch (Exception ex)
                {
                    await Clients.Caller.SendAsync("ErrorInVote",ex.Message);
                }
            }
            valuePairs[str] = (Int32.Parse(valuePairs[str]) + 1).ToString();
            string content = string.Empty;
            foreach (var pair in valuePairs)
            {
                content += (pair.Key + "," + pair.Value + "\n");
            }
            using (StreamWriter sr = new StreamWriter(listPath, false,Encoding.UTF8))
            {
                try
                {
                    await sr.WriteAsync(content);
                }
                catch (Exception ex)
                {
                    await Clients.Caller.SendAsync("ErrorInVote", ex.Message);
                }
                await Clients.Caller.SendAsync("AcceptVote");
            }
        }

        private async Task RegistVoteDev(string str)
        {
            Console.WriteLine("Vote to " + str + "from" + string.Empty);
            await Clients.Caller.SendAsync("AcceptVote");
        }
    }
}
