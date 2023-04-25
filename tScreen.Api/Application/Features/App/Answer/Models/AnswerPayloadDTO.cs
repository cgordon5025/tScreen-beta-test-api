namespace Application.Features.App.Answer.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class AnswerPayloadDTO<TData>
{
    public TData? Data { get; set; }
    public bool Skipped { get; set; } = false;
}