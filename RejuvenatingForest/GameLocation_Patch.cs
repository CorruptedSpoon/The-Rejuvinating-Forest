using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewValley;
using HarmonyLib;

namespace RejuvenatingForest
{
    class GameLocation_Patch : HarmonyPatches
    {
        // EXAMPLE PATCH CALL
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Utilized by Harmony.")]
        private static void GameLocation_GetLocationContext_Patch()
        {
            HarmonyPatcher.ApplyPatch(
                caller: typeof(HarmonyPatches).GetMethod("GameLocation_GetLocationContext_Patch"),
                original: typeof(GameLocation).GetMethod("GetLocationContext"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod("GameLocation_GetLocationContext_Prefix"))
            );
        }

        public static bool GameLocation_GetLocationContext_Prefix(GameLocation __instance, GameLocation.LocationContext __result)
        {
            // do patchy stuff
            return true; // Temporary result to avoid throwing an error
        }
    }
}
