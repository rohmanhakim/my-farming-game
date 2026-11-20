namespace MyFarmingGame.Core;

/// <summary>
/// CanvasLayer numbers for Main's UI stack (project-structure.md §4, scene-architecture standard).
/// Never hand-type a layer number anywhere else; World draws at the default layer 0.
/// </summary>
public static class UILayers
{
    public const int Hud = 10;        // clock dial, tool belt, bag, picture bubbles, nudges
    public const int PauseMenu = 20;  // pause menu
    public const int Transition = 30; // day fades, scene wipes
    public const int Debug = 40;      // debug overlays
}
