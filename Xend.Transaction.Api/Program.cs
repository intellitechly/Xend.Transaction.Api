
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Xend.Transaction.Api.Conf;
using Xend.Transaction.Api.Contracts;
using Xend.Transaction.Api.MiddleWare;
using Xend.Transaction.Api.Services;

namespace Xend.Transaction.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        
            builder.Services.AddDbContext<TransactionDbContext>(options => options.UseSqlServer("Data Source=SQL8002.site4now.net;Initial Catalog=db_a88f2f_tradeplafd;User Id=db_a88f2f_tradeplafd_admin;Password=secret#321"));

            builder.Services.AddMemoryCache(); // Add this line to register IMemoryCache
            builder.Services.AddScoped<TransactionsService>();
            builder.Services.AddScoped<ITransactionCacheService, TransactionCacheService>();
            builder.Services.AddScoped<IEventBus, EventBus>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = $"amqp://guest:guest@localhost"; // Update with your RabbitMQ connection string
                return new EventBus(connectionString);
            });
            builder.Services.AddScoped<IMessageBroker, MessageBroker>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = $"amqp://guest:guest@localhost"; // Update with your RabbitMQ connection string
                return new MessageBroker(connectionString);
            });
            builder.Services.AddScoped<ITransactionService,TransactionsService>();
            builder.Services.AddHttpClient<ICryptoApiClient, CryptoApiClient>();

            var app = builder.Build();

            // Inside Configure method
           
      

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
           // app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }

}