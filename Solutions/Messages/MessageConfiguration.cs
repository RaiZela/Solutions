using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solutions.Messages;

public class MessageConfiguration
{
    public List<MessageTypeInfo>? MessageTypes { get; set; }
    public List<MessageModel>? Messages { get; set; }
}
