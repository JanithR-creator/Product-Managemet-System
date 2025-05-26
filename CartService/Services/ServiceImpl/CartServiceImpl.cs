using CartService.Data;
using CartService.Messaging;
using CartService.Models.Dtos.RequestDtos;
using CartService.Models.Enitity;
using Common.Events;
using Microsoft.EntityFrameworkCore;

namespace CartService.Services.ServiceImpl
{
    public class CartServiceImpl : ICartService
    {
        private readonly AppDbContext dbContext;
        private readonly EventPublisher eventPublisher;

        public CartServiceImpl(AppDbContext dbContext, EventPublisher eventPublisher)
        {
            this.dbContext = dbContext;
            this.eventPublisher = eventPublisher;
        }

        public async Task AddItemToCart(CartItemReqDto dto, string provider)
        {
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

            if (cart == null)
            {
                cart = new Cart { 
                    UserId = dto.UserId,
                    Items = new List<CartItem>()
                };
                dbContext.Carts.Add(cart);
            }

            cart.Items.Add(new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });

            await dbContext.SaveChangesAsync();

            var @event = new ProductReserveEvent
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UserId = dto.UserId,
                Provider = provider
            };

            eventPublisher.PublishProductReserveEvent(@event);
        }
    }

}
