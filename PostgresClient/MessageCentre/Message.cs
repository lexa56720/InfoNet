using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.MessageCentre
{
    public class Message
    {
        public string MessageText { get; }

        public Message(string message)
        {
            MessageText = message;
        }      
    }
}
