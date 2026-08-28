using FinansalAsistanApi.Models;
using MongoDB.Driver;

namespace FinansalAsistanApi.Services;

public static class BankProfileSeeder
{
    // NOT: Bu değerler gerçek banka tekliflerinden esinlenilmiş, temsili örnek verilerdir gerçek zamanlı banka oranlarını yansıtmaz.
    private static readonly List<BankProfile> Banks = new()
    {
        new BankProfile { Name = "A Bankası", MaxDebtToIncomeRatio = 0.20m, MinMonthlyIncome = 25000m, Description = "Düşük risk toleransı, kurumsal odaklı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.19m, MaxLoanAmount = 300000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.29m, MaxLoanAmount = 5000000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 2.99m, MaxLoanAmount = 1500000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "B Bankası", MaxDebtToIncomeRatio = 0.22m, MinMonthlyIncome = 22000m, Description = "Konut kredisinde agresif fiyatlama sunan profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.35m, MaxLoanAmount = 250000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.19m, MaxLoanAmount = 6000000m, MaxTermMonths = 240 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.15m, MaxLoanAmount = 1200000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "C Bankası", MaxDebtToIncomeRatio = 0.24m, MinMonthlyIncome = 20000m, Description = "Dengeli, orta segment profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.45m, MaxLoanAmount = 350000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.45m, MaxLoanAmount = 4500000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.25m, MaxLoanAmount = 1300000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "D Bankası", MaxDebtToIncomeRatio = 0.25m, MinMonthlyIncome = 20000m, Description = "Taşıt kredisinde uzman, kampanyalı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.55m, MaxLoanAmount = 280000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.55m, MaxLoanAmount = 4000000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 2.85m, MaxLoanAmount = 1800000m, MaxTermMonths = 60 },
        }},
        new BankProfile { Name = "E Bankası", MaxDebtToIncomeRatio = 0.26m, MinMonthlyIncome = 18000m, Description = "Genç profesyonellere yönelik esnek profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.65m, MaxLoanAmount = 220000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.65m, MaxLoanAmount = 3500000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.35m, MaxLoanAmount = 1000000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "F Bankası", MaxDebtToIncomeRatio = 0.28m, MinMonthlyIncome = 17000m, Description = "Bölgesel şube ağı geniş, orta segment profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.75m, MaxLoanAmount = 260000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.7m, MaxLoanAmount = 3800000m, MaxTermMonths = 204 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.45m, MaxLoanAmount = 950000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "G Bankası", MaxDebtToIncomeRatio = 0.29m, MinMonthlyIncome = 16000m, Description = "Esnek vadeli, orta segment profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.85m, MaxLoanAmount = 240000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.75m, MaxLoanAmount = 3200000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.55m, MaxLoanAmount = 900000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "H Bankası", MaxDebtToIncomeRatio = 0.30m, MinMonthlyIncome = 15000m, Description = "Orta risk toleranslı, yaygın erişimli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.95m, MaxLoanAmount = 220000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.85m, MaxLoanAmount = 3000000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.65m, MaxLoanAmount = 850000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "I Bankası", MaxDebtToIncomeRatio = 0.30m, MinMonthlyIncome = 15000m, Description = "Kısa vadeli ihtiyaç kredisinde hızlı onay sunan profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.6m, MaxLoanAmount = 200000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.9m, MaxLoanAmount = 2800000m, MaxTermMonths = 156 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.7m, MaxLoanAmount = 800000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "J Bankası", MaxDebtToIncomeRatio = 0.25m, MinMonthlyIncome = 20000m, Description = "Yüksek gelir grubuna özel, düşük faizli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 3.4m, MaxLoanAmount = 320000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.5m, MaxLoanAmount = 4200000m, MaxTermMonths = 192 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.1m, MaxLoanAmount = 1400000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "K Bankası", MaxDebtToIncomeRatio = 0.32m, MinMonthlyIncome = 14000m, Description = "Orta risk toleranslı, geniş ürün yelpazeli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.05m, MaxLoanAmount = 260000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 2.95m, MaxLoanAmount = 2600000m, MaxTermMonths = 180 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.75m, MaxLoanAmount = 780000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "L Bankası", MaxDebtToIncomeRatio = 0.35m, MinMonthlyIncome = 13000m, Description = "Esnek onay kriterleri sunan orta segment profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.15m, MaxLoanAmount = 240000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.0m, MaxLoanAmount = 2400000m, MaxTermMonths = 156 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.85m, MaxLoanAmount = 700000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "M Bankası", MaxDebtToIncomeRatio = 0.36m, MinMonthlyIncome = 12000m, Description = "Taşıt kredisi kampanyalarıyla öne çıkan profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.25m, MaxLoanAmount = 220000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.1m, MaxLoanAmount = 2200000m, MaxTermMonths = 156 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.4m, MaxLoanAmount = 1100000m, MaxTermMonths = 54 },
        }},
        new BankProfile { Name = "N Bankası", MaxDebtToIncomeRatio = 0.38m, MinMonthlyIncome = 12000m, Description = "Orta-yüksek risk toleranslı, geniş erişimli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.35m, MaxLoanAmount = 200000m, MaxTermMonths = 36 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.2m, MaxLoanAmount = 2000000m, MaxTermMonths = 156 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 3.95m, MaxLoanAmount = 650000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "O Bankası", MaxDebtToIncomeRatio = 0.40m, MinMonthlyIncome = 11000m, Description = "Orta-yüksek risk toleranslı, hızlı süreç odaklı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.45m, MaxLoanAmount = 190000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.3m, MaxLoanAmount = 1800000m, MaxTermMonths = 144 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.05m, MaxLoanAmount = 600000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "P Bankası", MaxDebtToIncomeRatio = 0.40m, MinMonthlyIncome = 10000m, Description = "Düşük gelir gruplarına da açık, esnek profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.55m, MaxLoanAmount = 180000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.4m, MaxLoanAmount = 1700000m, MaxTermMonths = 144 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.15m, MaxLoanAmount = 550000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "R Bankası", MaxDebtToIncomeRatio = 0.42m, MinMonthlyIncome = 10000m, Description = "Yüksek risk toleranslı, geniş segment profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.65m, MaxLoanAmount = 170000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.5m, MaxLoanAmount = 1600000m, MaxTermMonths = 132 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.25m, MaxLoanAmount = 520000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "S Bankası", MaxDebtToIncomeRatio = 0.43m, MinMonthlyIncome = 9000m, Description = "Yüksek risk toleranslı, kampanyalı düşük gelir profili.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.75m, MaxLoanAmount = 160000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.6m, MaxLoanAmount = 1500000m, MaxTermMonths = 132 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.35m, MaxLoanAmount = 480000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "T Bankası", MaxDebtToIncomeRatio = 0.44m, MinMonthlyIncome = 8000m, Description = "Yüksek risk toleranslı, hızlı onaylı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.85m, MaxLoanAmount = 150000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.7m, MaxLoanAmount = 1400000m, MaxTermMonths = 120 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.45m, MaxLoanAmount = 450000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "U Bankası", MaxDebtToIncomeRatio = 0.45m, MinMonthlyIncome = 10000m, Description = "Taşıt odaklı, yüksek risk toleranslı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 4.9m, MaxLoanAmount = 145000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.75m, MaxLoanAmount = 1350000m, MaxTermMonths = 120 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.0m, MaxLoanAmount = 900000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "V Bankası", MaxDebtToIncomeRatio = 0.46m, MinMonthlyIncome = 8000m, Description = "Yüksek risk toleranslı, esnek limitli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.0m, MaxLoanAmount = 220000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.85m, MaxLoanAmount = 1900000m, MaxTermMonths = 144 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.55m, MaxLoanAmount = 700000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "Y Bankası", MaxDebtToIncomeRatio = 0.48m, MinMonthlyIncome = 7000m, Description = "Yüksek risk toleranslı, düşük gelir dostu profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.1m, MaxLoanAmount = 200000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.95m, MaxLoanAmount = 1750000m, MaxTermMonths = 132 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.65m, MaxLoanAmount = 650000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "Z Bankası", MaxDebtToIncomeRatio = 0.50m, MinMonthlyIncome = 6000m, Description = "Yüksek risk toleranslı, geniş kabul kriterli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.25m, MaxLoanAmount = 250000m, MaxTermMonths = 30 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.05m, MaxLoanAmount = 2100000m, MaxTermMonths = 144 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.75m, MaxLoanAmount = 800000m, MaxTermMonths = 42 },
        }},
        new BankProfile { Name = "AA Bankası", MaxDebtToIncomeRatio = 0.50m, MinMonthlyIncome = 6000m, Description = "Yüksek risk toleranslı, konutta agresif profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.15m, MaxLoanAmount = 180000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 3.9m, MaxLoanAmount = 2300000m, MaxTermMonths = 168 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.85m, MaxLoanAmount = 620000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "BB Bankası", MaxDebtToIncomeRatio = 0.52m, MinMonthlyIncome = 5000m, Description = "Yüksek risk toleranslı, geniş ürün gamlı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.35m, MaxLoanAmount = 170000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.15m, MaxLoanAmount = 1600000m, MaxTermMonths = 120 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.95m, MaxLoanAmount = 580000m, MaxTermMonths = 36 },
        }},
        new BankProfile { Name = "CC Bankası", MaxDebtToIncomeRatio = 0.53m, MinMonthlyIncome = 5000m, Description = "Yüksek risk toleranslı, taşıtta güçlü profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.45m, MaxLoanAmount = 160000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.25m, MaxLoanAmount = 1450000m, MaxTermMonths = 120 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 4.4m, MaxLoanAmount = 1000000m, MaxTermMonths = 48 },
        }},
        new BankProfile { Name = "DD Bankası", MaxDebtToIncomeRatio = 0.55m, MinMonthlyIncome = 4000m, Description = "En yüksek risk toleranslı, geniş kabul kriterli profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.55m, MaxLoanAmount = 150000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.35m, MaxLoanAmount = 1350000m, MaxTermMonths = 108 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 5.05m, MaxLoanAmount = 500000m, MaxTermMonths = 30 },
        }},
        new BankProfile { Name = "EE Bankası", MaxDebtToIncomeRatio = 0.55m, MinMonthlyIncome = 4000m, Description = "Yüksek risk toleranslı, düşük gelir odaklı profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.65m, MaxLoanAmount = 130000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.45m, MaxLoanAmount = 1250000m, MaxTermMonths = 108 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 5.15m, MaxLoanAmount = 450000m, MaxTermMonths = 30 },
        }},
        new BankProfile { Name = "FF Bankası", MaxDebtToIncomeRatio = 0.58m, MinMonthlyIncome = 3000m, Description = "En geniş kabul kriterli, düşük gelir dostu profil.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.75m, MaxLoanAmount = 120000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.55m, MaxLoanAmount = 1150000m, MaxTermMonths = 96 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 5.25m, MaxLoanAmount = 400000m, MaxTermMonths = 30 },
        }},
        new BankProfile { Name = "GG Bankası", MaxDebtToIncomeRatio = 0.60m, MinMonthlyIncome = 5000m, Description = "Son çare profili, en yüksek risk toleransı.", LoanTypeOffers = new List<LoanTypeOffer> {
            new LoanTypeOffer { LoanType = LoanType.Ihtiyac, MonthlyInterestRate = 5.9m, MaxLoanAmount = 110000m, MaxTermMonths = 24 },
            new LoanTypeOffer { LoanType = LoanType.Konut, MonthlyInterestRate = 4.65m, MaxLoanAmount = 1050000m, MaxTermMonths = 96 },
            new LoanTypeOffer { LoanType = LoanType.Tasit, MonthlyInterestRate = 5.35m, MaxLoanAmount = 350000m, MaxTermMonths = 24 },
        }},
    };

    public static async Task SeedAsync(MongoDbContext context)
    {
        var existingCount = await context.BankProfiles.CountDocumentsAsync(_ => true);
        if (existingCount > 0) return; // zaten veri varsa tekrar ekleme

        await context.BankProfiles.InsertManyAsync(Banks);
    }
}