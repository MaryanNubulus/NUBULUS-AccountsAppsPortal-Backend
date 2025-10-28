
using NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.Models.Requests;

public record CreateEmployeeRequest
{
    public Email Email { get; }
    public string Name { get; }

    private CreateEmployeeRequest(string email, string name)
    {
        try
        {
            Email = Email.Create(email);
        }
        catch (Exception)
        {
            throw;
        }
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100 || name.Length < 2)
        {
            throw new ArgumentException("Invalid name.", nameof(name));
        }
        Name = name;
    }
    public static CreateEmployeeRequest Create(string email, string name)
    {
        return new CreateEmployeeRequest(email, name);
    }
}