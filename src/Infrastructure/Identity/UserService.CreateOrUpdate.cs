using System.Security.Claims;

using Application.Core.Identity.Users.Features;

namespace Infrastructure.Identity;

public partial class UserService
{
    public Task<Result<string>> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<string>> CreateAsync(CreateUserCommand command, string origin)
    {
        var user = new ApplicationUser
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.UserName,
            PhoneNumber = command.PhoneNumber,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            return Result<string>.Error("An error occurred while creating the user.");
        }

        await _userManager.AddToRoleAsync(user, "");

        var messages = new List<string> { string.Format("User {0} Registered.", user.UserName) };

        /*if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))
        {
            // send verification email
            string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
            RegisterUserEmailModel eMailModel = new RegisterUserEmailModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                Url = emailVerificationUri
            };
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                _t["Confirm Registration"],
                _templateService.GenerateEmailTemplate("email-confirmation", eMailModel));
            _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));
            messages.Add(_t[$"Please check {user.Email} to verify your account!"]);
        }*/

        return Result.Success(string.Join(Environment.NewLine, messages));
    }

    public async Task<Result> UpdateAsync(UpdateUserRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return Result.NotFound("User not found.");

        /*string currentImage = user.ImageUrl ?? string.Empty;
        if (request.Image != null || request.DeleteCurrentImage)
        {
            //user.ImageUrl = await _fileStorage.UploadAsync<ApplicationUser>(request.Image, FileType.Image);
            if (request.DeleteCurrentImage && !string.IsNullOrEmpty(currentImage))
            {
                string root = Directory.GetCurrentDirectory();
                //_fileStorage.Remove(Path.Combine(root, currentImage));
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

        if (!result.Succeeded)
        {
            return Result.Error("An error occurred while updating the user.");
        }

        return Result.NoContent();
    }
}
