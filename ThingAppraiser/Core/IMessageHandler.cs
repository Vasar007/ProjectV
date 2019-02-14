namespace ThingAppraiser.Core
{
    public interface IMessageHandler
    {
        string GetMessage();

        void OutputMessage(string message);
    }
}
