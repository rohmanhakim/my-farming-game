using Godot;
using MyFarmingGame.Autoloads;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>HUD coin readout (Task 1.4, inventory.md §5): coin icon + amount, top-left. Pure view;
/// state lives in <see cref="Wallet"/>.</summary>
public partial class WalletView : Control
{
    /// <summary>Optional wallet (tests); defaults to the Wallet autoload.</summary>
    public Wallet Purse { get; set; }

    private Label _amount;

    public string ShownText => _amount?.Text ?? "";

    public override void _Ready()
    {
        _amount = GetNode<Label>("%Amount");
        GetNode<TextureRect>("%CoinIcon").Texture = SpriteLibrary.Get("coin");
        Purse ??= Wallet.Instance;
        if (Purse != null) Purse.CoinsChanged += OnCoinsChanged;
        OnCoinsChanged(Purse?.Coins ?? 0);
    }

    public override void _ExitTree()
    {
        if (Purse != null && IsInstanceValid(Purse)) Purse.CoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int coins) => _amount.Text = coins.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
}
