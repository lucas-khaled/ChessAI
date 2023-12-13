using System.ComponentModel;

public enum DrawType
{
    None,
    [Description("stalemate")]
    Stalemate,
    [Description("dead position")]
    Deadposition,
    [Description("repetition")]
    Repetition,
    [Description("mutual agreement")]
    Agreement,
    [Description("the rule of 50 moves")]
    RuleOf50Moves
}

public static class DrawTypeExtensions
{
    public static string ToDescriptionString(this DrawType val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}
