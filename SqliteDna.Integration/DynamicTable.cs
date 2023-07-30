namespace SqliteDna.Integration
{
    public record DynamicTable(string Schema, IEnumerable<object[]> Data);
}
