using EventBus.Base.Events;
using EventBus.Base.SubManager;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using IModel = RabbitMQ.Client.IModel;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
   
        private readonly IConnectionFactory connectionFactory;

        private readonly IModel consumerChannel;
     

 

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
                {

                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                connectionFactory = JsonConvert.DeserializeObject<IConnectionFactory>(connJson);

                consumerChannel = CreateConsumerChannel();

            }
            else
                connectionFactory = new ConnectionFactory();

            persistentConnection = new RabbitMQPersistentConnection((ConnectionFactory)connectionFactory, config.ConnectionRetryCount);

            consumerChannel=CreateConsumerChannel();
            subsManager.OnEventRemoved += SubsManager_OnEventRemoved;

        }
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            consumerChannel.QueueUnbind(queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

            if (subsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)//event yayınlama
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    // log
                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct"); // Ensure exchange exists while publishing

           

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                //consumerChannel.QueueDeclare(queue: getSubName(eventName), // Ensure queue exists while publishing
                //                     durable: true,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                //consumerChannel.QueueBind(queue: getSubName(eventName),
                //                 exchange: EventBusConfig.DefaultTopicName,
                //                 routingKey: eventName);

                consumerChannel.QueueBind(queue: getSubName(eventName),
                                 exchange: EventBusConfig.DefaultTopicName,
                                 routingKey: eventName);

                consumerChannel.QueueBind(queue: getSubName(eventName),
                                  exchange: EventBusConfig.DefaultTopicName,
                                  routingKey: eventName);

                consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        public override void Subscribe<T, TH>()//abonelik işlemi
        {
           var eventName = typeof(T).Name;//event adını alır
           eventName = ProcessEventName(eventName);//event adını işler (prefix/suffix temizler) : bu methos BaseEventBus da tanımlı

            subsManager.HasSubscriptionsForEvent(eventName);//event için abonelik var mı kontrol eder ve döndürür .
                                                            //submanager Baseeventbusta tanımlı ve oda IEventBusSubscriptionManager interfacesinden kalıtım alır

            if (!subsManager.HasSubscriptionsForEvent(eventName))//eğer event için abonelik yoksa ! ile kontrol eder
            {//ama önce rabbıtmq de bağlantıların yönetileceği bir connection mangere ihtiyaç var 

                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(queue: getSubName(eventName), // Ensure queue exists while consuming
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null
                );

                consumerChannel.QueueBind(queue: getSubName(eventName),
                                  exchange: EventBusConfig.DefaultTopicName,
                                  routingKey: eventName
                );
            }

            subsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
  
        }


        public override void UnSubscribe<T, TH>()
        {
            subsManager.RemoveSubscription<T, TH>();
        }
        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");

            return channel;
        }
        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(
                    queue: getSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }
        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                // logging
            }

            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
    }
}

