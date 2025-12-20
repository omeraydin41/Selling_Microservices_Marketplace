using AutoMapper;
using OrderService.Application.Features.Command.CreateOrders;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Mapping.OrderMapping
{
    public class OrderMappingProfile : Profile
    {
        // Order ile ilgili AutoMapper dönüşümlerinin tanımlandığı mapping profili
        public OrderMappingProfile()
        {
            // Order entity ↔ CreateOrderCommand dönüşümü
            // ReverseMap sayesinde iki yönlü dönüşüm yapılabilir
            CreateMap<Order, CreateOrderCommand>()
                .ReverseMap();

            // OrderItem entity ↔ OrderItemDTO dönüşümü
            // Sipariş kalemleri için kullanılır
            CreateMap<OrderItem, OrderItemDTO>()
                .ReverseMap();

            // Order entity ↔ OrderDetailViewModel dönüşümü
            CreateMap<Order, OrderDetailViewModel>()

                // Address içindeki City bilgisini ViewModel’deki City alanına map eder
                .ForMember(
                    x => x.City,
                    y => y.MapFrom(z => z.Address.City)
                )

                // Address içindeki Country bilgisini map eder
                .ForMember(
                    x => x.Country,
                    y => y.MapFrom(z => z.Address.Country)
                )

                // Address içindeki Street bilgisini map eder
                .ForMember(
                    x => x.Street,
                    y => y.MapFrom(z => z.Address.Street)
                )

                // Address içindeki ZipCode bilgisini map eder
                .ForMember(
                    x => x.Zipcode,
                    y => y.MapFrom(z => z.Address.ZipCode)
                )

                // Sipariş tarihini OrderDate alanından alır
                .ForMember(
                    x => x.Date,
                    y => y.MapFrom(z => z.OrderDate)
                )

                // Order Id’yi string’e çevirip Ordernumber alanına atar
                .ForMember(
                    x => x.Ordernumber,
                    y => y.MapFrom(z => z.Id.ToString())
                )

                // Sipariş durumunun Name bilgisini Status alanına map eder
                .ForMember(
                    x => x.Status,
                    y => y.MapFrom(z => z.OrderStatus.Name)
                )

                // Siparişin toplam tutarını hesaplar
                // Units * UnitPrice tüm ürünler için toplanır
                .ForMember(
                    x => x.Total,
                    y => y.MapFrom(
                        z => z.OrderItems.Sum(i => i.Units * i.UnitPrice)
                    )
                )

                // ViewModel ↔ Order için çift yönlü dönüşüm sağlar
                .ReverseMap();

            // OrderItem entity ↔ Orderitem (muhtemelen ViewModel veya farklı DTO)
            CreateMap<OrderItem, Orderitem>();
        }

    }
}
