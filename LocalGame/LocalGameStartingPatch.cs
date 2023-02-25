﻿using SIT.Tarkov.Core.AI;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SIT.Tarkov.Core.LocalGame
{
    public class LocalGameStartingPatch : ModulePatch
    {
        public static object LocalGameInstance;
        public static event Action LocalGameStarted;
        protected override MethodBase GetTargetMethod()
        {
            var t = typeof(EFT.LocalGame);
            if (t == null)
                Logger.LogInfo($"LocalGameStartingPatch:Type is NULL");

            var method = PatchConstants.GetAllMethodsForType(t)
                .FirstOrDefault(x => x.GetParameters().Length >= 3
                && x.GetParameters()[0].Name.Contains("botsSettings")
                && x.GetParameters()[1].Name.Contains("entryPoint")
                && x.GetParameters()[2].Name.Contains("backendUrl")
                );

            Logger.LogInfo($"{ThisTypeName}:{t.Name}:{method.Name}");
            return method;
        }

        [PatchPrefix]
        public static async void PatchPrefix(
            object __instance
            , Task __result
            )
        {
            await __result;

            Logger.LogInfo($"LocalGameStartingPatch:PatchPrefix");
            LocalGameInstance = __instance;
            if (LocalGameStarted != null)
                LocalGameStarted();

            BotSystemHelpers.BotControllerInstance = PatchConstants.GetFieldOrPropertyFromInstance<object>(__instance, BotSystemHelpers.BotControllerType.Name.ToLower() + "_0", false);
            Logger.LogInfo($"LocalGameStartingPatch:BotSystemInstance:" + BotSystemHelpers.BotControllerInstance.GetType().Name);
        }
    }
}
