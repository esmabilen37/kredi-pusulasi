namespace FinansalAsistanApi.Services;

public static class TasitKrediRules
{
    public static bool IsAllowed(decimal amount, int maturity)
    {
        return amount switch
        {
            > 0 and <= 280000 when maturity <= 48 => true,
            > 280000 and <= 400000 when maturity <= 36 => true,
            _ => false
        };
    } 
}

