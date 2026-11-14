/****************/
/* MSSV: 202419124
 * Họ tên: Phan Thị Hải Yến
 */
/****************/
namespace SmartHomeMini;

/// <summary>Simple Factory tập trung quyết định chọn lớp cụ thể.</summary>
public static class DeviceFactory
{
    public static IDevice Create(string type, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên thiết bị không được trống.");
        return type?.Trim().ToLowerInvariant() switch
        {
            "light" => new SmartLight(name.Trim()),
            "fan" => new SmartFan(name.Trim()),
            "door" => new SmartDoor(name.Trim()),
            _ => throw new ArgumentException($"Unsupported device type: {type}")
        };
    }
}

/// <summary>Lazy bảo đảm khởi tạo duy nhất, an toàn khi nhiều luồng truy cập.</summary>
public sealed class HomeConfiguration
{
    private static readonly Lazy<HomeConfiguration> lazy = new(() => new HomeConfiguration());
    public static HomeConfiguration Instance => lazy.Value;
    private HomeConfiguration() { }
    public string HomeName { get; set; } = "Nhà của Hải Yến";
    private int nightLightLevel = 30;
    public int NightLightLevel
    {
        get => nightLightLevel;
        set
        {
            if (value is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(value), "Mức sáng phải từ 0 đến 100.");
            nightLightLevel = value;
        }
    }
}

/// <summary>Điều phối thiết bị qua interface, nhận phụ thuộc qua constructor.</summary>
public sealed class SmartHomeFacade
{
    private readonly IDevice light;
    private readonly IDevice fan;
    private readonly ILockable door;
    public SmartHomeFacade(IDevice light, IDevice fan, ILockable door)
    {
        this.light = light ?? throw new ArgumentNullException(nameof(light));
        this.fan = fan ?? throw new ArgumentNullException(nameof(fan));
        this.door = door ?? throw new ArgumentNullException(nameof(door));
    }
    public void GoToWork()
    {
        light.TurnOff(); fan.TurnOff(); door.Lock();
    }
    public void GoToSleep()
    {
        fan.TurnOff(); door.Lock(); light.TurnOn();
        Console.WriteLine($"Night light level: {HomeConfiguration.Instance.NightLightLevel}%");
    }
}
