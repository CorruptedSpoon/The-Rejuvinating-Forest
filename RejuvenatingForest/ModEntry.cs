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

// HARMONY
using HarmonyLib;

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod//, IAssetLoader, IAssetEditor
    {
        IDictionary<string, string> explorerDialogue;
        IDictionary<string, string> explorerSchedule;
        Texture2D explorerSprite;
        Texture2D explorerPortrait;

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

            /*explorerDialogue = helper.Content.Load<IDictionary<string, string>>("Assets/Explorer/ExplorerDialogue.json", ContentSource.ModFolder);
            explorerSchedule = helper.Content.Load<IDictionary<string, string>>("Assets/Explorer/ExplorerSchedule.json", ContentSource.ModFolder);
            explorerSprite = helper.Content.Load<Texture2D>("Assets/Explorer/explorer.png", ContentSource.ModFolder);
            explorerPortrait = helper.Content.Load<Texture2D>("Assets/Explorer/explorerPortrait.png", ContentSource.ModFolder);*/
            //CanLoad<T>(explorerDialogue, "Characters/Explorer/assets/ExplorerDialogue");
            //Load<T>(explorerDialogue, "Characters/Explorer/assets/ExplorerDialogue");

            // Uses harmony to patch all OriginalClassName_Patch.cs classes
            HarmonyPatcher.ApplyPatches();
        }
        #endregion

        #region Public Methods
        /*
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("Assets/Explorer/ExplorerDialogue"))
            {
                return true;
            }
            if(asset.AssetNameEquals("Assets/Explorer/ExplorerSchedule"))
            {
                return true;
            }
            if (asset.AssetNameEquals("Assets/Explorer/explorer"))
            {
                return true;
            }
            if (asset.AssetNameEquals("Assets/Explorer/explorerPortrait"))
            {
                return true;
            }
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if(asset.AssetNameEquals("Assets/Explorer/ExplorerDialogue"))
            {
                return (T)explorerDialogue;
            }
            if(asset.AssetNameEquals("Assets/Explorer/ExplorerSchedule"))
            {
                return (T)explorerSchedule;
            }
            if (asset.AssetNameEquals("Assets/Explorer/explorer"))
            {
                return (T)(object)explorerSprite;
            }
            if (asset.AssetNameEquals("Assets/Explorer/explorerPortrait"))
            {
                return (T)(object)explorerPortrait;
            }
            return default(T);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                return true;
            }
            return false;
        }

        
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("Explorer", "adult/polite/outgoing/positive/male/not-datable//Town/fall 17//Forest 4 5/Explorer");
            }
        }
        */
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
        #endregion
    }
}