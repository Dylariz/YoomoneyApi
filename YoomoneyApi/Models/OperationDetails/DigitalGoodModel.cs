using YoomoneyApi.Operation;

namespace YoomoneyApi.Models.OperationDetails;

public class DigitalGoodModel
{
    public IEnumerable<DigitalBonus>? products;
    public IEnumerable<DigitalProduct>? bonuses;
}