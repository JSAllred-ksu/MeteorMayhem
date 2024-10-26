using Microsoft.Xna.Framework;
using GameArchitectureExample.StateManagement;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System;

namespace GameArchitectureExample.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Meteor Mayhem")
        {
            var playGameMenuEntry = new MenuEntry("New Game");
            var loadGameMenuEntry = new MenuEntry("Load Game");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (File.Exists("save.json"))
            {
                var gameplayScreen = new GameplayScreen();
                //try
                //{
                    string jsonString = File.ReadAllText("save.json");
                    if (string.IsNullOrEmpty(jsonString))
                    {
                        var messageBox = new MessageBoxScreen("Save file is empty or corrupted.");
                        ScreenManager.AddScreen(messageBox, e.PlayerIndex);
                        return;
                    }

                    GameState state = JsonSerializer.Deserialize<GameState>(jsonString);
                    LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gameplayScreen);
                    gameplayScreen.LoadState(state);
                //}
                //catch (Exception ex)
                //{
                    //var messageBox = new MessageBoxScreen($"Failed to load saved game: {ex.Message}");
                    //ScreenManager.AddScreen(messageBox, e.PlayerIndex);
                //}
            }
            else
            {
                var messageBox = new MessageBoxScreen("No saved game found.");
                ScreenManager.AddScreen(messageBox, e.PlayerIndex);
            }
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            /*const string message = "Are you sure you want to exit the game?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);*/
            ScreenManager.Game.Exit();
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
