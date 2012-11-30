using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestBed
{

    public enum ChordState
    {
        Pressed = 0,
        Release = 1
    }
    /// <summary>
    /// Chords are a combination of XNA Key presses.
    /// </summary>
    public struct Chord
    {
        public List<Keys> Keys
        {
            get;
            set;
        }

        public ChordState State
        {
            get;
            set;
        }

        /// <summary>
        /// Compares this Chord against a list of keys.
        /// </summary>
        /// <param name="keys">List of Chords to compare against this Chord</param>
        /// <returns>Returns 0 if the list contains all the keys in the Chord.</returns>
        public int CompareTo(List<Keys> keyList)
        {
            if (Keys.Intersect(keyList).Count() == Keys.Count)
            {
                return 0;
            }

            return -1;
        }
    }

    /// <summary>
    /// Compares two chords
    /// </summary>
    public class ChordEquityComparer : IEqualityComparer<Chord>
    {
        /// <summary>
        /// Chords are considered equal if they contain the same Keys in them
        /// </summary>
        /// <param name="chord1">First Chord to compare.</param>
        /// <param name="chord2">Second Chord to compare.</param>
        /// <returns>True if Chords are equal</returns>
        public bool Equals(Chord chord1, Chord chord2)
        {
            return chord1.Keys.Intersect(chord2.Keys).Count() == chord1.Keys.Count() && chord1.State == chord2.State;
        }

        public int GetHashCode(Chord chord)
        {
            int hCode = 0;
            chord.Keys.ForEach(k => hCode += k.GetHashCode());
            return hCode.GetHashCode();
        }
    }

    [Flags]
    public enum MouseButton
    {
        Left    = 0x01,
        Middle  = 0x02,
        Right   = 0x04,
        None    = 0x00
    }

    public class Input : GameComponent
    {
        //Previous states of input controllers
        private GamePadState m_oldGamePadState;
        private KeyboardState m_oldKeyboardState;
        private MouseState m_oldMouseState;

        //Current states of input controllers
        private GamePadState m_currentGamePadState;
        private KeyboardState m_currentKeyboardState;
        private MouseState m_currentMouseState;

        private Point m_lastMouseLocation;
        private Vector2 m_mouseMovedDist;

        private Dictionary<Chord/*Combination of keys*/, Action/*Modifier*/> m_chordActionBinding;
        private Dictionary<Chord, ChordState> m_previousChordPressStates;
        private Dictionary<Chord, ChordState> m_currentChordPressStates;

        private Dictionary<MouseButton, Tuple<ButtonState, Action>> m_mouseButtonActionBindings;
        private Dictionary<MouseButton, ButtonState> m_previousMouseButtonStates;
        private Dictionary<MouseButton, ButtonState> m_currentMouseButtonStates;


        public Input(Game game)
            : base(game)
        {
            m_chordActionBinding = new Dictionary<Chord/*Combination of keys*/, Action>/*Modifier*/();
            m_mouseButtonActionBindings = new Dictionary<MouseButton, Tuple<ButtonState, Action>>();
            m_oldMouseState = Mouse.GetState();
            m_currentMouseState = Mouse.GetState();

            m_currentMouseButtonStates = GetCurrentMouseButtonState();
            m_currentChordPressStates = GetCurrentChordPressState();

            m_currentChordPressStates = GetCurrentChordPressState();
            m_previousChordPressStates = m_currentChordPressStates;
        }

        public void Update()
        {
            m_oldGamePadState = m_currentGamePadState;
            m_oldKeyboardState = m_currentKeyboardState;
            m_oldMouseState = m_currentMouseState;

            m_currentGamePadState = GamePad.GetState(PlayerIndex.One);
            m_currentKeyboardState = Keyboard.GetState();
            m_currentMouseState = Mouse.GetState();
            
            m_mouseMovedDist = new Vector2(m_oldMouseState.X - m_currentMouseState.X, m_oldMouseState.Y - m_currentMouseState.Y);
            m_lastMouseLocation = new Point(m_currentMouseState.X, m_currentMouseState.Y);

            m_previousMouseButtonStates = m_currentMouseButtonStates;
            m_currentMouseButtonStates = GetCurrentMouseButtonState();

            m_previousChordPressStates = m_currentChordPressStates;
            m_currentChordPressStates = GetCurrentChordPressState();

            if (m_previousChordPressStates.Count == 0)
            {
                m_previousChordPressStates = m_currentChordPressStates;
            }

            ExecuteChordPresses();
            ExecuteMousePresses();
        }

        private Dictionary<Chord, ChordState> GetCurrentChordPressState()
        {
            var currentChordPressState = new Dictionary<Chord, ChordState>();

            foreach (var chordActionTuple in m_chordActionBinding)
            {
                if (chordActionTuple.Key.CompareTo(m_currentKeyboardState.GetPressedKeys().ToList()) == 0)
                {
                    currentChordPressState.Add(chordActionTuple.Key, ChordState.Pressed);
                }
                else
                {
                    currentChordPressState.Add(chordActionTuple.Key, ChordState.Release);
                }
            }

            return currentChordPressState;
        }

        private Dictionary<MouseButton, ButtonState> GetCurrentMouseButtonState()
        {
            var currentMouseButtonStates = new Dictionary<MouseButton, ButtonState>();

            currentMouseButtonStates.Add(MouseButton.Left, m_currentMouseState.LeftButton);
            currentMouseButtonStates.Add(MouseButton.Right, m_currentMouseState.RightButton);
            currentMouseButtonStates.Add(MouseButton.Middle, m_currentMouseState.MiddleButton);

            return currentMouseButtonStates;
        }

        /// <summary>
        /// Binds an action to a key press
        /// </summary>
        /// <param name="chordToBind">Chord to trigger the action to.</param>
        /// <param name="action">Action to perform</param>
        public void BindChordToAction(Chord chordToBind, Action action)
        {
            var comparer = new ChordEquityComparer();

            if (!m_chordActionBinding.Keys.Contains(chordToBind, comparer))
            {
                m_chordActionBinding.Add(chordToBind, action);
            }
        }

        public Vector2 MousePosition()
        {
            return new Vector2(m_currentMouseState.X, m_currentMouseState.Y);
        }

        /// <summary>
        /// Binds an action to a mouse button press
        /// </summary>
        /// <param name="chordToBind">Mouse button to trigger the action to.</param>
        /// <param name="action">Action to perform</param>
        public void BindMouseButtonToAction(MouseButton button, ButtonState state, Action action)
        {
            //If that button is not currently in use.
            if (!m_mouseButtonActionBindings.Keys.Contains(button))
            {
                m_mouseButtonActionBindings.Add(button, new Tuple<ButtonState, Action>(state, action));
            }
        }

        private void ExecuteMousePresses()
        {
            //Get any mouse states that are different from the previous tick
            var newButtonStates = m_currentMouseButtonStates.Where((mouseButtonState) =>
                {
                    //Check to see if the state of the current button in the list, has the same state
                    //as last tick
                    return mouseButtonState.Value != m_previousMouseButtonStates[mouseButtonState.Key];
                }).ToList();

            foreach (var mouseButtonState in newButtonStates)
            {
                if(m_mouseButtonActionBindings.ContainsKey(mouseButtonState.Key) &&
                   m_mouseButtonActionBindings[mouseButtonState.Key].Item1 == mouseButtonState.Value)
                    m_mouseButtonActionBindings[mouseButtonState.Key].Item2.Invoke();
            }
        }
        /// <summary>
        /// Responcible for executing Actions when Chord combinations are met.
        /// </summary>
        private void ExecuteChordPresses()
        {
            //Get a list of all the keys pressed that aren't modifier keys.
            var boundKeysPressed = m_currentKeyboardState.GetPressedKeys().ToList();
            var actionsToExecute = new List<Action>();

            List<Chord> chordsThatHaveChanged = new List<Chord>();

            //Find a list of Chords whose states have changed since last time.
            foreach(var chordKey in m_currentChordPressStates.Keys)
            {
                if (m_currentChordPressStates[chordKey] != m_previousChordPressStates[chordKey])
                {
                    chordsThatHaveChanged.Add(chordKey);
                }
            }

            //Attempt to execute these chords
            foreach (var chordKeyBind in chordsThatHaveChanged)
            {
                if (chordKeyBind.State == m_currentChordPressStates[chordKeyBind])
                {
                    actionsToExecute.Add(m_chordActionBinding[chordKeyBind]);
                }
            }

            actionsToExecute.ForEach(a => a.Invoke());
        }
    }
}
