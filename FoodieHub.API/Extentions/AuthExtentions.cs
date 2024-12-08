using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodieHub.API.Extentions
{
    public static class AuthExtentions
    {
        public static string GenerateToken(string userID, string jwtKey, string? role)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userID)
            };
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDesciption = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = creds
            };
            var token = jwtSecurityTokenHandler.CreateToken(tokenDesciption);
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        public static string GenerateAccountRestoredEmail(string fullName, DateTime restoredDate)
        {
            return $@"
        Dear {fullName},

        We are pleased to inform you that your account has been successfully restored as of {restoredDate}.

        You can now log in and continue using our services without any restrictions. If you have any questions or encounter any issues, please feel free to contact our support team at support@example.com.

        Thank you for your patience and understanding during this time. We value your trust and are committed to providing you with the best possible experience.

        Best regards,
        The Support Team
    ";
        }

        public static string GenerateAccountDisabledEmail(string fullname, DateTime disabledDate)
        {
            return $@"
        Dear {fullname},

        We are writing to inform you that your account has been disabled as of {disabledDate:MMMM dd, yyyy}.

        This action was taken to ensure the security of your account or due to a violation of our terms of service. 
        If you believe this was done in error or would like to resolve the issue, please contact our support team at nguyenphuc14112003@gmail.com.

        We apologize for any inconvenience this may cause and appreciate your understanding.

        Best regards,
        The Support Team
    ";
        }

        public static string GenerateForgotPasswordEmailBody(string resetPasswordLink, string fullName)
        {
            // Tạo nội dung email với HTML
            string emailBody = $@"
               <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 50px auto;
                    background-color: #ffffff;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    padding: 20px;
                }}
                h2 {{
                    color: #333;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                }}
                a {{
                    display: inline-block;
                    background-color: #007bff;
                    color: white;
                    padding: 10px 20px;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 20px;
                    font-weight: bold;
                }}
                a:hover {{
                    background-color: #0056b3;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Hello {fullName},</h2>
                <p>You have requested to reset your password. This link will expire in 5 minutes. Please click the button below to reset your password:</p>
                <a style={"color:white"} href='{resetPasswordLink}' target='_blank'>Reset Password</a>
                <p class='footer'>If you did not request a password reset, please ignore this email.</p>
                <p class='footer'>Thank you!</p>
            </div>
        </body>
        </html>

            ";

            return emailBody;
        }


        public static string GenerateRegistrationSuccessEmail(string userName, string confirmationLink)
        {
            // Email body structure
            var emailBody = $@"
<html>
<head>
    <style>
        .email-body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
        }}
        .email-header {{
            background-color: #4CAF50;
            color: white;
            padding: 10px;
            text-align: center;
            font-size: 24px;
        }}
        .email-content {{
            padding: 20px;
            font-size: 16px;
        }}
        .email-footer {{
            font-size: 14px;
            text-align: center;
            margin-top: 20px;
            color: #777;
        }}
        .confirmation-link {{
            background-color: #4CAF50;
            color: white;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 5px;
            display: inline-block;
        }}
    </style>
</head>
<body class='email-body'>
    <div class='email-header'>
        Registration Successful
    </div>
    <div class='email-content'>
        <p>Dear {userName},</p>
        <p>Thank you for registering an account on our website.</p>
        <p>We are excited to welcome you to our community. To complete your registration, please confirm your email by clicking the link below:</p>
        <p><a href='{confirmationLink}' class='confirmation-link'>Confirm your email</a></p>
        <p>If you did not register for this account, please ignore this email.</p>
        <p>If you have any questions, feel free to reach out to us.</p>
        <p>We wish you a wonderful day!</p>
    </div>
    <div class='email-footer'>
        <p>Best regards,<br>Support Team</p>
    </div>
</body>
</html>";

            return emailBody;
        }
    }
}
