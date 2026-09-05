using System;
using System.Threading.Tasks;
using CuriosClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace CuriosClient.Services;

public static class TemplateService
{
    public static CuriosTemplate TemplateData;
    
    public static async Task<bool> RequestData()
    {
        try
        {
            string? data = await RequestHandler.GetJsonAsync("/pc/templates");

            if (data != null)
            {
                TemplateData = JsonConvert.DeserializeObject<CuriosTemplate>(data)!;

                return true;
            }
        }
        catch (Exception e)
        {
            Plugin.PluginLogger.LogError($"Failed to get templates from server with error: {e.Message}");
            throw;
        }

        return false;
    }
}