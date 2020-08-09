using System;
using System.Collections.Generic;
using System.IO;

namespace ThorNet.Sample
{
    public class Messages : Thor
    {
        const string MessagesFile = "./Messages.txt";

        [Alias("add")]
        [Desc("messages add MESSAGE", "adds a message")]
        public void Add(string message)
        {
            List<string> lines;

            if (File.Exists(MessagesFile))
                lines = new List<string>(File.ReadAllLines(MessagesFile));

            else
                lines = new List<string>();

            lines.Add(message);

            File.WriteAllLines(MessagesFile, lines);
        }

        [Alias("list")]
        [Desc("messages list", "lists all messages")]
        public void List()
        {
            if (File.Exists(MessagesFile))
            {
                string[] lines = File.ReadAllLines(MessagesFile);

                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
        }

        [Alias("remove")]
        [Desc("messages remove ID", "removes a message")]
        public void Remove(int id)
        {
            if (File.Exists(MessagesFile))
            {
                List<string> lines = new List<string>(File.ReadAllLines(MessagesFile));

                lines.RemoveAt(id);

                File.WriteAllLines(MessagesFile, lines);
            }
        }
    }
}
