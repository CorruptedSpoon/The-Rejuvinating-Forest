using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        IDictionary<string, string> explorerDialogue;
        IDictionary<string, string> explorerSchedule;
        Texture2D explorerSprite;
        Texture2D explorerPortrait;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            explorerDialogue = helper.Content.Load<IDictionary<string, string>>("Assets/Explorer/ExplorerDialogue.json", ContentSource.ModFolder);
            explorerSchedule = helper.Content.Load<IDictionary<string, string>>("Assets/Explorer/ExplorerSchedule.json", ContentSource.ModFolder);
            explorerSprite = helper.Content.Load<Texture2D>("Assets/Explorer/explorer.png", ContentSource.ModFolder);
            explorerPortrait = helper.Content.Load<Texture2D>("Assets/Explorer/explorerPortrait.png", ContentSource.ModFolder);
            //CanLoad<T>(explorerDialogue, "Characters/Explorer/assets/ExplorerDialogue");
            //Load<T>(explorerDialogue, "Characters/Explorer/assets/ExplorerDialogue");
        }

        public void OnRenderWorld(object sender, RenderedWorldEventArgs e)
        {
            e.SpriteBatch.Draw(explorerSprite, )
        }

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
    }
}