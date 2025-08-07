using System.Text;

using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Identity;

public partial class UserService
{
    private async Task<Result<string>> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        try
        {
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "api/users/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

            return Result<string>.Success(verificationUri);
        }
        catch (Exception ex)
        {
            return Result<string>.Error($"An error occurred while generating the verification URI: {ex.Message}");
        }
    }

    public async Task<Result<string>> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId && !u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result<string>.NotFound("User not found or email already confirmed.");

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            return result.Succeeded
                ? Result<string>.Success($"Account Confirmed for E-Mail {user.Email}. You can now use the /api/tokens endpoint to generate JWT.")
                : Result<string>.Error($"An error occurred while confirming {user.Email}");
        }
        catch (Exception ex)
        {
            return Result<string>.Error($"An error occurred while confirming E-Mail: {ex.Message}");
        }
    }

    public async Task<Result<string>> ConfirmPhoneNumberAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            return Result<string>.NotFound("User not found or phone number is empty.");

        try
        {
            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);

            if (result.Succeeded)
            {
                var message = user.PhoneNumberConfirmed
                    ? $"Account Confirmed for Phone Number {user.PhoneNumber}. You can now use the /api/tokens endpoint to generate JWT."
                    : $"Account Confirmed for Phone Number {user.PhoneNumber}. You should confirm your E-mail before using the /api/tokens endpoint to generate JWT.";
                
                return Result<string>.Success(message);
            }
            else
            {
                return Result<string>.Error($"An error occurred while confirming {user.PhoneNumber}");
            }
        }
        catch (Exception ex)
        {
            return Result<string>.Error($"An error occurred while confirming Phone Number: {ex.Message}");
        }
    }
}
