using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GameArchitectureExample.StateManagement;

namespace GameArchitectureExample.Screens
{
    public class TimeTrialsMenuScreen : MenuScreen
    {
        public TimeTrialsMenuScreen() : base("Galactic Trials")
        {
            var trial1MenuEntry = new MenuEntry("Cosmic Circle");
            var trial2MenuEntry = new MenuEntry("Celestial Cross");
            var trial3MenuEntry = new MenuEntry("Dazzling Diamond");
            var trial4MenuEntry = new MenuEntry("SuperNova Spiral");
            var trial5MenuEntry = new MenuEntry("Extragalactic X");
            var backMenuEntry = new MenuEntry("Back");

            trial1MenuEntry.Selected += Trial1Selected;
            trial2MenuEntry.Selected += Trial2Selected;
            trial3MenuEntry.Selected += Trial3Selected;
            trial4MenuEntry.Selected += Trial4Selected;
            trial5MenuEntry.Selected += Trial5Selected;
            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(trial1MenuEntry);
            MenuEntries.Add(trial2MenuEntry);
            MenuEntries.Add(trial3MenuEntry);
            MenuEntries.Add(trial4MenuEntry);
            MenuEntries.Add(trial5MenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

        private void StartTimeTrial(int trialNumber, PlayerIndexEventArgs e)
        {
            var gameplayScreen = new GameplayScreen();
            gameplayScreen.InitializeTimeTrialAsteroids(trialNumber);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gameplayScreen);
        }

        private void Trial1Selected(object sender, PlayerIndexEventArgs e)
        {
            StartTimeTrial(1, e);
        }

        private void Trial2Selected(object sender, PlayerIndexEventArgs e)
        {
            StartTimeTrial(2, e);
        }

        private void Trial3Selected(object sender, PlayerIndexEventArgs e)
        {
            StartTimeTrial(3, e);
        }

        private void Trial4Selected(object sender, PlayerIndexEventArgs e)
        {
            StartTimeTrial(4, e);
        }

        private void Trial5Selected(object sender, PlayerIndexEventArgs e)
        {
            StartTimeTrial(5, e);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }
    }
}
