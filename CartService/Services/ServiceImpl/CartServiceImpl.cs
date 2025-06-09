using CartService.Data;
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

        public CartServiceImpl(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddItemToCart(CartItemReqDto dto)
        {
            var cart = dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == dto.UserId);

            if (cart != null)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

                if (existingItem != null)
                {
                    var newQty = existingItem.Quantity +1;
                    await UpdateCartItem(new CartItemUpdateReqDto
                    {
                        CartItemId = existingItem.CartItemId,
                        Quantity = newQty
                    });
                    return true;
                }

                var newItem = new CartItem
                {
                    ProductId = dto.ProductId,
                    ExternalProductId = dto.ExternalProductId,
                    Quantity = 1,
                    UnitPrice = dto.UnitPrice,
                    AddedDate = DateTime.UtcNow,
                    ProductName = dto.ProductName,
                    ProductDescription = dto.ProductDescription,
                    Provider = dto.Provider,
                    Cart = cart,
                    CartId = cart.CartId
                };
                dbContext.CartItems.Add(newItem);
                dbContext.SaveChanges();
                return true;
            }

            var newCart = new Cart
            {
                CartId = Guid.NewGuid(),
                UserId = dto.UserId,
                Items = new List<CartItem>()
            };

            dbContext.Carts.Add(newCart);
            dbContext.SaveChanges();

            var newItemForCart = new CartItem
            {
                ProductId = dto.ProductId,
                ExternalProductId = dto.ExternalProductId,
                Quantity = 1,
                UnitPrice = dto.UnitPrice,
                AddedDate = DateTime.UtcNow,
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                Provider = dto.Provider,
                Cart = newCart,
                CartId = newCart.CartId
            };
            dbContext.CartItems.Add(newItemForCart);
            dbContext.SaveChanges();

            return true;
        }

        public async Task RemoveItemFromCart(Guid cartItemId)
        {
            var item = await dbContext.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId);

            if (item == null)
            {
                throw new EntryPointNotFoundException("Item not found.");
            }

            dbContext.CartItems.Remove(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartItem(CartItemUpdateReqDto dto)
        {
            var item = await dbContext.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == dto.CartItemId);

            if (item == null) { throw new EntryPointNotFoundException("Item not found."); }

            if (dto.Quantity <= 0)
            {
                RemoveItemFromCart(item.CartItemId).Wait();
            }
            else
            { 
                int changeQuantity = item.Quantity - dto.Quantity;

                item.Quantity = dto.Quantity;

                dbContext.CartItems.Update(item);
                await dbContext.SaveChangesAsync();
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
                ExternalProductId = i.ExternalProductId,
                ProductDescription = i.ProductDescription,
                Provider = i.Provider
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
