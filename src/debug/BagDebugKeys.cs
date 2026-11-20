using Godot;
using MyFarmingGame.Autoloads;

namespace MyFarmingGame.Debugging;

/// <summary>
/// Debug builds only (added by Main when <c>OS.IsDebugBuild()</c>): playtest shortcuts for the
/// bag and wallet until real ways to get items exist (Tasks 1.5–1.9).
/// F6 = add one of every Phase-001 item · F7 = +100 coins · F8 = spend 100 coins (refused below 0).
/// Raw keys on purpose (developer tools, not game controls); never shipped in a release build.
/// </summary>
public partial class BagDebugKeys : Node
{
    public override void _UnhandledInput(InputEvent e)
    {
        if (e is not InputEventKey { Pressed: true, Echo: false } k)
            return;
        switch (k.PhysicalKeycode)
        {
            case Key.F6:
                foreach (var id in ItemIds.All)
                    Inventory.Instance?.Add(id);
                GD.Print("[debug] +1 of every item");
                break;
            case Key.F7:
                Wallet.Instance?.Earn(100);
                GD.Print($"[debug] wallet → {Wallet.Instance?.Coins}");
                break;
            case Key.F8:
                bool ok = Wallet.Instance?.Spend(100) ?? false;
                GD.Print(ok ? $"[debug] spent 100 → {Wallet.Instance?.Coins}" : "[debug] can't spend 100 (never below 0)");
                break;
            default:
                return;
        }
        GetViewport().SetInputAsHandled();
    }
}
