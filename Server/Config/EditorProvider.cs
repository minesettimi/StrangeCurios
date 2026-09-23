using CuriosServer.Models;
using CuriosServer.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Servers.Ws;
using SPTarkov.Server.Web.Models.Configs;
using SPTarkov.Server.Web.Services;

namespace CuriosServer.Loaders;

[Injectable(InjectionType.Singleton)]
public class EditorProvider(CurioConfig curioConfig) : IConfigEditorConfigProvider
{
    public IEnumerable<ConfigEditorConfigRegistration> GetConfigs()
    {
        yield return ConfigEditorConfigRegistration.Create(
            "com.minesettimi.curios",
            "Curios Config",
            curioConfig,
            CuriosMod.ConfigPath);
    }
}