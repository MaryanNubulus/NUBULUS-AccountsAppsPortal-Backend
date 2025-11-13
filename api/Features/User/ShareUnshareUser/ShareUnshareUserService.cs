using Nubulus.Backend.Api.Features.Common;
using Nubulus.Domain.Abstractions;
using Nubulus.Domain.ValueObjects;

namespace Nubulus.Backend.Api.Features.User.ShareUnshareUser;

public class ShareUnshareUserService
{
    internal class ShareUnshareUserResponse : IGenericResponse<string>
    {
        public ResultType ResultType { get; set; } = ResultType.None;
        public string? Message { get; set; } = null;
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = null;
        public string? Data { get; set; } = null;

        private ShareUnshareUserResponse(ResultType resultType, string? message, string? data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
            ValidationErrors = null;
        }

        public static ShareUnshareUserResponse Success(string message)
        {
            return new ShareUnshareUserResponse(ResultType.Ok, message, message);
        }

        public static ShareUnshareUserResponse NotFound(string message)
        {
            return new ShareUnshareUserResponse(ResultType.NotFound, message, null);
        }

        public static ShareUnshareUserResponse Error(string message)
        {
            return new ShareUnshareUserResponse(ResultType.Error, message, null);
        }

        public static ShareUnshareUserResponse Conflict(string message)
        {
            return new ShareUnshareUserResponse(ResultType.Conflict, message, null);
        }
    }

    private readonly IUnitOfWork _unitOfWork;

    public ShareUnshareUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IGenericResponse<string>> ShareAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            var accountIdValue = new AccountId(accountId);
            var userIdValue = new UserId(userId);
            var currentUserEmail = new EmailAddress(userContextEmail);

            // Compartir el usuario
            await _unitOfWork.Users.ShareUserAsync(userIdValue, accountIdValue, currentUserEmail, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return ShareUnshareUserResponse.Success("User shared successfully.");
        }
        catch (InvalidOperationException ex)
        {
            // Errores de validación de negocio (usuario no encontrado, ya compartido, etc.)
            if (ex.Message.Contains("not found"))
                return ShareUnshareUserResponse.NotFound(ex.Message);
            if (ex.Message.Contains("already shared"))
                return ShareUnshareUserResponse.Conflict(ex.Message);

            return ShareUnshareUserResponse.Error(ex.Message);
        }
        catch (Exception ex)
        {
            return ShareUnshareUserResponse.Error($"Error sharing user: {ex.Message}");
        }
    }

    public async Task<IGenericResponse<string>> UnshareAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    {
        try
        {
            var accountIdValue = new AccountId(accountId);
            var userIdValue = new UserId(userId);
            var currentUserEmail = new EmailAddress(userContextEmail);

            // Dejar de compartir el usuario
            await _unitOfWork.Users.UnshareUserAsync(userIdValue, accountIdValue, currentUserEmail, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return ShareUnshareUserResponse.Success("User unshared successfully.");
        }
        catch (InvalidOperationException ex)
        {
            // Errores de validación de negocio
            if (ex.Message.Contains("not found"))
                return ShareUnshareUserResponse.NotFound(ex.Message);
            if (ex.Message.Contains("not shared") || ex.Message.Contains("Cannot unshare"))
                return ShareUnshareUserResponse.Conflict(ex.Message);

            return ShareUnshareUserResponse.Error(ex.Message);
        }
        catch (Exception ex)
        {
            return ShareUnshareUserResponse.Error($"Error unsharing user: {ex.Message}");
        }
    }
}
