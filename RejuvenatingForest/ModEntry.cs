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
using StardewValley.TerrainFeatures;

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Name of the map set in Content Patcher
        private readonly string REJ_FOREST_MAP_NAME = "Custom_RejuvenatingForest";
        private readonly string REJ_FOREST_CAVE_MAP_NAME = "Custom_RejuvenatingForestCave";

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
            // Update the bool flag to reflect whether the player already owns the recipe
            recievedRecipe = Game1.player.knowsRecipe("Magic Fertilizer");

            // Refresh the NPC routes so they can properly pathfind to the RejuvenatingForest
            NPC.populateRoutesFromLocationToLocationList();

            // Apply logic conditional to whether the quest line has been completed
            UpdateRejForestOnQuestStatus();
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            // Apply logic conditional to whether the quest line has been completed
            UpdateRejForestOnQuestStatus();
        }

        /// <summary>
        /// Check to see if the Heart of the Forest quest line has been completed.
        /// If so, remove the overgrowth so that the Wizard can successfully return home. :)
        /// </summary>
        private void UpdateRejForestOnQuestStatus()
        {
            // Ignore any update if the quest line hasn't been completed yet
            if (!Game1.player.hasOrWillReceiveMail("Custom_TTimber_ForestQuest_complete"))
                return;

            // Loop through all locations to get a ref to RejuvenatingForest.
            // Start at the end of the IList since RejuvenatingForest should be
            // one of the last entries in the list.
            GameLocation rejuvenatingForest = null;
            IList<GameLocation> locations = Game1.locations;
            for(int i = Game1.locations.Count - 1; i >= 0; i--)
            {
                if(locations[i].Name == REJ_FOREST_MAP_NAME) {
                    rejuvenatingForest = locations[i];
                    break;
                }
            }

            // Check to make sure the map was found
            if (rejuvenatingForest == null)
                throw new KeyNotFoundException(
                    "Map \"" + REJ_FOREST_MAP_NAME + "\" not found in Game1.locations" +
                    " (has this method been called before OnSaveLoaded()?)");

            // Success - handle all logic relevant to the quest being previously completed
            Globals.Monitor.Log("Quest has been completed, removing bushes from Twizard's home", LogLevel.Debug);

            // Remove the two berry bushes in front of the Twizard's door.
            // A foreach doesn't work here since removing an object edits the collection's indexing.
            // By starting at the end of the collection and indexing backwards,
            // all indices are preserved while iterating.
            for(int i = rejuvenatingForest.largeTerrainFeatures.Count - 1; i >= 0; i--)
            {
                // Get a reference to the current object in the loop
                LargeTerrainFeature ltf = rejuvenatingForest.largeTerrainFeatures[i];

                // Remove the object if it's at X = 74, Y = 30 (should be a bush)
                if (ltf.tilePosition.Value == new Vector2(74, 30))
                {
                    rejuvenatingForest.largeTerrainFeatures.Remove(ltf); // Remove the Bush object
                    rejuvenatingForest.Map.Layers[3].Tiles.Array[74, 30] = null; // Remove the tile for good measure
                }
                // Remove the object if it's at X = 72, Y = 31 (should be a bush)
                else if (ltf.tilePosition.Value == new Vector2(72, 31))
                {
                    rejuvenatingForest.largeTerrainFeatures.Remove(ltf); // Remove the Bush object
                    rejuvenatingForest.Map.Layers[3].Tiles.Array[72, 31] = null; // Remove the tile for good measure
                }
            }
        }
        #endregion
    }
}