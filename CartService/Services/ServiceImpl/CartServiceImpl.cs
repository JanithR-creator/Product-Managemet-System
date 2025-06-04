using CartService.AdapterEndPointController;
using CartService.Data;
using CartService.Messaging;
using CartService.Models.Dtos.ExternalCartDtos;
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
        private readonly IAdapterEndPointHandler adapterEndPointHandler;

        public CartServiceImpl(AppDbContext dbContext, CartEventPublisher eventPublisher, IAdapterEndPointHandler adapterEndPointHandler)
        {
            this.dbContext = dbContext;
            this.eventPublisher = eventPublisher;
            this.adapterEndPointHandler = adapterEndPointHandler;
        }

        public async Task<bool> AddItemToCart(CartItemReqDto dto, string provider)
        {
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

            if (cart != null && cart.Items.Any(i => i.ProductId == dto.ProductId))
            {
                var existingItem = cart?.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;

                    await UpdateCartItem(new CartItemUpdateReqDto
                        {
                            CartItemId = existingItem.CartItemId,
                            Quantity = existingItem.Quantity
                        }, provider);

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }

            var @event = new ProductCommonEventDto
            {
                ProductId = dto.ProductId,
                Quantity = 1
            };

            bool isReserved = await eventPublisher.PublishProductReserveEventAsync(@event);

            if (!isReserved)
            {
                Console.WriteLine($"[X] Insufficient stock for product {dto.ProductId}");
                return false;
            }

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
                ExternalProductId = dto.ExternalProductId,
                Quantity = 1,
                UnitPrice = dto.UnitPrice,
                AddedDate = DateTime.UtcNow,
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                Provider = provider
            });
            await dbContext.SaveChangesAsync();

            if (dto.ExternalProductId.HasValue)
            {
                var cartReqDto = new CartReqDto()
                {
                    ProductId = dto.ExternalProductId.Value,
                    Quantity = 1,
                    UserId = dto.UserId
                };

                bool resp = await adapterEndPointHandler.AddToCartAsync(provider, cartReqDto);

                return resp;
            }

            return true;
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
                Quantity = item.Quantity
            };

            if (item.ExternalProductId.HasValue)
            {
                var cartReqDto = new CartItemRemoveReqDto()
                {
                    ProductId = item.ExternalProductId.Value,
                    UserId = item.Cart.UserId
                };
                bool resp = await adapterEndPointHandler.RemoveFromCartAsync(provider, cartReqDto);

                if (!resp)
                {
                    throw new Exception("Failed to remove item from external cart.");
                }
            }

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
                if (item.ExternalProductId.HasValue)
                {
                    var cartItemUpdateDto = new CartReqDto()
                    {
                        ProductId = item.ExternalProductId.Value,
                        UserId = item.Cart.UserId,
                        Quantity = dto.Quantity
                    };

                    var resp = await adapterEndPointHandler.UpdateItemAsync(provider, cartItemUpdateDto);

                    if (!resp)
                    {
                        throw new Exception("Failed to update item in external cart.");
                    }
                }

                int changeQuantity = item.Quantity - dto.Quantity;

                item.Quantity = dto.Quantity;

                dbContext.CartItems.Update(item);
                await dbContext.SaveChangesAsync();

                var updateEvent = new ProductCommonEventUpdateDto
                {
                    ProductId = item.ProductId,
                    ChangeQuantity = changeQuantity,
                    Quantity = dto.Quantity
                };
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
                ProductDescription = i.ProductDescription,
                Provider = i.Provider
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
                    ProductDescription = i.ProductDescription,
                    Provider = i.Provider
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
