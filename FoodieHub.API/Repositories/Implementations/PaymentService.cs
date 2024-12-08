using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FoodieHub.API.Repositories.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        public PaymentService(AppDbContext context, IMailService mailService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mailService = mailService;
            _userManager = userManager;
        }

        public async Task<bool> Create(PaymentDTO payment)
        {
            var order = await _context.Orders.FindAsync(payment.OrderID);
            if (order == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newPayment = new FoodieHub.API.Data.Entities.Payment
                {
                    OrderID = order.OrderID,
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.TotalAmount
                };
                await _context.Payments.AddAsync(newPayment);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    order.Status = "PAYED";
                    order.PaymentStatus = true;
                    _context.Orders.Update(order);

                    await _context.OrderStatusHistories.AddAsync(new FoodieHub.API.Data.Entities.OrderStatusHistory
                    {
                        OrderID = order.OrderID,
                        Status = "PAYED"
                    });

                    var result2 = await _context.SaveChangesAsync();

                    if (result > 0 && result2 > 1)
                    {
                        var user = await _userManager.FindByIdAsync(order.UserID);
                        // gửi mail
                        var newMail = new MailRequest
                        {
                            ToEmail = user.Email,
                            Subject = "Invoice Information",
                            Body = GenerateInvoiceMail(order.User.Fullname,newPayment.PaymentMethod, order.PhoneNumber, "", newPayment.PaymentDate.ToShortDateString(), newPayment.Amount.ToString())
                        };
                        await _mailService.SendEmailAsync(newMail);
                        await transaction.CommitAsync();
                        return true;
                    }                
                }
                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }


        public string GenerateInvoiceMail(string customerName,string PaymentMethod, string orderNumber, string invoiceLink, string issueDate, string totalAmount)
        {
            return $@"
        <div style=""font-family: Arial, sans-serif; line-height: 1.6;"">
            <h2>Invoice Notification for Your Order on FoodieHub</h2>
            <p>Dear {customerName},</p>

            <p>Thank you for shopping with FoodieHub.</p>
            
            <p>We are pleased to inform you that the electronic invoice for your order <strong>{orderNumber}</strong> has been successfully generated. You can view and download the invoice directly from our website by logging into your account and visiting the ""Order History"" section, or by clicking the link below:</p>
            
            <p><a href=""{invoiceLink}"" style=""color: #007BFF;"">Download Invoice</a></p>

            <p>Invoice details:</p>
            <ul>
                <li><strong>Payment Method:</strong> {PaymentMethod}</li>
                <li><strong>Order Number:</strong> {orderNumber}</li>
                <li><strong>Invoice Issue Date:</strong> {issueDate}</li>
                <li><strong>Total Amount:</strong> {totalAmount} $</li>
            </ul>

            <p>Please keep this invoice for your records and warranty purposes (if applicable).</p>

            <p>If you have any questions regarding the invoice or your order, feel free to contact us:</p>
            <ul>
                <li><strong>Email:</strong> <a href=""mailto:{"nguyenphuc14112003@gmail.com"}"">{"nguyenphuc14112003@gmail.com"}</a></li>
                <li><strong>Phone:</strong>0898827656</li>
            </ul>

            <p>Thank you for choosing FoodieHub. We look forward to serving you again!</p>

            <p>Best regards,</p>
            <p>FoodieHub</p>
            <p><strong>Address:</strong>QTSC</p>
            <p><strong>Phone:</strong>0898827656</p>
        </div>
    ";
        }
    }
}
