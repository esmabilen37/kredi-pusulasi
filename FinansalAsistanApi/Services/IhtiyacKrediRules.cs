namespace FinansalAsistanApi.Services;

public static class IhtiyacKrediRules
{
   public static bool IsAllowed(decimal amount, int maturity)
   {
       return amount switch
       {
           > 0 and < 125000 when maturity <= 36 => true,
           > 125000 and < 250000 when maturity <= 24 => true,
           > 0 when maturity <= 12 => true,
           _ => false
       };
   } 
}


