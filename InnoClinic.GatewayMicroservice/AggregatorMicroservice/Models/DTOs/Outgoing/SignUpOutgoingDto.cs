namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class SignUpOutgoingDto
{
    public string AccountId { get; set; }
    public TokensOutgoingDto Tokens { get; set; }
}
