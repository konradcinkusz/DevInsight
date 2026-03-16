namespace DevInsight.CSharpSamples;

internal class Program
{
    internal static readonly TimeZoneInfo London = ResolveLondon();

    private static TimeZoneInfo ResolveLondon()
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById("Europe/London", 
                                                out TimeZoneInfo? tz))
            return tz;

        return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
    }

    static void Main(string[] args)
    {
        DateTime DetermineTradingDate(DateTime londonLocalTime) =>
        londonLocalTime.Hour >= 23
            ? londonLocalTime.Date.AddDays(1)
            : londonLocalTime.Date;


        var UTC_Now = DateTime.UtcNow;
        DateTime windowLocal23_00 = new DateTime(
            UTC_Now.Year, UTC_Now.Month, UTC_Now.Day,
            23, 0, 0, DateTimeKind.Unspecified);
        DateTime windowLocal23_30 = new DateTime(
            UTC_Now.Year, UTC_Now.Month, UTC_Now.Day,
            23, 30, 0, DateTimeKind.Unspecified);

        // Static UTC dates for common UK DST transitions (last Sundays in March and October)
        // Use these fixed values when you want deterministic tests across summer/winter shifts.
        DateTime dstStart2024 = new DateTime(2024, 3, 31, 1, 0, 0, DateTimeKind.Unspecified);
        DateTime dstEnd2024 = new DateTime(2024, 10, 27, 1, 0, 0, DateTimeKind.Unspecified);
        DateTime dstStart2025 = new DateTime(2025, 3, 30, 1, 0, 0, DateTimeKind.Unspecified);
        DateTime dstEnd2025 = new DateTime(2025, 10, 26, 1, 0, 0, DateTimeKind.Unspecified);

        DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(windowLocal23_00, London);
        DateTime localNow2 = TimeZoneInfo.ConvertTimeFromUtc(windowLocal23_00, London);

        DateTime tradingDate = DetermineTradingDate(localNow);
        DateTime tradingDate2 = DetermineTradingDate(localNow2);

        Console.WriteLine("Hello, World!");
    }
}