namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Common.ValueObjects;

public record IdObject
{
    public Guid Value { get; init; }

    public static IdObject Create(Guid value) => new IdObject(value);
    public static Guid ValidateId(Guid id)
    {
        var idVO = new IdObject(id);
        return idVO.Value;
    }
    private IdObject(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be an empty GUID.", nameof(id));
        }

        Value = id;
    }


}