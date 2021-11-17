using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace RejuvenatingForest
{
    class HoeDirt_Patch : HarmonyPatches
    {
        // EXAMPLE PATCH CALL
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Utilized by Harmony.")]
        private static void HoeDirt_plant_Patch()
        {
            HarmonyPatcher.ApplyPatch(
                caller: typeof(HoeDirt_Patch).GetMethod("plant_Prefix"),
                original: typeof(HoeDirt).GetMethod("plant"),
                prefix: new HarmonyMethod(typeof(HoeDirt_Patch).GetMethod("plant_Prefix"))
            );
        }

        public static void plant_Prefix(HoeDirt __instance, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location)
        {
            Globals.Monitor.Log("Applying plant prefix", LogLevel.Debug);

            // do patchy stuff

            // TODO: Implement Magic Fertilizer as an overnight fertilizer
            // Quality retaining soil only
            if (index == 371)
            {
                __instance.crop.growCompletely();
            }
        }
    }
}
