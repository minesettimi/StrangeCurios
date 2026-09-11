using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CuriosServer.Models;
using CuriosServer.Services;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Utils.Json.Converters;

namespace CuriosServer.Loaders;

public class InjectConstruct : IOnDIConstruct
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NewLine = "\n",
        Converters = { new StringToMongoIdConverter() }
    };
    
    public static async Task OnDIConstructAsync(IServiceCollection serviceCollection, CancellationToken cancellationToken)
    {
        CurioConfig modConfig =
            await LoadConfig<CurioConfig>(CuriosMod.ConfigPath, cancellationToken) ?? new CurioConfig();

        await File.WriteAllTextAsync(CuriosMod.ConfigPath,
            JsonSerializer.Serialize(modConfig, _serializerOptions), cancellationToken);

        serviceCollection.AddSingleton(modConfig);
    }
    
    private static async Task<T?> LoadConfig<T>(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return default;
        }
        
        await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

        return await JsonSerializer.DeserializeAsync<T>(fs, _serializerOptions, cancellationToken);
    }
}