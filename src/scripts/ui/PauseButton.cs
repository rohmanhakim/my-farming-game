using Godot;
using MyFarmingGame.Core;

namespace MyFarmingGame.UI;

/// <summary>
/// Opens the pause menu (Task 1.11): the <c>pause</c> action (Start on any gamepad, Esc) or this
/// on-screen button (mouse). Lives in the HUD, so it only runs while the game is not paused; menus
/// that close on Esc (bag, bubble, board, shop) handle Esc first.
/// </summary>
public partial class PauseButton : Button
{
    public override void _Ready()
    {
        Icon = SpriteLibrary.Get("icon_pause");
        ExpandIcon = true;
        CustomMinimumSize = new Vector2(56, 56);
        FocusMode = FocusModeEnum.None;
        TooltipText = "Pause";
        Pressed += OpenMenu;
    }

    private void OpenMenu() => PauseMenuView.Find(GetTree())?.Open();

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("pause") && !e.IsEcho())
        {
            var menu = PauseMenuView.Find(GetTree());
            if (menu != null && menu.CanOpen && (Autoloads.GameClock.Instance?.IsPlayable ?? true))
            {
                menu.Open();
                GetViewport().SetInputAsHandled();
            }
        }
    }
}
