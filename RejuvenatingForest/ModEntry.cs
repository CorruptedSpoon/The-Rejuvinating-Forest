using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;
using xTile;
using xTile.ObjectModel;

// HARMONY
using HarmonyLib;

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod//, IAssetEditor, //IAssetLoader // sorry for the merge conflict nick :c
    {
        #region Entry method
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Globals.Helper = this.Helper; // Helper can be referenced by Globals.Helper
            Globals.Monitor = this.Monitor; // Monitor can be referenced by Globals.Logger.Log(...)
            Globals.Manifest = this.ModManifest; // Manifest can be referenced by Globals.Manifest

            helper.Events.GameLoop.DayStarted += this.OnDayStart;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            // the game clears locations when loading the save, so load the custom map after the save loads
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            // Uses harmony to patch all OriginalClassName_Patch.cs classes
            HarmonyPatcher.ApplyPatches();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            
            // when you are loaded into the world, pressing buttons logs that button to the console
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        /// <summary>Load the Rejuvenating Forest map into the world</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("Maps/RejuvenatingForest.tmx", ContentSource.ModFolder);

            // add the location
            GameLocation location = new GameLocation(mapAssetKey, "RejuvenatingForest") { IsOutdoors = true, IsFarm = false };
            Game1.locations.Add(location);

            // get the internal asset key for the map file
            string mapAssetKey2 = this.Helper.Content.GetActualAssetKey("Maps/RejuvenatingForestCave.tmx", ContentSource.ModFolder);

            // add the location
            GameLocation location2 = new GameLocation(mapAssetKey2, "RejuvenatingForestCave") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(location2);

            xTile.Map busStop = Game1.getLocationFromName("BusStop").Map;


            NPC.populateRoutesFromLocationToLocationList();
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
        }
        #endregion
    }
}