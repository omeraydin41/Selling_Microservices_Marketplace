using Microsoft.EntityFrameworkCore.Metadata;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable //amacı, bir nesne tarafından kullanılan yönetilemeyen kaynakları (unmanaged resources) serbest bırakmaktır.
    {
        private readonly IConnectionFactory connectionFactory;//bağlantı fabrikası : rabbitmq bağlantısı oluşturmak için kullanılır
        private readonly int retryCount;//bağlantı deneme sayısı
        private IConnection connection;//hangı bağlantı açık veya değil kontrol eder 
        private object lock_object = new object();//çoklu thread ler için kilit mekanizması sağlar
        private bool _disposed;//nesnenin dispose edilip edilmediğini takip eder
        public RabbitMQPersistentConnection(ConnectionFactory connectionFactory,int retryCount=5)
        {
            this.connectionFactory= connectionFactory;
        }

        public bool IConnection => connection != null && connection.IsOpen;//bağlantının kapalı olmamaıs lazım ve açık olması lazım

        public bool IsConnected { get; private set; }

        //public IModel CreateModel()
        //{

        //    return connection.CreateModel();

        //}

        public void Dispose()
        {
            connection.Dispose();
            _disposed = true;   
        }
        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(
                        retryCount,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            // log
                        }
                    );

                policy.Execute(() =>
                {
                    //connection = connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                   // connection.ConnectionShutdownAsync += connection_ConnectionShutdown;
                    connection.CallbackExceptionAsync += connection_CallbackException;
                    connection.ConnectionBlockedAsync += connection_ConnectionBlocked;
                    return true;
                }

                return false;
            }
        }

        private async Task connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs @event)
        {
            if (!_disposed) return;
                TryConnect();
        }

        private async Task connection_CallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            TryConnect();
        }

        private void connection_ConnectionShutdown()
        {
            TryConnect();
        }

      
    }

   
}
