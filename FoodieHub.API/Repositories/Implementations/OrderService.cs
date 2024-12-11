using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly IQRCodeService _qrCodeService;
        private readonly ImageExtentions uploadImageHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderService(AppDbContext context, IAuthService authService, IMapper mapper, IMailService mailService, IConfiguration config, IQRCodeService qrCodeService, ImageExtentions uploadImageHelper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
            _mailService = mailService;
            _config = config;
            _qrCodeService = qrCodeService;
            this.uploadImageHelper = uploadImageHelper;
            _userManager = userManager;
        }
        public async Task<ServiceResponse> Create(OrderDTO order)
        {
            if (!order.OrderDetails.Any()) return new ServiceResponse
            {
                Success = false,
                Message = "List order detail is required",
                StatusCode = 400
            };
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userID = _authService.GetUserID();

                var newOrder = _mapper.Map<Order>(order);
                newOrder.UserID = userID;
                newOrder.Status = "PENDING";
                await _context.Orders.AddAsync(newOrder);
                var resultSaveOrder = await _context.SaveChangesAsync();
                if (resultSaveOrder <= 0)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "An error occured while creating order",
                        StatusCode = 500
                    };
                }

                var products = await _context.Products.ToListAsync();

                foreach (var detailDto in order.OrderDetails)
                {
                    var product = products.FirstOrDefault(p => p.ProductID == detailDto.ProductID);
                    if (product == null)
                    {
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = $"Not found product with ID {detailDto.ProductID}.",
                            StatusCode = 404
                        };
                    }
                    // Check số lượng sản phẩm 
                    if(detailDto.Quantity> product.StockQuantity)
                    {
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = "Invalid product quantity",
                            StatusCode = 400
                        };
                    }
                    // cập nhật số lượng trong kho
                    product.StockQuantity -= detailDto.Quantity;
                    _context.Products.Update(product);

                    decimal priceAfterDiscount = (100 - product.Discount) * product.Price / 100 * detailDto.Quantity;
                    var orderDetail = new OrderDetail
                    {
                        ProductID = detailDto.ProductID,
                        UnitPrice = product.Price,
                        Quantity = detailDto.Quantity,
                        Discount = product.Discount,
                        TotalPrice = priceAfterDiscount,
                        OrderID = newOrder.OrderID
                    };

                    await _context.OrderDetails.AddAsync(orderDetail);
                    newOrder.TotalAmount += priceAfterDiscount;
                }

                if (order.CouponID.HasValue)
                {
                    var coupon = await _context.Coupons.FindAsync(order.CouponID);
                    if (coupon == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = "Invalid Coupon. Please try again",
                            StatusCode = 400
                        };
                    }
                    if (coupon.MinimumOrderAmount >= newOrder.TotalAmount)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = $"The order amount must be at least {coupon.MinimumOrderAmount} $ to apply this coupon.",
                            StatusCode = 400
                        };
                    }
                    decimal discountOfCoupon = 0;
                    if (coupon.DiscountType.ToUpperInvariant() == "PERCENTAGE")
                    {
                        discountOfCoupon = newOrder.TotalAmount * coupon.DiscountValue / 100;
                    }
                    if (coupon.DiscountType.ToUpperInvariant() == "FIXED")
                    {
                        discountOfCoupon = coupon.DiscountValue;
                    }
                    coupon.IsUsed = true;
                    newOrder.DiscountOfCoupon = discountOfCoupon;
                    _context.Coupons.Update(coupon);
                }
                if (order.PaymentMethod)
                {
                    newOrder.Discount = newOrder.TotalAmount * 10 / 100;
                }

                // Tạo QR Code
                string linkOrderForQRCode = _config["OriginFE"] + $"/orders/qrcode/{newOrder.OrderID}";

                byte[] qrcodeByte = _qrCodeService.GenerateQRCode(linkOrderForQRCode);

                var pathQRCode = await uploadImageHelper.SaveImageFromBytesAsync(qrcodeByte, (newOrder.OrderID+".png"));

                newOrder.QRCode = pathQRCode;

                _context.Orders.Update(newOrder);

                await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
                {
                    OrderID = newOrder.OrderID,
                    Status = newOrder.Status
                });

                var result = await _context.SaveChangesAsync();
                if (result > order.OrderDetails.Count()+2)
                {
                    await transaction.CommitAsync();
                    // Gui mail
                    var user = await _authService.GetCurrentUser();
                    if (user != null)
                    {
                        var mailRequest = new MailRequest
                        {
                            Subject = "Order Placed Successfully",
                            ToEmail = user.Email,
                            Body = GenerateOrderConfirmationEmail(newOrder),
                            Attachments = new List<Attachment>()
                            {
                                new Attachment
                                {
                                    FileName = "QRCode.png", // Thêm phần mở rộng .png cho tên tệp
                                    FileData = qrcodeByte,
                                    ContentType = "image/png"
                                }
                            }
                        };
                        await _mailService.SendEmailAsync(mailRequest);
                    }
                    var data = new ReponseOrder
                    {
                        OrderID = newOrder.OrderID,
                        TotalAmount = newOrder.TotalAmount
                    };
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "The order has been successfully created",
                        Data = data,
                        StatusCode = 201
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "An error occured while creating order",
                        StatusCode = 500
                    };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<PaginatedModel<GetOrder>> Get(QueryOrderModel queryOrder)
        {
            var listOrders = _context.Orders
                .ProjectTo<GetOrder>(_mapper.ConfigurationProvider)
                .AsQueryable();
            if (queryOrder.OrderDate.HasValue)
            {
                var dateToCompare = queryOrder.OrderDate.Value.ToDateTime(TimeOnly.MinValue);
                listOrders = listOrders.Where(x => x.OrderedAt.Date == dateToCompare.Date);
            }

            if (!string.IsNullOrEmpty(queryOrder.Status))
            {
                listOrders = listOrders.Where(x => x.Status.ToLower().Contains(queryOrder.Status.ToLower()));
            }
            return await listOrders.ApplyQuery(queryOrder);
        }
        public async Task<PaginatedModel<GetOrder>> GetByUser(QueryOrderModel queryOrder)
        {
            var userID = _authService.GetUserID();
            var listOrders = _context.Orders
                .Where(o => o.UserID == userID)
                .OrderByDescending(x => x.OrderedAt)
                .ProjectTo<GetOrder>(_mapper.ConfigurationProvider)
                .AsQueryable();
            if (queryOrder.OrderDate.HasValue)
            {
                listOrders = listOrders.Where(x => DateOnly.FromDateTime(x.OrderedAt) == queryOrder.OrderDate.Value);
            }
            if (!string.IsNullOrEmpty(queryOrder.Status))
            {
                listOrders = listOrders.Where(x => x.Status.ToLower().Contains(queryOrder.Status.ToLower()));
            }
            return await listOrders.ApplyQuery(queryOrder);
        }

        public async Task<GetDetailOrder?> GetByID(int orderID)
        {
            return await _context.Orders.Where(x => x.OrderID == orderID)
                .ProjectTo<GetDetailOrder>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(); 
        }
        public string GenerateOrderConfirmationEmail(Order order)
        {
            var discount = order.Discount ?? 0;
            var discountofcoupon = order.DiscountOfCoupon ?? 0;

            var totalAmount = order.TotalAmount - discount - discountofcoupon;
            var url = _config["OriginFE"];

            var emailBody = $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Order Confirmation</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .email-container {{
                max-width: 600px;
                margin: 0 auto;
                background-color: #ffffff;
                border: 1px solid #dddddd;
                padding: 20px;
                color: #333333;
            }}
            .email-header {{
                text-align: center;
                margin-bottom: 20px;
            }}
            .email-header a {{
                text-decoration: none;
            }}
            .email-header img {{
                max-width: 150px;
            }}
            .email-content {{
                line-height: 1.6;
            }}
            .order-details, .product-details {{
                margin-top: 20px;
            }}
            .order-details table, .product-details table {{
                width: 100%;
                border-collapse: collapse;
            }}
            .order-details th, .product-details th {{
                background-color: #f4f4f4;
                padding: 10px;
                text-align: left;
            }}
            .order-details td, .product-details td {{
                padding: 10px;
                border-bottom: 1px solid #dddddd;
            }}
            .total-price {{
                font-weight: bold;
                color: #d9534f;
            }}
            .note, .qr-code {{
                margin-top: 20px;
            }}
            .qr-code img {{
                max-width: 100px;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <!-- Logo -->
            <div class='email-header'>
                <a href='{url}'>
                    <img src='{url}/logo.png' alt='Logo'>
                </a>
            </div>

            <!-- Email Content -->
            <div class='email-content'>
                <h2>Thank you for your order!</h2>
                <p>Your order has been successfully placed. Below are the details of your order:</p>

                <!-- Order Details -->
                <div class='order-details'>
                    <h3>Order Information</h3>
                    <table>
                        <tr>
                            <th>Order ID</th>
                            <td>{order.OrderID}</td>
                        </tr>
                        <tr>
                            <th>Order Date</th>
                            <td>{order.OrderedAt.ToString("dd MMMM yyyy")}</td>
                        </tr>
                        <tr>
                            <th>Amount</th>
                            <td>${order.TotalAmount}</td>
                        </tr>
                        <tr>
                            <th>Status</th>
                            <td>{order.Status}</td>
                        </tr>
                        <tr>
                            <th>Payment Method</th>
                            <td>{(order.PaymentMethod ? "Cart" : "Cash")}</td>
                        </tr>
                        <tr>
                            <th>Discount Of Coupon</th>
                            <td>
                                {(order.Coupon != null ? $"{order.Coupon.CouponCode} - {order.DiscountOfCoupon}$" : "No coupon applied")}
                            </td>
                        </tr>
                        <tr>
                            <th>Phone Number</th>
                            <td>{order.PhoneNumber}</td>
                        </tr>
                        <tr>
                            <th>Shipping Address</th>
                            <td>{order.ShippingAddress}</td>
                        </tr>
                        <tr>
                            <th>Note</th>
                            <td>{order.Note}</td>
                        </tr>
                        <tr>
                            <th>Total Amount</th>
                            <td>{totalAmount}</td>
                        </tr>
                    </table>
                </div>

                <!-- Product Details -->
                <div class='product-details'>
                    <h3>Product Details</h3>
                    <table>
                        <tr>
                            <th>Product Name</th>
                            <th>Image</th>
                            <th>Price</th>
                            <th>Discount</th>
                            <th>Quantity</th>
                            <th>Total</th>
                        </tr>";

            // Check if order details are not empty before iterating
            if (order.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                foreach (var item in order.OrderDetails)
                {
                    emailBody += $@"
                        <tr>
                            <td>{item.Product.ProductName}</td>
                            <td><img src='{url + item.Product.MainImage}' alt='{item.Product.ProductName}' style='width:50px;'></td>
                            <td>${item.UnitPrice}</td>
                            <td>${item.Discount}</td>
                            <td>{item.Quantity}</td>
                            <td>${item.UnitPrice * (100 - item.Discount) / 100 * item.Quantity}</td>
                        </tr>";
                }
            }
            else
            {
                emailBody += "<tr><td colspan='6'>No products found.</td></tr>";
            }

            emailBody += $@"
                    </table>
                </div>

                <!-- QR Code -->
                <div class='qr-code'>
                    <h3>Track Your Order</h3>              
                    <p>Scan the QR code below to track your order!</p>
                </div>
                <p>Thank you for shopping with us!</p>
            </div>
        </div>
    </body>
    </html>";

            return emailBody;
        }



        public async Task<ServiceResponse> ChangeStatus(int orderID, string status)
        {
            var order = await _context.Orders.FindAsync(orderID);
            if (order == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "No order found with given ID",
                    StatusCode = 404
                };
            }
            var oldStatus = order.Status;
            order.Status = status.ToUpper();
            _context.Orders.Update(order);

            var statusHistory = new OrderStatusHistory
            {
                OrderID = orderID,
                Status = order.Status
            };
            await _context.OrderStatusHistories.AddAsync(statusHistory);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var user = await _userManager.FindByIdAsync(order.UserID);
                var newEmail = new MailRequest { 
                    ToEmail = user.Email,
                    Subject = "Change Order status",
                    Body = GenerateOrderStatusNotification(orderID,oldStatus,status,user.Fullname)
                };
                await _mailService.SendEmailAsync(newEmail);
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Change order status successfully",
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to change status. Please try again later",
                StatusCode = 400
            };
        }

        public async Task<ServiceResponse> ChangeStatusUser(int orderID, string status, string? cancellationReason = null)
        {
            // Tìm đơn hàng theo ID
            var order = await _context.Orders.FindAsync(orderID);
            if (order == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "No order found with the given ID",
                    StatusCode = 404
                };
            }

            string oldStatus = order.Status;
            order.Status = status.ToUpper();
            _context.Orders.Update(order);

            var statusHistory = new OrderStatusHistory
            {
                OrderID = orderID,
                Status = order.Status
            };
            await _context.OrderStatusHistories.AddAsync(statusHistory);

            if (order.Status == "CANCELED")
            {
                if (string.IsNullOrEmpty(cancellationReason))
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Cancellation reason is required for cancelled orders",
                        StatusCode = 400
                    };
                }

                var userID = _authService.GetUserID();
                var cancellation = new OrderCancellation
                {
                    OrderID = orderID,
                    Reason = cancellationReason,
                    UserID = userID
                };
                await _context.OrderCancellations.AddAsync(cancellation);

                if (!string.IsNullOrEmpty(userID))
                {
                    var today = DateTime.Today;
                    int cancellationCount = await _context.OrderCancellations
                        .Where(c => c.UserID == userID && c.CancelledDate.Date == today)
                        .CountAsync();

                    if (cancellationCount >= 2)
                    {
                        // Khóa tài khoản
                        var user = await _context.Users.FindAsync(userID);
                        if (user != null)
                        {
                            user.LockoutEnabled = true;
                            user.LockoutEnd = DateTimeOffset.UtcNow.AddDays(1);
                            _context.Users.Update(user);

                            await SendAccountLockEmail(user.Email, user.Fullname, user.LockoutEnd.Value.DateTime);
                        }
                    }
                }
            }

            // Lưu thay đổi vào database
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = $"Order status changed from {oldStatus} to {order.Status} successfully",
                    StatusCode = 200
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to change status. Please try again later",
                StatusCode = 400
            };
        }
        public async Task SendAccountLockEmail(string userEmail, string fullName, DateTime lockoutEnd)
        {
            var subject = "Account Locked Notification";
            var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Account Locked</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border: 1px solid #dddddd;
            padding: 20px;
            color: #333333;
        }}
        .email-header {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .email-content {{
            line-height: 1.6;
        }}
        .alert {{
            color: #d9534f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h1>Account Locked</h1>
        </div>
        <div class='email-content'>
            <p>Dear {fullName},</p>
            <p class='alert'>Your account has been locked for 1 day due to suspicious activity or violations of our policies.</p>
            <p>The account lockout will end on <strong>{lockoutEnd:dddd, MMMM dd, yyyy hh:mm tt}</strong>.</p>
            <p>If you believe this is a mistake, please contact our support team for assistance.</p>
            <p>Thank you for your understanding.</p>
        </div>
    </div>
</body>
</html>";

            var mailRequest = new MailRequest
            {
                Subject = subject,
                ToEmail = userEmail,
                Body = emailBody
            };

            await _mailService.SendEmailAsync(mailRequest);
        }


        public async Task<List<RecentlyOrder>> GetRecently()
        {
            var order = await _context.Orders.OrderByDescending(x => x.OrderedAt)
                .Take(10).ProjectTo<RecentlyOrder>(_mapper.ConfigurationProvider).ToListAsync();
            return order;
        }



        public async Task<ServiceResponse> GetOrderWithUserId()
        {
            var orderWithUserId = await _context.Orders
                .Select(x => new
                {
                    x.OrderID,
                    UserID = x.User.Id, // Adjust if UserId is directly in Orders table
                    x.Status,
                    x.OrderedAt
                })
                .ToListAsync();

            if (orderWithUserId.Any())
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Orders retrieved successfully",
                    StatusCode = 200,
                    Data = orderWithUserId // Include the data in the response
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "No orders found for the given user",
                StatusCode = 404
            };
        }

        public async Task<ServiceResponse> GetOrderDetailsWithProductId()
        {
            var orderDetails = await _context.OrderDetails
                .Select(x => new
                {
                    x.TotalPrice,
                    x.Product.ProductID,
                    x.Order.OrderID,
                    x.Order.Status,
                    x.Quantity
                })
                .ToListAsync();

            if (orderDetails.Any())
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Order details retrieved successfully",
                    StatusCode = 200,
                    Data = orderDetails
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "No order details found for the given product",
                StatusCode = 404
            };
        }

        public string GenerateOrderStatusNotification(int orderId, string oldStatus, string newStatus, string customerName)
        {
            string notificationMessage = $"Order #{orderId} for customer '{customerName}' " +
                                         $"status has been changed from '{oldStatus}' to '{newStatus}'." + "Contact: nguyenphuc14112003@gmail.com. Phone: 0898827656";

            return notificationMessage;
        }

    }
}
