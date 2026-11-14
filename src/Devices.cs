/****************/
/* MSSV: 202419124
 * Họ tên: Phan Thị Hải Yến
 */
/****************/
namespace SmartHomeMini;

/// <summary>Hành vi chung, không buộc đèn hoặc quạt hỗ trợ khóa.</summary>
public interface IDevice
{
    void TurnOn();
    void TurnOff();
}

/// <summary>Khả năng khóa độc lập với nguồn điện của thiết bị.</summary>
public interface ILockable
{
    void Lock();
    void Unlock();
}

public sealed class SmartLight(string name) : IDevice
{
    public bool IsOn { get; private set; }
    public void TurnOn() { IsOn = true; Console.WriteLine($"{name}: ON"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"{name}: OFF"); }
}

public sealed class SmartFan(string name) : IDevice
{
    public bool IsOn { get; private set; }
    public void TurnOn() { IsOn = true; Console.WriteLine($"{name}: ON"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"{name}: OFF"); }
}

public sealed class SmartDoor(string name) : IDevice, ILockable
{
    public bool IsOn { get; private set; }
    public bool IsLocked { get; private set; }
    // Bật/tắt bộ điều khiển không tự thay đổi trạng thái khóa.
    public void TurnOn() { IsOn = true; Console.WriteLine($"{name}: ON"); }
    public void TurnOff() { IsOn = false; Console.WriteLine($"{name}: OFF"); }
    public void Lock() { IsLocked = true; Console.WriteLine($"{name}: LOCKED"); }
    public void Unlock() { IsLocked = false; Console.WriteLine($"{name}: UNLOCKED"); }
}
