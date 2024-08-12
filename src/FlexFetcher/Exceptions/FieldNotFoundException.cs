namespace FlexFetcher.Exceptions;

public class FieldNotFoundException : Exception
{
    public FieldNotFoundException(string field) : base($"Field {field} not found.")
    {
    }

    public FieldNotFoundException(string field, string entity) : base($"Field {field} not found in entity {entity}.")
    {
    }
}