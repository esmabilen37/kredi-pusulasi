using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinansalAsistanApi.Models;

public class BankProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }
    public decimal MaxDebtToIncomeRatio { get; set; }
    public decimal MinMonthlyIncome { get; set; }
    public string Description { get; set; } = string.Empty;

    // Kredi türüne göre ayrı faiz/limit/vade. Bir banka bir kredi türünü hiç sunmuyorsa o tür için burada hiç kayıt olmaz.
   
    public List<LoanTypeOffer> LoanTypeOffers { get; set; } = new();

    public LoanTypeOffer? GetOfferFor(LoanType loanType) =>
        LoanTypeOffers.FirstOrDefault(o => o.LoanType == loanType);
}