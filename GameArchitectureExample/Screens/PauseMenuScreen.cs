using GameArchitectureExample.StateManagement;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace GameArchitectureExample.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class PauseMenuScreen : MenuScreen
    {
        private GameState _state;

        public PauseMenuScreen() : base("Paused")
        {
            var resumeGameMenuEntry = new MenuEntry("Resume Game");
            var saveGameMenuEntry = new MenuEntry("Save Game");
            var quitGameMenuEntry = new MenuEntry("Quit Game");

            resumeGameMenuEntry.Selected += OnCancel;
            saveGameMenuEntry.Selected += SaveGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(saveGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        private void SaveGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("save.json"))
            {
                writer.WriteLine(JsonSerializer.Serialize(_state));
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
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}
