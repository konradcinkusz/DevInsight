/// <summary>
/// A uint is 32 bits. In C#, if you shift by >= 32, 
/// the shift count is masked with 0x1F, 
/// so it wraps modulo 32. For example, 
/// 1u << 32 is equivalent to 1u << 0. 
/// That means attempting to define bit flags beyond bit 31 causes collisions. 
/// If more bits are required, ulong should be used instead.
/// effectiveShift = N & 0x1F = N & 31(decimal)
/// 0x1F(hex) = 31(decimal) = 00011111(binary)
/// 
/// uint is 32 bits. In C#, the shift count is masked with 0x1F, 
/// so shifting by >=32 does not fail—it wraps. 1u << 32 is the same as 1u << 0, 
/// 1u << 33 equals 1u << 1, etc. This can silently cause enum value collisions 
/// when people try to shift beyond bit 31. If you need more flags, 
/// use ulong instead and continue safely to bit 63.
/// </summary>
[Flags]
public enum Features : uint //32 bit
{
    None = 0,
    Bluetooth = 1u << 0,  // 000...0001
    Wifi = 1u << 1,  // 000...0010
    Gps = 1u << 2,  // 000...0100
    NoiseCancel = 1u << 3,  // 000...1000

    // Continuing safely
    MicBoost = 1u << 4,  // 0x00000010
    VoiceAssist = 1u << 5,  // 0x00000020
    HeartRate = 1u << 6,  // 0x00000040
    AmbientMode = 1u << 7,  // 0x00000080

    // Let's go near the top
    DebugStream = 1u << 30, // valid
    DeepSleep = 1u << 31, // highest legal bit
    // ------------------------------
    // >>> Now we go PAST 31 <<<<
    // ------------------------------
    Bit32 = 1u << 32, // SAME AS (1u << 0) = 1
    Bit33 = 1u << 33, // SAME AS (1u << 1) = 2
    Bit63 = 1u << 63  // SAME AS (1u << 31) = 0x80000000
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
        Console.WriteLine($"{Features.Bluetooth} → {(uint)Features.Bluetooth:X}");
        Console.WriteLine($"{Features.Bit32} → {(uint)Features.Bit32:X}");
        Console.WriteLine($"{Features.Bit33} → {(uint)Features.Bit33:X}");
        Console.WriteLine($"{Features.Bit63} → {(uint)Features.Bit63:X}");

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
