using GameArchitectureExample.StateManagement;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace GameArchitectureExample.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class PauseMenuScreen : MenuScreen
    {
        private readonly GameplayScreen _gameplayScreen;

        public PauseMenuScreen(GameplayScreen gameplayScreen) : base("Paused")
        {
            _gameplayScreen = gameplayScreen;

            var resumeGameMenuEntry = new MenuEntry("Resume Game");
            var saveGameMenuEntry = new MenuEntry("Save Game");
            var quitGameMenuEntry = new MenuEntry("Quit to Title");

            resumeGameMenuEntry.Selected += OnCancel;
            saveGameMenuEntry.Selected += SaveGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(saveGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        private void SaveGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            try
            {
                var state = _gameplayScreen.SaveState();
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(state, options);
                File.WriteAllText("save.json", jsonString);

                var messageBox = new MessageBoxScreen("Game has been saved successfully!");
                ScreenManager.AddScreen(messageBox, e.PlayerIndex);
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBoxScreen($"Failed to save game: {ex.Message}");
                ScreenManager.AddScreen(messageBox, e.PlayerIndex);
            }
        }

        private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";
            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        // This uses the loading screen to transition from the game back to the main menu screen.
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}
