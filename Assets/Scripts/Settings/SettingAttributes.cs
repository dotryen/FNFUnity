using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class SettingAttributes : Attribute {
    public string name = string.Empty;
    public string description = string.Empty;
    public int min = int.MinValue;
    public int max = int.MaxValue;
}
