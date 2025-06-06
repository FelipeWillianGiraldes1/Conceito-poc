
using OrderAPI.Models;
using System.Threading.Tasks;

namespace OrderAPI.Services
{
    public interface IAzureServiceBusPublisher
    {
        Task SendMessageAsync(Order order);
    }
}
