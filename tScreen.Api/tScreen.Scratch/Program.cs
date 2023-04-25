// See https://aka.ms/new-console-template for more information

using Application.Features.App.Answer.Models;
using Core;

var data = File.ReadAllText("/Users/resetvector/Library/Application Support/JetBrains/Rider2022.3/scratches/data.json");

var answerPayloadDTO =
    Utility.DeserializeObject<AnswerPayloadDTO<IReadOnlyList<AnswerDataParentMood>>>(data);

var characteristicsCollection = answerPayloadDTO
    ?.Data?.Select(guardian => guardian.Characteristics)
    .Where(characteristics => !answerPayloadDTO.Skipped && characteristics.Any())
                                ?? Enumerable.Empty<List<string>>();

foreach (var characteristics in characteristicsCollection)
{
    Console.WriteLine("-----");
    foreach (var characteristic in characteristics)
    {
        Console.WriteLine(characteristic);
    }
}


var defaultList = new []{ 
    "Angry",
    "Anxious",
    "Irritable",
    "Moody",
    "Overwhelmed",
    "Sad",
    "Tired" 
};

var feProtectiveCount = defaultList.Except(new[] { "Angry", "Sad" }).Count();
Console.WriteLine("FE-Protective: " + feProtectiveCount);

Console.WriteLine(string.Join(",", defaultList.Except(new [] { "Boo", "Sad", "Tired", "Relaxed" })));