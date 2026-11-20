using Godot;

namespace MyFarmingGame.Autoloads;

/// <summary>
/// Kit's coins (Task 1.4, inventory.md §5, economy.md): shown on the HUD and in the bag. Never
/// below zero: no debt, so <see cref="Spend"/> refuses (and spends nothing) when Kit can't afford it.
/// Shared by both players in co-play.
/// </summary>
public partial class Wallet : Node
{
    public static Wallet Instance { get; private set; }

    /// <summary>Starting money on a new game (economy.md: 500 coins ≈ A$50).</summary>
    public const int StartingCoins = 500;

    /// <summary>Emitted whenever the amount changes. Argument is the new amount.</summary>
    [Signal] public delegate void CoinsChangedEventHandler(int coins);

    public int Coins { get; private set; } = StartingCoins;

    public override void _EnterTree()
    {
        if (Instance == null && GetParent() == GetTree().Root)
            Instance = this;
        ProcessMode = ProcessModeEnum.Pausable; // project-structure.md §4 process-mode table
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool CanAfford(int amount) => amount >= 0 && Coins >= amount;

    /// <summary>Adds coins. Zero or negative amounts are ignored.</summary>
    public void Earn(int amount)
    {
        if (amount <= 0)
            return;
        Coins = (int)System.Math.Min((long)Coins + amount, int.MaxValue);
        EmitSignal(SignalName.CoinsChanged, Coins);
    }

    /// <summary>Spends coins if Kit has enough. Returns false and spends nothing otherwise, so the
    /// wallet can never go below zero. Negative amounts are refused.</summary>
    public bool Spend(int amount)
    {
        if (amount < 0 || amount > Coins)
            return false;
        if (amount == 0)
            return true;
        Coins -= amount;
        EmitSignal(SignalName.CoinsChanged, Coins);
        return true;
    }

    /// <summary>Sets the amount directly (save/load, Task 1.10). Clamped at zero.</summary>
    public void SetCoins(int amount)
    {
        Coins = Mathf.Max(0, amount);
        EmitSignal(SignalName.CoinsChanged, Coins);
    }

    public void ResetForNewGame() => SetCoins(StartingCoins);
}
