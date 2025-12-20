using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using PaymentService.Api.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Command.CreateOrders
{
    // CreateOrderCommand’i karşılayan ve işleyen Command Handler sınıfı
    // IRequestHandler<CreateOrderCommand, bool> → bu handler bool sonuç döner
    public class CreateOrderCommandHandler
        : IRequestHandler<CreateOrderCommand, bool>
    {
        // Sipariş işlemleri için kullanılan repository
        private readonly IOrderRepository orderRepository;

        // Mikroservisler arası event göndermek için kullanılan event bus
        private readonly IEventBus eventBus;

        // Logger eklenebilir ama şu an kullanılmıyor
        // private readonly ILogger<CreateOrderCommandHandler> logger;

        // Constructor
        // Gerekli bağımlılıklar Dependency Injection ile alınır
        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IEventBus eventBus
        )
        {
            this.orderRepository = orderRepository;
            this.eventBus = eventBus;
            // this.logger = logger;
        }

        // Command geldiğinde çalışan ana metot
        public async Task<bool> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken
        )
        {
            // Handler’ın çalıştığını loglamak için kullanılabilir
            // logger.LogInformation("CreateOrderCommandHandler -> Handle method invoked");

            // Kullanıcının adres bilgileri ile Address ValueObject oluşturulur
            var addr = new Address(
                request.Street,
                request.City,
                request.State,
                request.Country,
                request.ZipCode
            );

            // Order aggregate root nesnesi oluşturulur
            Order dbOrder = new(
                request.UserName,
                addr,
                request.CardTypeId,
                request.CardNumber,
                request.CardSecurityNumber,
                request.CardHolderName,
                request.CardExpiration,
                null
            );

            // Command içinden gelen sipariş kalemleri tek tek Order’a eklenir
            foreach (var orderItem in request.OrderItems)
            {
                dbOrder.AddOrderItem(
                    orderItem.ProductId,
                    orderItem.ProductName,
                    orderItem.UnitPrice,
                    orderItem.PictureUrl,
                    orderItem.Units
                );
            }

            // Oluşturulan sipariş veritabanına eklenir
            await orderRepository.AddAsync(dbOrder);

            // UnitOfWork ile değişiklikler kaydedilir
            // Aynı zamanda domain event’ler tetiklenir
            await orderRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            // Siparişin kaydedildiği bilgisi loglanabilir
            // logger.LogInformation("CreateOrderCommandHandler -> dbOrder saved");

            // Sipariş başladı event’i oluşturulur
            var orderStartedIntegrationEvent =
                new OrderStartedIntegrationEvent(
                    request.UserName,
                    dbOrder.Id
                );

            // Event diğer servislerin haberdar olması için publish edilir
            eventBus.publish(orderStartedIntegrationEvent);

            // Event’in gönderildiği bilgisi loglanabilir
            // logger.LogInformation("CreateOrderCommandHandler -> OrderStartedIntegrationEvent fired");

            // İşlem başarılı olduğu için true döner
            return true;
        }
    }

}
