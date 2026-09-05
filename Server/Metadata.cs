using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace CuriosServer;

public static class Metadata
{
    public record ModMetadata : IModMetadata
    {
        public string ModGuid { get; init; } = "com.minesettimi.curios";
        public string Name { get; init; } = "Powerful Curios and Trinkers";
        public string Author { get; init; } = "minesettimi";
        public List<string>? Contributors { get; init; }

        public Version Version { get; init; } = new(1, 0, 0);
        public Range SptVersion { get; init; } = new("~4.1.5");

        public bool HasPrepatcher { get; init; } = false;
        public List<string>? Incompatibilities { get; init; }
        public Dictionary<string, Range>? ModDependencies { get; init; } = new()
        {
            { "com.wtt.commonlib", new Range("^3.0.6") }
        };

        public string? Url { get; init; } = "https://github.com/minesettimi/TushonkaTerritories";
        public string License { get; init; } = "MIT";
    }
}