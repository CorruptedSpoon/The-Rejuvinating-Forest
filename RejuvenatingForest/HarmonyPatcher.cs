using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Reflection;

using StardewValley;

namespace RejuvenatingForest
{
	class HarmonyPatcher
	{
		// NOTE: All Monitor code is experimental and may not be correct

		private static Harmony harmony;
		private static IMonitor Monitor; // ref to ModEntry.cs monitor
		
		static HarmonyPatcher()
		{
			harmony = new("manifest.json");
		}

		internal static void ApplyPatches(IMonitor monitor)
		{
			Monitor = monitor;

			MethodInfo[] patchMethods = 
				typeof(HarmonyPatcher)
					.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(m => m.Name.Contains("_Patch")).ToArray();

			foreach (MethodInfo patchMethod in patchMethods)
			{
				patchMethod.Invoke(null, null);
			}
		}

		internal static void ApplyPatch(MethodInfo caller, MethodInfo original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			if (original is null)
			{
				Monitor.Log($"Aborting {caller.Name}: Method to patch cannot be null.", LogLevel.Error);
			}

			if (prefix is null && postfix is null && transpiler is null)
			{
				Monitor.Log($"Aborting {caller.Name}: At least one valid patch method must be specified.", LogLevel.Error);
			}

			try
			{
				harmony.Patch(
					original: original,
					prefix: prefix,
					postfix: postfix,
					transpiler: transpiler
				);

				Monitor.Log($"Successfully patched {original.DeclaringType}::{original.Name}.");
			}
			catch (Exception ex)
			{
				Monitor.Log($"Exception encountered while patching {original.DeclaringType}::{original.Name}: {ex}", LogLevel.Error);
				Monitor.Log($"Aborting {caller.Name}.", LogLevel.Error);
			}
		}
	}
}