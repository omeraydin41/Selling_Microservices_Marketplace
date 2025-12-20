using AutoMapper;
using MediatR;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Queries.GetOrderDetailById
{
    // Sipariş detaylarını getirmek için kullanılan Query Handler sınıfı
    // IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
    // → Query çalışır ve OrderDetailViewModel döner
    public class GetOrderDetailsQueryHandler
        : IRequestHandler<GetOrderDetailsQuery, OrderDetailViewModel>
    {
        // Sipariş verilerine erişmek için kullanılan repository
        private readonly IOrderRepository orderRepository;

        // Entity → ViewModel dönüşümü için kullanılan AutoMapper
        private readonly IMapper mapper;

        // Constructor
        // Gerekli bağımlılıklar Dependency Injection ile alınır
        public GetOrderDetailsQueryHandler(
            IOrderRepository orderRepository,
            IMapper mapper
        )
        {
            // orderRepository null gelirse hata fırlatılır
            this.orderRepository = orderRepository
                ?? throw new ArgumentNullException(nameof(orderRepository));

            // mapper atanır
            this.mapper = mapper;
        }

        // Query çalıştırıldığında çağrılan metot
        public async Task<OrderDetailViewModel> Handle(
            GetOrderDetailsQuery request,
            CancellationToken cancellationToken
        )
        {
            // Verilen OrderId’ye göre sipariş bilgileri veritabanından alınır
            // OrderItems include edilerek birlikte getirilir
            var order = await orderRepository.GetByIdAsync(
                request.OrderId,
                i => i.OrderItems
            );

            // Order entity’si OrderDetailViewModel’e dönüştürülür
            var result = mapper.Map<OrderDetailViewModel>(order);

            // Oluşturulan ViewModel geri döndürülür
            return result;
        }
    }

}
