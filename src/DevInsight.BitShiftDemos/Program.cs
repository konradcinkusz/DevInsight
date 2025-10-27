using System;

[Flags]
public enum Features : uint
{
    None = 0,
    Bluetooth = 1u << 0,  // 000...0001
    Wifi = 1u << 1,  // 000...0010
    Gps = 1u << 2,  // 000...0100
    NoiseCancel = 1u << 3,  // 000...1000
}

public static class BitPacking
{
    // Layout (little-endian bit order shown for intuition):
    // [ 31..24 : FirmwareMajor (8 bits) ]
    // [ 23..16 : FirmwareMinor (8 bits) ]
    // [ 15..4  : Reserved (12 bits)     ]
    // [ 3..0   : Unused (4 bits)        ]
    //
    // Flags are stored separately in a uint 'flags' value using Features enum.

    public const int MajorShift = 24;
    public const int MinorShift = 16;
    public const uint ByteMask = 0xFFu;

    public static uint SetFirmware(uint word, byte major, byte minor)
    {
        // Clear the regions first, then OR in new values.
        word &= ~((ByteMask << MajorShift) | (ByteMask << MinorShift));
        word |= ((uint)major << MajorShift);
        word |= ((uint)minor << MinorShift);
        return word;
    }

    public static (byte Major, byte Minor) GetFirmware(uint word)
        => ((byte)((word >> MajorShift) & ByteMask),
            (byte)((word >> MinorShift) & ByteMask));

    public static uint EnableFeatures(uint flags, Features toEnable)
        => flags | (uint)toEnable;

    public static uint DisableFeatures(uint flags, Features toDisable)
        => flags & ~(uint)toDisable;

    public static bool HasFeature(uint flags, Features f)
        => (flags & (uint)f) != 0;

    public static uint ToggleFeatures(uint flags, Features toToggle)
        => flags ^ (uint)toToggle;
}

public class Demo
{
    public static void Main()
    {
        uint flags = 0;
        flags = BitPacking.EnableFeatures(flags, Features.Bluetooth | Features.Gps);
        Console.WriteLine($"Has BT? {BitPacking.HasFeature(flags, Features.Bluetooth)}"); // true
        Console.WriteLine($"Has WiFi? {BitPacking.HasFeature(flags, Features.Wifi)}");   // false

        flags = BitPacking.ToggleFeatures(flags, Features.Gps);  // flips GPS bit
        Console.WriteLine($"Has GPS? {BitPacking.HasFeature(flags, Features.Gps)}");     // false

        uint word = 0;
        word = BitPacking.SetFirmware(word, major: 9, minor: 42);
        var fw = BitPacking.GetFirmware(word);
        Console.WriteLine($"FW v{fw.Major}.{fw.Minor}"); // v9.42
    }
}
