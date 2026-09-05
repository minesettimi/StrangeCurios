using System;
using BepInEx;
using BepInEx.Logging;
using CuriosClient.Models;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace CuriosClient;

[BepInPlugin("com.minesettimi.curios", "Powerful Curios and Trinkets", "1.0.0")]
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

    private void Start()
    {
        JsonTypes.TypeTable.Add("6a9b52c15dfaf97d11fc341e", typeof(Curios));
        JsonTypes.TemplateTypeTable.Add("6a9b52c15dfaf97d11fc341e", typeof(CuriosTemplate));
        JsonTypes.ItemConstructors.Add("6a9b52c15dfaf97d11fc341e", (id, template) => new Curios(id, (CuriosTemplate)template));
    }
}