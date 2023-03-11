namespace Infra.Services
{
    public interface IMessageProducer
    {
        Task<bool> SendMessageAsync(object data, CancellationToken cancellationToken);
    }
}
