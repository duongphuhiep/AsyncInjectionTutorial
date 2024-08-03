namespace Common;

public record Payment
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
}
