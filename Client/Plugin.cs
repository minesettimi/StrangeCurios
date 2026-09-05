using BepInEx;
using BepInEx.Logging;
using SPT.Reflection.Patching;

namespace CuriosClient;

[BepInPlugin("com.minesettimi.curios", "Strange Curios", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource PluginLogger = null!;

    private PatchManager _patchManager = null!;

    private void Awake()
    {
        _patchManager = new PatchManager(this, true);
        _patchManager.EnablePatches();

        PluginLogger = Logger;
    }
}