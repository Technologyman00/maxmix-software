namespace MaxMix.Services.Communication
{
    internal interface IMessage
    {
        int MessageId { get; }
        byte[] GetBytes();
        bool SetBytes(byte[] bytes);
    }
}