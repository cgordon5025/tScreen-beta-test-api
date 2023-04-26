using System;

namespace Application.Common.Models;

public class HistoryCheckpointData
{
    public string Label { get; set; } = null!;
    public DateTime Value { get; set; }
}