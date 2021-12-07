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
    public class ModEntry : Mod
    {
        // Bool flag for whether the player has already been assigned the recipe
        private bool recievedRecipe = false;

        #region Entry method
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Globals.Helper = this.Helper; // Helper can be referenced by Globals.Helper
            Globals.Monitor = this.Monitor; // Monitor can be referenced by Globals.Logger.Log(...)
            Globals.Manifest = this.ModManifest; // Manifest can be referenced by Globals.Manifest

            // Add event hooks for our own custom methods
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

            // Use a frequent event check to add the Magic Fertilizer as soon as the player completes the quest line.
            // TODO: Refactor this to use a less resource-intensive event hook (maybe Helper.Events.Player.InventoryChanged?)
            if (!recievedRecipe && Game1.player.hasOrWillReceiveMail("Custom_TTimber_ForestQuest_complete"))
            {
                Game1.player.craftingRecipes.Add("Magic Fertilizer", 0);
                recievedRecipe = true;
            }
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

            // Update the bool flag to reflect whether the player already owns the recipe
            recievedRecipe = Game1.player.knowsRecipe("Magic Fertilizer");

            // Refresh the NPC routes so they can properly pathfind to the RejuvenatingForest
            NPC.populateRoutesFromLocationToLocationList();
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
        }
        #endregion
    }
}