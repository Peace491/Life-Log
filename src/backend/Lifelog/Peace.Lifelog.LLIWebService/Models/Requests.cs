﻿namespace Peace.Lifelog.LLIWebService;

using Peace.Lifelog.LLI;

public class PostLLIRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? Categories { get; set; }
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public string RecurrenceStatus { get; set; } = "On";
    public string RecurrenceFrequency { get; set; } = "None";
}

public class PutLLIRequest
{
    public string LLIID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? Categories { get; set; } = null;
    public string Status { get; set; } = LLIStatus.Active;
    public string Visibility { get; set; } = LLIVisibility.Public;
    public string Deadline { get; set; } = string.Empty;
    public int? Cost { get; set; } = null;
    public string RecurrenceStatus { get; set; } = "On";
    public string RecurrenceFrequency { get; set; } = "None";
}