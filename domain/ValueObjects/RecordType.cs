namespace Nubulus.Domain.ValueObjects;

public class RecordType
{
    public string Value { get; private set; }

    private RecordType(string value)
    {
        Value = value;
    }

    public static RecordType Create => new RecordType("C");
    public static RecordType Update => new RecordType("U");
    public static RecordType Pause => new RecordType("P");
    public static RecordType Resume => new RecordType("R");
    public static RecordType Delete => new RecordType("D");


    public static RecordType Parse(string recordType)
    {
        return recordType switch
        {
            "C" => Create,
            "U" => Update,
            "P" => Pause,
            "R" => Resume,
            "D" => Delete,
            _ => throw new ArgumentException($"Invalid record type: {recordType}")
        };
    }

    public override string ToString()
    {
        return Value;
    }
}