using CartService.Data;
using CartService.Messaging;
using CartService.Models.Dtos.RequestDtos;
using CartService.Models.Dtos.ResponseDtos;
using CartService.Models.Enitity;
using Common.Events;
using Microsoft.EntityFrameworkCore;

namespace CartService.Services.ServiceImpl
{
    public class CartServiceImpl : ICartService
    {
        private readonly AppDbContext dbContext;
        private readonly CartEventPublisher eventPublisher;

        public CartServiceImpl(AppDbContext dbContext, CartEventPublisher eventPublisher)
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
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                AddedDate = DateTime.UtcNow,
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription
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

                var updateEvent = new ProductCommonEventUpdateDto
                {
                    ProductId = item.ProductId,
                    ChangeQuantity = changeQuantity,
                    Quantity = dto.Quantity,
                    UserId = item.Cart.UserId,
                    Provider = provider
                };
                await dbContext.SaveChangesAsync();

                eventPublisher.PublishProductUpdateEvent(updateEvent);
            }
        }

        public async Task<List<CartItemGetResDto>> GetCartItems(Guid userId)
        {
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null || !cart.Items.Any())
            {
                return new List<CartItemGetResDto>();
            }
            return cart.Items.Select(i => new CartItemGetResDto
            {
                CartItemId = i.CartItemId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                AddedDate = i.AddedDate,
                ProductName = i.ProductName,
                ProductDescription = i.ProductDescription
            }).ToList();
        }

        public async Task<List<CartDetailsResDto>> GetAllCartItems()
        {
            var carts = await dbContext.Carts
                .Include(c => c.Items)
                .ToListAsync();
            return carts.Select(c => new CartDetailsResDto
            {
                CartId = c.CartId,
                UserId = c.UserId,
                Items = c.Items.Select(i => new CartItemGetResDto
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    AddedDate = i.AddedDate,
                    ProductName = i.ProductName,
                    ProductDescription = i.ProductDescription
                }).ToList()
            }).ToList();
        }

        public async Task<bool> Checkout(CheckoutEventDto @event)
        {
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == @event.UserId);

            if (cart == null)
            {
                return false;
            }

            var items = cart.Items.ToList();

            dbContext.CartItems.RemoveRange(items);
            dbContext.Carts.Remove(cart);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }

}
