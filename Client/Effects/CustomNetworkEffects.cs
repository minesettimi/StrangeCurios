using EFT.HealthSystem;

namespace CuriosClient.Effects;

public class CustomNetworkEffects
{
    public class Cursed : NetworkHealthController.Effect, ICursed
    {
    }

    public class Unkillable : NetworkHealthController.Effect, IUnkillable
    {
    }
}