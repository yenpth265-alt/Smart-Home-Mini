/****************/
/* MSSV: 202419124
 * Họ tên: Phan Thị Hải Yến
 */
/****************/
using System.Text;
namespace SmartHomeMini;

public static class Program
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Contains("--test")) return SelfTests.Run();
        IDevice light = DeviceFactory.Create("light", "Đèn phòng ngủ");
        IDevice fan = DeviceFactory.Create("fan", "Quạt phòng ngủ");
        IDevice door = DeviceFactory.Create("door", "Cửa chính");
        var facade = new SmartHomeFacade(light, fan, (ILockable)door);
        if (args.Contains("--demo"))
        {
            Console.WriteLine("SMART HOME MINI — Phan Thị Hải Yến — 202419124");
            light.TurnOn(); fan.TurnOn(); ((ILockable)door).Unlock();
            Console.WriteLine("[Đi làm]"); facade.GoToWork();
            Console.WriteLine("[Đi ngủ]"); facade.GoToSleep();
            return 0;
        }
        var devices = new List<IDevice> { light, fan, door };
        while (true)
        {
            Console.WriteLine($"\n{HomeConfiguration.Instance.HomeName}");
            Console.WriteLine("1. Đi làm  2. Đi ngủ  3. Tạo thiết bị  4. Điều khiển thiết bị  5. Mức sáng đêm  0. Thoát");
            string? choice = Console.ReadLine();
            if (choice is null or "0") return 0;
            try
            {
                switch (choice)
                {
                    case "1": facade.GoToWork(); break;
                    case "2": facade.GoToSleep(); break;
                    case "3":
                        Console.Write("Loại (light/fan/door): "); string type = Console.ReadLine() ?? "";
                        Console.Write("Tên: "); string name = Console.ReadLine() ?? "";
                        devices.Add(DeviceFactory.Create(type, name));
                        Console.WriteLine($"Đã tạo thiết bị số {devices.Count}. Thiết bị thêm được điều khiển riêng."); break;
                    case "4":
                        Console.WriteLine($"Chọn số 1..{devices.Count} (1: đèn, 2: quạt, 3: cửa; tiếp theo là thiết bị đã thêm):");
                        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > devices.Count)
                            throw new ArgumentException("Số thiết bị không hợp lệ.");
                        IDevice device = devices[index - 1];
                        Console.Write("Lệnh (on/off/lock/unlock): ");
                        switch (Console.ReadLine()?.Trim().ToLowerInvariant())
                        {
                            case "on": device.TurnOn(); break;
                            case "off": device.TurnOff(); break;
                            case "lock" when device is ILockable locked: locked.Lock(); break;
                            case "unlock" when device is ILockable unlocked: unlocked.Unlock(); break;
                            default: throw new ArgumentException("Lệnh không hợp lệ hoặc thiết bị không hỗ trợ khóa.");
                        }
                        break;
                    case "5":
                        Console.Write("Mức sáng (0..100): ");
                        if (!int.TryParse(Console.ReadLine(), out int level)) throw new ArgumentException("Hãy nhập số nguyên.");
                        HomeConfiguration.Instance.NightLightLevel = level; break;
                    default: Console.WriteLine("Lựa chọn không hợp lệ."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine($"Lỗi: {ex.Message}"); }
        }
    }
}
