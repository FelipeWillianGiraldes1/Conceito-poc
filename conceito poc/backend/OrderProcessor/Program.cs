
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderAPI.Data;
using OrderAPI.Models;
using System.Text.Json;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var connectionString = configuration["AzureServiceBus:ConnectionString"];
var queueName = configuration["AzureServiceBus:QueueName"];

var client = new ServiceBusClient(connectionString);
var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

processor.ProcessMessageAsync += async args =>
{
    var json = args.Message.Body.ToString();
    var order = JsonSerializer.Deserialize<Order>(json);

    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    var orderDb = await db.Orders.FindAsync(order.Id);
    if (orderDb != null)
    {
        orderDb.Status = "Processando";
        await db.SaveChangesAsync();

        await Task.Delay(5000); // simula processamento

        orderDb.Status = "Finalizado";
        await db.SaveChangesAsync();
    }

    await args.CompleteMessageAsync(args.Message);
};

processor.ProcessErrorAsync += args =>
{
    Console.WriteLine($"Erro no processamento: {args.Exception.Message}");
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();
await host.RunAsync();
