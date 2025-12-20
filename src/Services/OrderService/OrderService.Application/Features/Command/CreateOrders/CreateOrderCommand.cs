using MediatR;
using OrderService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Command.CreateOrders
{
    // Sipariş oluşturmak için kullanılan Command sınıfı
    // MediatR kullanıldığı için IRequest<bool> implement edilir
    // bool: işlem başarılı mı değil mi bilgisini döner
    public class CreateOrderCommand : IRequest<bool>
    {
        // Siparişteki ürünleri tutan liste
        // private yapıldı çünkü dışarıdan direkt değiştirilmesi istenmiyor
        private readonly List<OrderItemDTO> _orderItems;

        // Siparişi veren kullanıcının kullanıcı adı
        public string UserName { get; private set; }

        // Teslimat adres bilgileri
        public string City { get; private set; }
        public string Street { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        // Ödeme kartı bilgileri
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime CardExpiration { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public int CardTypeId { get; private set; }

        // Sipariş ürünlerini dışarıya sadece okunabilir şekilde açıyoruz
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        // Boş constructor
        // OrderItem listesi burada başlatılır
        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDTO>();
        }

        // Asıl kullanılan constructor
        // Sepetteki ürünlerden sipariş oluşturmak için kullanılır
        public CreateOrderCommand(
            List<BasketItem> basketItems,
            string userId,
            string userName,
            string city,
            string street,
            string state,
            string country,
            string zipcode,
            string cardNumber,
            string cardHolderName,
            DateTime cardExpiration,
            string cardSecurityNumber,
            int cardTypeId
        ) : this()
        {
            // Sepetteki ürünleri OrderItemDTO listesine çeviriyoruz
            var dtoList = basketItems.Select(item => new OrderItemDTO()
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl,
                UnitPrice = item.UnitPrice,
                Units = item.Quantity
            });

            // DTO listesini private listeye aktarıyoruz
            _orderItems = dtoList.ToList();

            // Kullanıcı ve adres bilgileri atanır
            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;

            // Kart bilgileri atanır
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
        }
    }
    // Sipariş içindeki ürün bilgilerini taşımak için kullanılan DTO sınıfı
    // DTO: Data Transfer Object
    public class OrderItemDTO
    {
        // Ürün Id bilgisi
        public int ProductId { get; init; }

        // Ürün adı
        public string ProductName { get; init; }

        // Ürünün birim fiyatı
        public decimal UnitPrice { get; init; }

        // Kaç adet alındığı bilgisi
        public int Units { get; init; }

        // Ürün görselinin URL bilgisi
        public string PictureUrl { get; init; }
    }


}
