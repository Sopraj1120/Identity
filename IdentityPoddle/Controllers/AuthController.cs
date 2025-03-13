using IdentityPoddle.Database;
using IdentityPoddle.Entity;
using IdentityPoddle.Models;
using IdentityPoddle.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPoddle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly UserDbcontext _context;
        private readonly IEmailSender<User> _emailSender;

        public AuthController(UserManager<User> userManager, UserDbcontext context, IEmailSender<User> emailSender)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
         
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address ?? "",
                Role = "User"
            };

            var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok(new {Message = "User create Successfully!"});
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email).ConfigureAwait(false);
            if (user == null)
            {
                return BadRequest(new { Message = "User not found" });
            }

            var otp = new Random().Next(100000, 999999).ToString();
            var otpRecord = new OtpRecord
            {
                UserId = user.Id,
                Otp = otp,
                CreatedAt = DateTime.Now,
                IsUsed = false
            };

            _context.OtpRecords.Add(otpRecord);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            await _emailSender.SendPasswordResetCodeAsync(user, forgotPasswordModel.Email, otp);
            return Ok(new { Message = "OTP sent to your email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword (ResetPasswordModel resetPasswordModel)
        {
            var otpRecord = await _context.OtpRecords.Include(u => u.User)
                .FirstOrDefaultAsync(x => x.Otp == resetPasswordModel.Otp && !x.IsUsed && x.CreatedAt> DateTime.UtcNow.AddMinutes(-10))
                .ConfigureAwait(false);

            if(otpRecord == null) return BadRequest("Invalid or expired OTP");

            var user = otpRecord.User;

            if(user == null) return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordModel.NewPassword).ConfigureAwait(false);

            if (!result.Succeeded) return BadRequest(result.Errors);

            otpRecord.IsUsed = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new { Message = "Password reset successfully" });
        }
    }
}
