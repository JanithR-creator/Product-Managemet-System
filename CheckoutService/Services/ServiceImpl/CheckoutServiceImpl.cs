using CheckoutService.AdapterEndpointHandler;
using CheckoutService.Data;
using CheckoutService.Messaging;
using CheckoutService.Models.Dto.ExternalDtos;
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
        private readonly IAdapterEndpointHandler adapterEndpointHandler;

        public CheckoutServiceImpl(AppDbContext dbContext, CheckoutEventPublisher eventPublisher, IAdapterEndpointHandler adapterEndpointHandler)
        {
            this.dbContext = dbContext;
            this.eventPublisher = eventPublisher;
            this.adapterEndpointHandler = adapterEndpointHandler;
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
                    UnitPrice = i.UnitPrice,
                    Provider = i.Provider
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

            if (checkout == null || checkout.Status == "Completed")
                return false;

            var groupedByProvider = checkout.Items
                .GroupBy(item => item.Provider)
                .ToList();

            foreach (var group in groupedByProvider)
            {
                var currentProvider = group.Key;
                var totalAmount = group.Sum(item => item.UnitPrice * item.Quantity);

                if (currentProvider == "Internal")
                {
                    continue;
                }

                var extPaymentDto = new ExtPaymentReqDto
                {
                    PaymentMethod = dto.PaymentMethod,
                    TotalAmount = totalAmount,
                    UserId = checkout.UserId
                };

                var resp = await adapterEndpointHandler.MakePaymentAaync(extPaymentDto, currentProvider);

                if (!resp)
                {
                    throw new InvalidOperationException($"Payment failed through adapter service for provider: {currentProvider}");
                }
            }

            var totalOverall = checkout.Items.Sum(i => i.Quantity * i.UnitPrice);

            var payment = new PaymentRecord
            {
                CheckoutId = checkout.CheckoutId,
                Amount = totalOverall,
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
                TotalAmount = totalOverall
            });

            return true;
        }


        public async Task<CheckoutResDto> GetCheckOutByCheckoutIdAsync(Guid checkOutId)
        {
            var checkout = await dbContext.Checkouts
                .Include(c => c.Items)
                .Include(c => c.Payment)
                .FirstOrDefaultAsync(c => c.CheckoutId == checkOutId);

            if (checkout == null)
            {
                throw new KeyNotFoundException($"Checkout with ID {checkOutId} not found.");
            }

            return new CheckoutResDto
            {
                CheckoutId = checkout.CheckoutId,
                PaymentRecordId = checkout.Payment?.PaymentRecordId ?? Guid.Empty,
                UserId = checkout.UserId,
                CheckoutDate = checkout.CreatedAt,
                PaidDate = checkout.Payment?.PaidAt ?? DateTime.MinValue,
                CheckoutStatus = checkout.Status,
                Amount = checkout.Payment?.Amount ?? 0,
                PaymentStatus = checkout.Payment?.Status ?? "Pending",
                Items = checkout.Items.Select(i => new CheckoutItemResDto
                {
                    CheckoutItemId = i.CheckoutItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Provider = i.Provider,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<List<PaymentDetailsResDto>> GetAllPaymentDetalsAsync(DateTime? dateTime = null)
            {
                try
                {
                    IQueryable<PaymentRecord> query = dbContext.PaymentRecords.Include(p => p.Checkout);

                    if (dateTime.HasValue)
                    {
                        var dateOnly = dateTime.Value.Date;
                        query = query.Where(p => p.PaidAt.Date == dateOnly);
                    }

                    var paymentRecords = await query.ToListAsync();

                    return paymentRecords.Select(p => new PaymentDetailsResDto
                    {
                        PaymentRecordId = p.PaymentRecordId,
                        CheckoutId = p.Checkout.CheckoutId,
                        Amount = p.Amount,
                        Status = p.Status,
                        Message = p.Message,
                        PaymentMethod = p.PaymentMethod,
                        PaidAt = p.PaidAt
                    }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while fetching payment details.", ex);
                }
            }
        public async Task<List<DateTime>> GetAllPaymentDatesAsync()
        {
            var dates = await dbContext.PaymentRecords
                .Select(p => p.PaidAt.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return dates;
        }

    }
}
