using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace RejuvenatingForest
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            //trying to get explorer npc to spawn, I have all the files created but getting him to spawn through code is proving, difficult
            //only information I can find that's of use is using the content patcher mod so it's just going to take a bunch more digging in the 
            //documentation I guess, which is kinda not too helpful but, yeah...
            //AnimatedSprite sprite = helper.Content.Load<AnimatedSprite>("assets/ExplorerPlaceholder.png", ContentSource.ModFolder);
            //NPC explorer(sprite, Vector2(20,20), 1, "Explorer", null);
            //Game1.getLocationFromName("Town").addCharacter(explorer);
        }

        /*
        //trying to add explorer npc things
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Characters/Dialogue/Explorer");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            return (T)(object)new Dictionary<string, string> // (T)(object) converts a known type to the generic 'T' placeholder
            {
                ["Introduction"] = "Hi there! My name is the Explorer."
            };
        }
        */



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