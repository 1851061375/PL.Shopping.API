using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Mailing;
using TD.WebApi.Application.Identity.Users.Password;

namespace TD.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        EnsureValidTenant();

        var user = await _userManager.FindByNameAsync(request.UserName.Normalize());
        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(request.UserName.Normalize());
            if (user is null)
            {
                user = await _userManager.Users
             .AsNoTracking()
             .Where(u => u.PhoneNumber == request.UserName)
             .FirstOrDefaultAsync(default);
                if (user is null)
                {
                    throw new InternalServerException(_t["An Error has occurred!"]);
                }
            }
        }

        if (user is null)
        {
            throw new InternalServerException(_t["An Error has occurred!"]);
        }

        string code = await _userManager.GeneratePasswordResetTokenAsync(user);

        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            var options = new RestClientOptions("https://api.hanhchinhcong.net");
            var client = new RestClient(options);
            var requestSMS = new RestRequest("/sms/SendSMSPublic", Method.Post);
            requestSMS.AddHeader("Content-Type", "application/json");
            requestSMS.AddHeader("Authorization", "Bearer 3bcd9fb7-2e0e-3adb-8ba9-ecab0e37916f");
            var requestBody = new RequestBody
            {
                soDienThoai = user.PhoneNumber,
                maPhanMem = "DVC",
                idMauTin = "1194861",
                noiDungthamSo = code,
                gioGui = string.Empty
            };
            requestSMS.AddJsonBody(requestBody);
            RestResponse response = await client.ExecuteAsync(requestSMS);

            return _t["Mã xác thực đã được gửi tới số điện thoại của bạn! Vui lòng kiểm tra và làm theo hướng dẫn."];
        }
        else if (!string.IsNullOrWhiteSpace(user.Email))
        {
            const string route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

            var content = await _templateService.GenerateEmailTemplateAsync("ForgotPassword", new
            {
                Code = code,
                FullName = user.FullName
            }, default);

            var mailRequest = new MailRequest(
               new List<string> { user.Email! },
               _t["Lấy lại mật khẩu"],
               content);

            /* var mailRequest = new MailRequest(
                 new List<string> { user.Email! },
                 _t["Đổi mật khẩu"],
                 _templateService.GenerateEmailTemplate("email-dangkithanhcong", new
                 {
                     NoiDung = $"Để đổi mật khẩu, vui lòng sử dụng Mã xác thực (OTP) sau: {code}. Không chia sẻ OTP này với bất kỳ ai. Nhóm dịch vụ khách hàng của chúng tôi sẽ không bao giờ yêu cầu bạn cung cấp mật khẩu, OTP, thẻ tín dụng hoặc thông tin ngân hàng của bạn."
                 }));*/
            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));

            return _t["Mã xác thực đã được gửi tới email của bạn! Vui lòng kiểm tra và làm theo hướng dẫn."];
        }
        else
        {
            return _t["Rất tiếc, chúng tôi không tìm thấy thông tin số điện thoại/email liên kết với tài khoản bạn muốn lấy lại mật khẩu. Vui lòng liên hệ quản trị viên để được hỗ trợ."];
        }
    }

    private class NoiDungHTMLModel
    {
        public string? NoiDung { get; set; }
        public string TieuDe { get; set; } = default!;
    }

    private class RequestBody
    {
        public string soDienThoai { get; set; }
        public string maPhanMem { get; set; }
        public string idMauTin { get; set; }
        public string gioGui { get; set; }
        public string noiDungthamSo { get; set; }
    }


    /*public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        EnsureValidTenant();

        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new InternalServerException(_t["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest(
            new List<string> { request.Email },
            _t["Reset Password"],
            _t[$"Your Password Reset Token is '{code}'. You can reset your password using the {endpointUri} Endpoint."]);
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));

        return _t["Password Reset Mail has been sent to your authorized Email."];
    }*/

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName.Normalize());
        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(request.UserName.Normalize());
            if (user is null)
            {
                user = await _userManager.Users
             //.AsNoTracking()
             .Where(u => u.PhoneNumber == request.UserName)
             .FirstOrDefaultAsync(default);
                if (user is null)
                {
                    throw new InternalServerException(_t["An Error has occurred!"]);
                }
            }
        }

        _ = user ?? throw new InternalServerException(_t["An Error has occurred!"]);

        var result = await _userManager.ResetPasswordAsync(user, request.Token!, request.Password!);

        return result.Succeeded
            ? _t["Password Reset Successful!"]
            : throw new InternalServerException(_t["An Error has occurred!"]);
    }

    public async Task<bool> AdminResetPasswordAsync(AdminResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            throw new InternalServerException(_t["An Error has occurred!"]);
        }

        var account = await _userManager.FindByNameAsync(request.UserName);

        if (account == null) throw new InternalServerException($"No Accounts Registered.");

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(account);

        var result = await _userManager.ResetPasswordAsync(account, resetToken, request.Password);

        if (result.Succeeded)
        {
            return true;
        }
        else
        {
            throw new InternalServerException($"Error occured while reseting the password.");
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Change password failed"], result.GetErrors(_t));
        }
    }
}