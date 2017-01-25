
#region >>>> Usings

using System;

#endregion

/// <summary>
/// This class contains all the data that we want to share between various bits of code.
/// </summary>
/// <remarks>
/// It's a very simplistic way of sharing data, but it's good enough for the purposes of our tutorial.
/// </remarks>
public static class SharedData
{

    public static DateTime Time { get; set; }

    public static float UpkeepRate { get; set; }
    public static float IncomeRate { get; set; }
    public static float TotalGold { get; set; }

}

