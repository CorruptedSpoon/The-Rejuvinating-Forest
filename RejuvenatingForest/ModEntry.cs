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
        IDictionary<string, string> explorerDialogue;
        IDictionary<string, string> explorerSchedule;
        Texture2D explorerSprite;
        Texture2D explorerPortrait;

        private bool recievedRecipe = false;

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
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

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

            xTile.Map busStop = Game1.getLocationFromName("BusStop").Map;

            //debug purposes
            GameLocation RF = Game1.getLocationFromName("Forest");
            // Update the bool flag to reflect whether the player already owns the recipe
            recievedRecipe = Game1.player.knowsRecipe("Magic Fertilizer");

            // Refresh the NPC routes so they can properly pathfind to the RejuvenatingForest
            NPC.populateRoutesFromLocationToLocationList();

            // LoadSecretWoodsChanges();
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
        }

        private void LoadSecretWoodsChanges()
        {
            // Get a reference to Custom_Woods, but do NOT add it to the game.
            // This is used solely to load the tiles that must be changed.
            string customWoodsAssetKey = this.Helper.Content.GetActualAssetKey("Maps/Custom_Woods.tmx", ContentSource.ModFolder);
            GameLocation custom_woods = new GameLocation(customWoodsAssetKey, "Custom_Woods") { IsOutdoors = true, IsFarm = false };
            Map customWoodsMap = custom_woods.Map;

            // Get a reference to the original woods
            GameLocation woods = Game1.getLocationFromName("Woods");
            Map woodsMap = woods.Map;

            // Define layers and min/max bounds to copy over
            string[] layerNames = { "Back", "Buildings", "Front", "Paths", "AlwaysFront" };
            int minX = 0;
            int maxX = 5;
            int minY = 14;
            int maxY = 23;

            // Get a reference to the tile sheet.
            // This assumes that the top-left corner of the Back layer is a non-null tile.
            TileSheet woodsTileSheet = GetTileSheet(woodsMap);

            // For each tile on each layer, copy the custom data into the original map
            Tile tempTile;
            foreach (string layerName in layerNames)
            {
                for(int x = minX; x <= maxX; x++)
                {
                    for(int y = minY; y <= maxY; y++)
                    {
                        tempTile = customWoodsMap.GetLayer(layerName).Tiles[x, y];
                        
                        // If this is a non-null tile, overwrite its tilesheet to match the real Woods map.
                        // This skips having to use Content Patcher to inject Custom_Woods.tmx into the game
                        // just to get a seasonal sprite sheet.
                        //if (tempTile != null)
                        //    tempTile.TileSheet = woodsTileSheet;

                        woodsMap.GetLayer(layerName).Tiles[x, y] = tempTile;
                    }
                }
            }
        }

        /// <summary>
        /// Get a reference to the (first) tile sheet used on a map.
        /// </summary>
        /// <param name="map">Map object that only uses one tile sheet (Woods.tmx)</param>
        /// <returns></returns>
        private TileSheet GetTileSheet(Map map)
        {
            return map.TileSheets[0];
        }
        #endregion
    }
}