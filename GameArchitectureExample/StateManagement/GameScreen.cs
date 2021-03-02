using System;
using Microsoft.Xna.Framework;

namespace GameArchitectureExample.StateManagement
{
    /// <summary>
    /// A screen is a single layer of game content that has
    /// its own update and draw logic and can be combined 
    /// with other layers to create complex menus or game
    /// experiences
    /// </summary>
    public abstract class GameScreen
    {
        /// <summary>
        /// Indicates if this screen is a popup
        /// </summary>
        /// <remarks>
        /// Normally when a new screen is brought over another, 
        /// the covered screen will transition off.  However, this
        /// bool indicates the covering screen is only a popup, and 
        /// the covered screen will remain partially visible
        /// </remarks>
        public bool IsPopup { get; protected set; }

        /// <summary>
        /// The amount of time taken for this screen to transition on
        /// </summary>
        protected TimeSpan TransitionOnTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The amount of time taken for this screen to transition off
        /// </summary>
        protected TimeSpan TransitionOffTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The screen's position in the transition
        /// </summary>
        /// <value>Ranges from 0 to 1 (fully on to fully off)</value>
        protected float TransitionPosition { get; set; } = 1;

        /// <summary>
        /// The alpha value based on the current transition position
        /// </summary>
        public float TransitionAlpha => 1f - TransitionPosition;

        /// <summary>
        /// The current state of the screen
        /// </summary>
        public ScreenState ScreenState { get; set; } = ScreenState.TransitionOn;

        /// <summary>
        /// Indicates the screen is exiting for good (not simply obscured)
        /// </summary>
        /// <remarks>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </remarks>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        /// Indicates if this screen is active
        /// </summary>
        public bool IsActive => !_otherScreenHasFocus && (
            ScreenState == ScreenState.TransitionOn ||
            ScreenState == ScreenState.Active);

        private bool _otherScreenHasFocus;

        /// <summary>
        /// The ScreenManager in charge of this screen
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. 
        /// </summary>
        /// <remarks>
        /// This is used to lock the game to a specific player profile. The main menu 
        /// responds to input from any connected gamepad, but whichever player makes 
        /// a selection from this menu is given control over all subsequent screens, 
        /// so other gamepads are inactive until the controlling player returns to the 
        /// main menu.
        /// </remarks>
        public PlayerIndex? ControllingPlayer { protected get; set; }

        /// <summary>
        /// Activates the screen.  Called when the screen is added to the screen manager 
        /// or the game returns from being paused.
        /// </summary>
        public virtual void Activate() { }

        /// <summary>
        /// Deactivates the screen.  Called when the screen is removed from the screen manager 
        /// or when the game is paused.
        /// </summary>
        public virtual void Deactivate() { }

        /// <summary>
        /// Unloads content for the screen. Called when the screen is removed from the screen manager
        /// </summary>
        public virtual void Unload() { }

        /// <summary>
        /// Updates the screen. Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away forever, it should transition off
                ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransitionPosition(gameTime, TransitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                // if the screen is covered by another, it should transition off
                ScreenState = UpdateTransitionPosition(gameTime, TransitionOffTime, 1)
                    ? ScreenState.TransitionOff
                    : ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                ScreenState = UpdateTransitionPosition(gameTime, TransitionOnTime, -1)
                    ? ScreenState.TransitionOn
                    : ScreenState.Active;
            }
        }

        /// <summary>
        /// Updates the TransitionPosition property based on the time
        /// </summary>
        /// <param name="gameTime">an object representing time in the game</param>
        /// <param name="time">The amount of time the transition should take</param>
        /// <param name="direction">The direction of the transition</param>
        /// <returns>true if still transitioning, false if the transition is done</returns>
        private bool UpdateTransitionPosition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta = (time == TimeSpan.Zero)
                ? 1
                : (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition time
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (direction < 0 && TransitionPosition <= 0 || direction > 0 && TransitionPosition >= 0)
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // if not, we are still transitioning
            return true;
        }

        /// <summary>
        /// Handles input for this screen.  Only called when the screen is active,
        /// and not when another screen has taken focus.
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        /// <param name="input">An object representing input</param>
        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        /// <summary>
        /// Draws the GameScreen.  Only called with the screen is active, and not 
        /// when another screen has taken the focus.
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// This method tells the screen to exit, allowing it time to transition off
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);    // If the screen has a zero transition time, remove it immediately
            else
                IsExiting = true;    // Otherwise flag that it should transition off and then exit.
        }
    }
}
