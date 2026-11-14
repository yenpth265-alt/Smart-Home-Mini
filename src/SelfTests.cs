/****************/
/* MSSV: 202419124
 * Họ tên: Phan Thị Hải Yến
 */
/****************/
namespace SmartHomeMini;

/// <summary>Kiểm thử tự động không cần gói NuGet; thất bại trả exit code 1.</summary>
public static class SelfTests
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
    private static void Reject(Action action)
    {
        try { action(); } catch (ArgumentException) { return; }
        throw new Exception("Thiếu ArgumentException");
    }
    public static int Run()
    {
        int failed = 0;
        var tests = new (string, Action)[]
        {
            ("T1 Factory trả đúng ba loại", () => {
                Check(DeviceFactory.Create("light", "L") is SmartLight, "light");
                Check(DeviceFactory.Create("fan", "F") is SmartFan, "fan");
                Check(DeviceFactory.Create("door", "D") is SmartDoor, "door"); }),
            ("T2 Từ chối camera", () => Reject(() => DeviceFactory.Create("camera", "C"))),
            ("T3 Singleton cùng tham chiếu", () => Check(ReferenceEquals(HomeConfiguration.Instance, HomeConfiguration.Instance), "Singleton")),
            ("T4 Đi làm", () => {
                var l = (SmartLight)DeviceFactory.Create("light", "L");
                var f = (SmartFan)DeviceFactory.Create("fan", "F");
                var d = (SmartDoor)DeviceFactory.Create("door", "D");
                l.TurnOn(); f.TurnOn(); d.Unlock();
                new SmartHomeFacade(l, f, d).GoToWork();
                Check(!l.IsOn && !f.IsOn && d.IsLocked, "Sai trạng thái đi làm"); }),
            ("T5 Đi ngủ và mức sáng cấu hình", () => {
                var l = (SmartLight)DeviceFactory.Create("light", "L");
                var f = (SmartFan)DeviceFactory.Create("fan", "F");
                var d = (SmartDoor)DeviceFactory.Create("door", "D");
                f.TurnOn(); d.Unlock();
                int old = HomeConfiguration.Instance.NightLightLevel;
                TextWriter output = Console.Out;
                using var captured = new StringWriter();
                try {
                    HomeConfiguration.Instance.NightLightLevel = 47;
                    Console.SetOut(captured);
                    new SmartHomeFacade(l, f, d).GoToSleep();
                } finally { Console.SetOut(output); HomeConfiguration.Instance.NightLightLevel = old; }
                Check(l.IsOn && !f.IsOn && d.IsLocked, "Sai trạng thái đi ngủ");
                Check(captured.ToString().Contains("Night light level: 47%"), "Không đọc cấu hình"); }),
            ("T6 Chuẩn hóa loại", () => Check(DeviceFactory.Create(" LiGhT ", "L") is SmartLight, "Chuẩn hóa")),
            ("T7 Dữ liệu không hợp lệ", () => {
                Reject(() => DeviceFactory.Create(null!, "L"));
                Reject(() => DeviceFactory.Create("light", " "));
                Reject(() => HomeConfiguration.Instance.NightLightLevel = -1);
                Reject(() => HomeConfiguration.Instance.NightLightLevel = 101); }),
            ("T8 Cửa bật/tắt, khóa/mở độc lập", () => {
                var d = (SmartDoor)DeviceFactory.Create("door", "D");
                d.Lock(); d.TurnOn(); d.TurnOff(); Check(d.IsLocked && !d.IsOn, "Nguồn và khóa");
                d.Unlock(); Check(!d.IsLocked, "Mở khóa"); })
        };
        foreach (var (name, test) in tests)
        {
            try { test(); Console.WriteLine($"PASS {name}"); }
            catch (Exception ex) { failed++; Console.WriteLine($"FAIL {name}: {ex.Message}"); }
        }
        Console.WriteLine($"Kết quả: {tests.Length - failed}/{tests.Length} PASS");
        return failed == 0 ? 0 : 1;
    }
}
