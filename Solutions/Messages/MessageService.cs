using Microsoft.Extensions.Options;

namespace Solutions.Messages;

public interface IMessageService
{
    string GetMessage(string key);
}
public class MessageService : IMessageService
{
    public readonly IOptions<MessageConfiguration>? _messageConfigurations;
    public MessageService(IOptions<MessageConfiguration>? messageConfigurations)
    {
        _messageConfigurations = messageConfigurations;
    }
    public string GetMessage(string key)
    {
        var message = _messageConfigurations!.Value.Messages!.FirstOrDefault(x => x.Key!.Equals(key))?.Message;
        var defaultMessage = _messageConfigurations!.Value.Messages!.FirstOrDefault(x => x.Key!.Equals(MessageKeys.Something_Went_Wrong))?.Message! ?? key;
        return message is null ? defaultMessage : message;
    }
}