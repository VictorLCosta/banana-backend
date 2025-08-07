using Application.Core.Identity.Users.Features;

using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Identity;

public partial class UserService
{
    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.NotFound("User Not Found.");

        var result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        return result.Succeeded
            ? Result.Success()
            : Result.Error("Change password failed");
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            return Result<string>.Error("An error has occurred!");
        }

        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri($"{origin}/{route}");
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

        /*var mailRequest = new MailRequest(
            new List<string> { request.Email },
            "Reset Password",
            $"Your Password Reset Token is '{code}'. You can reset your password using the {passwordResetUrl} Endpoint.");
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));*/

        return Result<string>.Success("Password Reset Mail has been sent to your authorized Email.");
    }

    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email?.Normalize()!);
        if (user == null)
            return Result<string>.NotFound("An error has occurred!");

        var result = await _userManager.ResetPasswordAsync(user, request.Token!, request.Password!);

        return result.Succeeded
            ? Result<string>.Success("Password Reset Successful!")
            : Result<string>.Error("An error has occurred!");
    }
}
