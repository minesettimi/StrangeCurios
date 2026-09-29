using BepInEx;
using Fika.Core.Networking;

namespace StrangeCuriosFika;

[BepInPlugin("com.minesettimi.curiosfika", "Strange Curios Fika", "1.0.0")]
[BepInDependency("com.minesettimi.curios", "1.1.0")]
[BepInDependency("com.fika.core", "2.4.2")]
public class FikaPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        EFTSerializationExtensions.RegisterPolymorphicType(CurioSerialization.PutCurioDescriptor, CurioSerialization.ReadCurioDescriptor);
    }
}