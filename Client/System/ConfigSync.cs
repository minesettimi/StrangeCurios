using CuriosClient.Models;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace CuriosClient.System;

public static class ConfigSync
{
    public static CurioConfig GetConfig()
    {
        string result = RequestHandler.GetJson("/curio/config");
        return JsonConvert.DeserializeObject<CurioConfig>(result) ?? new CurioConfig();
    }
}