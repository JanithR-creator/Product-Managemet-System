using CheckoutService.Data;
using CheckoutService.Messaging;
using CheckoutService.Models.Dto.ReqDtos;
using CheckoutService.Models.Dto.ResDtos;
using CheckoutService.Models.Entities;
using Common.Events;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Services.ServiceImpl
{
    public class CheckoutServiceImpl : ICheckoutService
    {
        private readonly AppDbContext dbContext;
        private readonly CheckoutEventPublisher eventPublisher;

        public CheckoutServiceImpl(AppDbContext dbContext, CheckoutEventPublisher eventPublisher)
        {
            this.dbContext = dbContext;
            this.eventPublisher = eventPublisher;
        }

        public async Task<CheckoutSuccessResDto> CreateCheckoutAsync(CheckoutReqDto dto)
        {
            var checkout = new Checkout
            {
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = dto.Items.Select(i => new CheckoutItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            dbContext.Checkouts.Add(checkout);
            await dbContext.SaveChangesAsync();

            return (new CheckoutSuccessResDto()
            {
                CheckoutId = checkout.CheckoutId,
                Status = checkout.Status
            });
        }

        public async Task<bool> MakePaymentAsync(PaymentReqDto dto)
        {
            var checkout = await dbContext.Checkouts
               .Include(c => c.Items)
               .Include(c => c.Payment)
               .FirstOrDefaultAsync(c => c.CheckoutId == dto.CheckoutId);

            if (checkout == null)
                return false;

            if (checkout.Status == "Completed")
                return false;

            var totalAmount = checkout.Items.Sum(i => i.Quantity * i.UnitPrice);

            var payment = new PaymentRecord
            {
                CheckoutId = checkout.CheckoutId,
                Amount = totalAmount,
                Status = "Success",
                Message = "Payment successfully processed",
                PaymentMethod = dto.PaymentMethod,
                PaidAt = DateTime.UtcNow
            };

            checkout.Status = "Completed";
            checkout.Payment = payment;

            dbContext.PaymentRecords.Add(payment);
            await dbContext.SaveChangesAsync();

            eventPublisher.PublishCheckoutEvent(new CheckoutEventDto
            {
                UserId = checkout.UserId,
                TotalAmount = totalAmount
            });

            return true;
        }


        public async Task<CheckoutResDto> GetCheckOutByUserIdAsync(Guid userId)
        {
            var checkout = await dbContext.Checkouts
                .Include(c => c.Items)
                .Include(c => c.Payment)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (checkout == null)
            {
                throw new EntryPointNotFoundException("Checkout Empty.");
            }

            if (checkout.Payment == null)
            {
                throw new InvalidOperationException("Payment record is missing for the checkout.");
            }

            return new CheckoutResDto()
            {
                CheckoutId = checkout.CheckoutId,
                UserId = userId,
                PaymentRecordId = checkout.Payment.PaymentRecordId,
                CheckoutDate = checkout.CreatedAt,
                PaidDate = checkout.Payment.PaidAt,
                CheckoutStatus = checkout.Status,
                Amount = checkout.Payment.Amount,
                PaymentStatus = checkout.Payment.Status
            };
        }

        public Task<List<CheckoutResDto>> GetCheckoutsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
