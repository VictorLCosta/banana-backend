namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; set; } = default!;
    public string BuildingNumber { get; set; } = default!;
    public string Neighborhood { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return BuildingNumber;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return PostalCode;
    }
}
