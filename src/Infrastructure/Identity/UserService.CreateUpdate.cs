using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Identity.Users;
using TD.WebApi.Domain.Identity;
using TD.WebApi.Shared.Authorization;
using TD.WebApi.Shared.Utils;

namespace TD.WebApi.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string? objectId = principal.GetObjectId();
        if (string.IsNullOrWhiteSpace(objectId))
        {
            throw new InternalServerException(_t["Invalid objectId"]);
        }

        var user = await _userManager.Users.Where(u => u.ObjectId == objectId).FirstOrDefaultAsync()
            ?? await CreateOrUpdateFromPrincipalAsync(principal);

        if (principal.FindFirstValue(ClaimTypes.Role) is string role &&
            await _roleManager.RoleExistsAsync(role) &&
            !await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return user.Id;
    }

    private async Task<ApplicationUser> CreateOrUpdateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        string? email = principal.FindFirstValue(ClaimTypes.Upn);
        string? username = principal.GetDisplayName();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            throw new InternalServerException(string.Format(_t["Username or Email not valid."]));
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new InternalServerException(string.Format(_t["Username {0} is already taken."], username));
        }

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new InternalServerException(string.Format(_t["Email {0} is already taken."], email));
            }
        }

        IdentityResult? result;
        if (user is not null)
        {
            user.ObjectId = principal.GetObjectId();
            result = await _userManager.UpdateAsync(user);

            await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
        }
        else
        {
            user = new ApplicationUser
            {
                ObjectId = principal.GetObjectId(),
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = principal.FindFirstValue(ClaimTypes.Surname),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            result = await _userManager.CreateAsync(user);

            await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));
        }

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
        }

        return user;
    }

    public async Task<string> CreateOtherAsync(CreateUserPublicRequest request, string origin)
    {
        string? phoneNumber = null;
        string? email = null;

        if (TDUtils.IsPhoneNumber(request.UserName))
        {
            phoneNumber = request.UserName;

            bool check = await ExistsWithPhoneNumberAsync(phoneNumber, null);
            if (check)
            {
                throw new InternalServerException($"Đã tồn tại thông tin số điện thoại {phoneNumber} trong hệ thống, vui lòng thử lại!");
            }

        }
        else if (TDUtils.IsEmailAddress(request.UserName))
        {
            email = request.UserName;
            bool check = await ExistsWithEmailAsync(email, null);
            if (check)
            {
                throw new InternalServerException($"Đã tồn tại thông tin email {email} trong hệ thống, vui lòng thử lại!");
            }
        }
        else
        {
            throw new InternalServerException("Tài khoản không đúng định dang, vui lòng thử lại!");
        }

        int type = request.Type == 1 ? 1 : request.Type == 4 ? 4 : request.Type == 5 ? 5 : 2;

        //Introduction
        string refcode = string.Empty;

        string? myRefCode = null;

        try
        {
            if (!string.IsNullOrEmpty(request.Introduction))
            {
                int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE MyRefCode = '{request.Introduction}' ", default);
                if (countUser > 0)
                {
                    refcode = request.Introduction;
                }
            }


            if (request.Type == 4 || request.Type == 5)
            {
                bool check = true;
                while (check)
                {
                    string myRefCodeTmp = TDUtils.GenerateUniqueCoupon(10);
                    int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE RefCode = '{myRefCodeTmp}' ", default);
                    if (countUser < 1)
                    {
                        myRefCode = myRefCodeTmp;
                        check = false;
                    }
                }
            }
        }
        catch { }
        var user = new ApplicationUser
        {
            Email = email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FullName = request.FullName,
            UserName = request.UserName,
            TaxCode = request.TaxCode,
            AgentName = request.AgentName,
            PhoneNumber = phoneNumber,
            IsActive = type == 4 || type == 5 ? false : true,
            Type = type,
            RefCode = refcode,
            MyRefCode = myRefCode
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
        }

        await _userManager.AddToRoleAsync(user, TDRoles.Basic);
        await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

        return user.Id;
    }

    public async Task<string> CreateAsync(CreateUserRequest request, string origin)
    {
        string? myRefCode = null;
        if (request.Type == 4 || request.Type == 5)
        {
            bool check = true;
            while (check)
            {
                string myRefCodeTmp = TDUtils.GenerateUniqueCoupon(10);
                int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE RefCode = '{myRefCodeTmp}' ", default);
                if (countUser < 1)
                {
                    myRefCode = myRefCodeTmp;
                    check = false;
                }
            }
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FullName = request.FullName,
            UserName = request.UserName,
            ImageUrl = request.ImageUrl,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            ProvinceCode = request.ProvinceCode,
            DistrictCode = request.DistrictCode,
            WardCode = request.WardCode,
            IsActive = true,
            Type = request.Type,
            MyRefCode = myRefCode,
            CreatedBy = _currentUser.GetUserId()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
        }

        await _userManager.AddToRoleAsync(user, TDRoles.Basic);

        var messages = new List<string> { string.Format(_t["User {0} Registered."], user.UserName) };

        await _events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

        return string.Join(Environment.NewLine, messages);
    }

    public async Task UpdateAsync(UpdateUserRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        string currentImage = user.ImageUrl ?? string.Empty;
        /*if (request.Image != null || request.DeleteCurrentImage)
        {
            user.ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image);
            if (request.DeleteCurrentImage && !string.IsNullOrEmpty(currentImage))
            {
                string root = Directory.GetCurrentDirectory();
                _fileStorage.Remove(Path.Combine(root, currentImage));
            }
        }*/

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        var result = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Update profile failed"], result.GetErrors(_t));
        }
    }


    public async Task UpdateAsync(PersonalUpdateUserRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FullName = request.FullName;
        user.ImageUrl = request.ImageUrl;
        user.Gender = request.Gender;
        user.DateOfBirth = request.DateOfBirth;
        user.ProvinceCode = request.ProvinceCode;
        user.DistrictCode = request.DistrictCode;
        user.WardCode = request.WardCode;
        user.Address = request.Address;
        user.IsReceiveEmail = request.IsReceiveEmail;


        if ((user.Type == 1 && request.Type == 2) || (user.Type == 2 && request.Type == 1))
        {
            user.Type = request.Type;
        }


        if (!string.IsNullOrEmpty(request.RefCode))
        {
            int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE MyRefCode = '{request.RefCode}' ", default);
            if (countUser > 0)
            {
                user.RefCode = request.RefCode;
            }
        }

        if (!string.IsNullOrEmpty(request.MyRefCode))
        {
            int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE MyRefCode = '{request.MyRefCode}' AND Id != '{user.Id}'", default);
            if (countUser > 0)
            {
                // throw new InternalServerException("Đã có lỗi trong quá trình cập nhật thông tin, giá trị MyRefCode đã tồn tại");
            }
            else
            {
                user.MyRefCode = request.MyRefCode;
            }
        }

        user.PhoneNumber = request.PhoneNumber;
        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        user.Email = request.Email;
        string? email = await _userManager.GetEmailAsync(user);
        if (request.Email != email)
        {
            await _userManager.SetEmailAsync(user, request.Email);
        }

        var result = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Update profile failed"], result.GetErrors(_t));
        }
    }
    public async Task<string> UpdateDiscountConfigAsync(string userId, UpdateDiscountConfigRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        user.DiscountConfig = request.DiscountConfig;
        var result = await _userManager.UpdateAsync(user);

        return "done";
    }

        public async Task<bool> UpdateByUserIdAsync(UpdateUserRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        user.ImageUrl = request.ImageUrl;
        user.Gender = request.Gender;
        user.DateOfBirth = request.DateOfBirth;
        user.FullName = request.FullName;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;
        user.ProvinceCode = request.ProvinceCode;
        user.DistrictCode = request.DistrictCode;
        user.WardCode = request.WardCode;
        user.Gender = request.Gender;
        user.DateOfBirth = request.DateOfBirth;
        user.IsSpecial = request.IsSpecial;

        if (!string.IsNullOrEmpty(request.MyRefCode))
        {
            int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE MyRefCode = '{request.MyRefCode}' AND Id != '{user.Id}'", default);
            if (countUser > 0)
            {
                // throw new InternalServerException("Đã có lỗi trong quá trình cập nhật thông tin, giá trị MyRefCode đã tồn tại");
            }
            else
            {
                user.MyRefCode = request.MyRefCode;
            }
        }

        if (!string.IsNullOrEmpty(request.RefCode))
        {
            int countUser = await _dapperRepository.ExecuteScalarAsync<int>($"SELECT COUNT(Id) FROM [Identity].[Users] WHERE MyRefCode = '{request.RefCode}' ", default);
            if (countUser > 0)
            {
                user.RefCode = request.RefCode;
            }
        }

        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        user.Email = request.Email;
        string? email = await _userManager.GetEmailAsync(user);
        if (request.Email != email)
        {
            await _userManager.SetEmailAsync(user, request.Email);
        }

        var result = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Update profile failed"], result.GetErrors(_t));
        }

        return true;
    }

    public async Task<bool> UpdateAsyncNew(UpdateUserRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        //if (request.UserName.Equals(user.UserName, StringComparison.CurrentCultureIgnoreCase))
        {
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.FullName = request.FullName;
            user.LastName = request.LastName;
            await _userManager.SetUserNameAsync(user, request.UserName);

            string? email = await _userManager.GetEmailAsync(user);
            if (request.Email != email)
            {
                await _userManager.SetEmailAsync(user, request.Email);
            }

            var result = await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

            if (!result.Succeeded)
            {
                throw new InternalServerException(_t["Update profile failed"], result.GetErrors(_t));
            }
        }
        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
        }

        return true;
    }

    public async Task<bool> DeleteAsyncByUserId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        await _userManager.DeleteAsync(user);

        return true;

    }

}
