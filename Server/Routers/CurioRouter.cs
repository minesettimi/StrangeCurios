using System.Text.Json;
using CuriosServer.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace CuriosServer.Routers;

[Injectable(TypePriority = OnLoadOrder.Routers + 5)]
public class CurioRouter(JsonUtil jsonUtil, HttpResponseUtil httpResponseUtil, CurioConfig curioConfig) : StaticRouter(jsonUtil, [
    new RouteAction<EmptyRequestData>(
        "/curio/config",
        async (url,
            info,
            sessionId,
            output,
            cancellationToken) => 
            await new ValueTask<string>(httpResponseUtil.NoBody(JsonSerializer.Serialize(curioConfig))))
]);