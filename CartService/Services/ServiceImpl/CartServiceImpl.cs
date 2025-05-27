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

            var @event = new ProductCommonEventDto
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UserId = dto.UserId,
                Provider = provider
            };

            eventPublisher.PublishProductReserveEvent(@event);
        }

        public async Task RemoveItemFromCart(Guid cartItemId, string provider)
        {
            var item = await dbContext.CartItems
                .Include(i => i.Cart) // include Cart to access UserId
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId);

            if (item == null)
            {
                throw new EntryPointNotFoundException("Item not found.");
            }

            var restoreEvent = new ProductCommonEventDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UserId = item.Cart.UserId,
                Provider = provider
            };

            dbContext.CartItems.Remove(item);
            await dbContext.SaveChangesAsync();

            eventPublisher.PublishProductRestoreEvent(restoreEvent);
        }

        public async Task UpdateCartItem(CartItemUpdateReqDto dto, string provider)
        {
            var item = await dbContext.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == dto.CartItemId);

            if (item == null) { throw new EntryPointNotFoundException("Item not found."); }

            if (dto.Quantity <= 0)
            {
                RemoveItemFromCart(item.CartItemId, provider).Wait();
            }
            else
            {
                int changeQuantity = item.Quantity - dto.Quantity;

                item.Quantity = dto.Quantity;
                dbContext.CartItems.Update(item);

                var restoreEvent = new ProductCommonEventDto
                {
                    ProductId = item.ProductId,
                    Quantity = changeQuantity,
                    UserId = item.Cart.UserId,
                    Provider = provider
                };
                await dbContext.SaveChangesAsync();

                eventPublisher.PublishProductRestoreEvent(restoreEvent);
            }
        }
    }

}
