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

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor //, IAssetLoader, 
    {
        IDictionary<string, string> explorerDialogue;
        IDictionary<string, string> explorerSchedule;
        Texture2D explorerSprite;
        Texture2D explorerPortrait;
        /*********
        ** Public methods
        *********/

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Maps/Forest"))
                return true;
            if (asset.AssetNameEquals("Maps/Beach"))
                return true;
            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Maps/Forest"))
            {
                asset.AsMap().Data.Properties["Warp"] += " 4 4 Beach 28 15";
                asset.AsMap().Data.Properties["Warp"] += " 3 4 RejuvenatingForest 94 31";
            }
            if (asset.AssetNameEquals("Maps/Beach"))
            {
                asset.AsMap().Data.Properties["Warp"] += " 26 15 Forest 5 5";
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStart;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            // the game clears locations when loading the save, so load the custom map after the save loads
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

        }

        /*********
        ** Private methods
        *********/
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


            //xTile.Map forest = Game1.getLocationFromName("Forest").Map;
            //forest.Properties.TryGetValue("Warp", out xTile.ObjectModel.PropertyValue initialWarps);
            //forest.Properties.Remove("Warp");
            //forest.Properties.Add("Warp", initialWarps + " 6 7 Beach 31 15");
            ////forest.Properties.Add("Warp", "6 7 Beach 31 15");


            //xTile.Map RejuvForest = Game1.getLocationFromName("Beach").Map;
            //RejuvForest.Properties.TryGetValue("Warp", out xTile.ObjectModel.PropertyValue rejuvWarps);
            //RejuvForest.Properties.Remove("Warp");
            //RejuvForest.Properties.Add("Warp", rejuvWarps + " 30 15 Forest 5 7");
            //RejuvForest.Properties.Add("Warp", "30 15 Forest 5 7");

            /*
            GameLocation forest = Game1.getLocationFromName("Forest");
            GameLocation rejuv = Game1.getLocationFromName("RejuvenatingForest");
            GameLocation bus = Game1.getLocationFromName("BusStop");
            GameLocation beach = Game1.getLocationFromName("Beach");

            forest.map.Properties["Warp"] += " 4 4 Beach 28 15";
            beach.map.Properties["Warp"] += " 26 15 Forest 4 5";
            forest.map.Properties["Warp"] += " 3 4 RejuvenatingForest 94 31";
            rejuv.map.Properties["Warp"] += " 96 32 Forest 3 5";

            forest.updateWarps();
            rejuv.updateWarps();
            beach.updateWarps();*/
            NPC.populateRoutesFromLocationToLocationList();
            
            
            //GameLocation
            //forest.setTileProperty(6, 7, "Building", "Warp", "6 7 Beach 31 15");
            //beach.setTileProperty(28, 15, "Building", "Warp", "28 15 Forest 5 7");

            //GameLocation forest = Game1.getLocationFromName("Forest");
            //Layer forestLayer = forest.map.GetLayer("Back");
            //Tile warpTile = forestLayer.Tiles[6, 7];
            //warpTile.Properties["@NPCWarp"] = "6 7 Town 32 64";
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("Maps/RejuvenatingForest.tmx", ContentSource.ModFolder);

            // add the location
            GameLocation location = new GameLocation(mapAssetKey, "RejuvenatingForest") { IsOutdoors = true, IsFarm = false };

            //get a reference to the index of the location, and reset it
            //GameLocation oldLocation = Game1.locations.Where(LocationListChangedEventArgs => location.name == "RejuvenatingForest").First();
            //int index = Game1.locations.IndexOf(oldLocation);
            //this.Monitor.Log(index.ToString());
            //Game1.locations[index] == location;

            //this was all for nothing all I have to do is this...
            Game1.locations.Add(location);

            foreach(SerializableDictionary<string, int> i in Game1.player.activeDialogueEvents)
            {
                foreach(KeyValuePair<string, int> j in i)
                {
                    this.Monitor.Log(j.Key, LogLevel.Debug);
                }
            }
        }
    }
}