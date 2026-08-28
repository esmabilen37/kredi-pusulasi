using Microsoft.ML;
using FinansalAsistan;

var mlContext = new MLContext(seed:1);
string dataPath = Path.Combine(AppContext.BaseDirectory, "training_data.csv");
IDataView fullData=mlContext.Data.LoadFromTextFile<TokenData>(dataPath, hasHeader: true, separatorChar: ',');

var split = mlContext.Data.TrainTestSplit(fullData, testFraction: 0.2);
var pipeline = mlContext.Transforms.Conversion
    .MapValueToKey("Label")
    .Append(mlContext.Transforms.Text.FeaturizeText("TokenFeaturized", nameof(TokenData.Token)))
    .Append(mlContext.Transforms.Text.FeaturizeText("PrevPrevTokenFeaturized", nameof(TokenData.PrevPrev)))
    .Append(mlContext.Transforms.Text.FeaturizeText("PrevTokenFeaturized", nameof(TokenData.Prev)))

    .Append(mlContext.Transforms.Text.FeaturizeText("NextTokenFeaturized", nameof(TokenData.Next)))
    .Append(mlContext.Transforms.Text.FeaturizeText("NextNextTokenFeaturized", nameof(TokenData.NextNext)))
    .Append(mlContext.Transforms.Conversion.ConvertType("IsNumericFloat", nameof(TokenData.IsNumeric),outputKind:Microsoft.ML.Data.DataKind.Single))
    .Append(mlContext.Transforms.Concatenate("Features", "TokenFeaturized","PrevPrevTokenFeaturized", "PrevTokenFeaturized", "NextNextTokenFeaturized", "NextTokenFeaturized","IsNumericFloat"));

var fullPipeline = pipeline
    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
    Console.WriteLine("model eğitiliyor..");
    var model= fullPipeline.Fit(split.TrainSet);
    mlContext.Model.Save(model, split.TrainSet.Schema, "model.zip");
    Console.WriteLine("Model kaydedildi: model.zip");
    Console.WriteLine("eğitim tamamlandı");
var predictions= model.Transform(split.TestSet);
var metrics=mlContext.MulticlassClassification.Evaluate(predictions);
Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:P2}");
Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:P2}");

var predictionEngine= mlContext.Model.CreatePredictionEngine<TokenData, TokenPrediction>(model);
var testSentences = new List<string>
{
    "35000 lira gideri olan biri 250000 lira konut kredisi istiyor 60 aylık",
    "Aylık 70000 TL kazanç sağlıyorum yaklaşık 300000 TL istiyorum 36 ay vadeli olsun",
    "12 ay vade ile 50000 TL kredi istiyorum gelirim 22000 TL giderim ise 9000 TL",
    "Toplam gelirim 65000 TL 3 aylık kısa vadeli 40000 TL ihtiyaç kredisi kullanmak istiyorum"
};
foreach (var sentence in testSentences)
{
    Console.WriteLine($"test cümlesi: {sentence}");
    var tokens=Tokenizer.TokenizeForPrediction(sentence);
    foreach (var token in tokens )
    {
        var result=predictionEngine.Predict(token);
        Console.WriteLine($"{token.Token,-15}-> {result.Label}");
    }
}



