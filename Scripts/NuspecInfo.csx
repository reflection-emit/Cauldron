using System;

public class NuspecInfo
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Version Version { get; set; }
    public bool IsBeta { get; set; }
}