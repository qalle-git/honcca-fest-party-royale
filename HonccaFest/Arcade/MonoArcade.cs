﻿// MonoArcade.cs
// Version 1.00
// 2020-11-12
// Author Simon Grönberg
// LBS Kreativa Gymnasiet

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Input;
/// <summary>
/// An enumeration of all buttons for each player in the arcade
/// </summary>
public enum ArcadeButton
{
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3,
    Red = 4,
    Green = 5,
    Blue = 6,
    Yellow = 7
}

public abstract class ArcadeInputState
{
    protected bool IsDown = false;
    protected bool WasReleased = false;
    protected bool WasPressed = false;
    protected DateTime PressedTime = DateTime.Now;
    protected DateTime ReleasedTime = DateTime.Now;
    protected abstract void Update();
    public bool GetInputDown()
    {
        Update();
        if (WasPressed && DateTime.Now < PressedTime + TimeSpan.FromSeconds(0.2))
        {
            WasPressed = false;
            return true;
        }
        return false;

    }
    public bool GetInputUp()
    {
        Update();
        if (WasReleased && DateTime.Now < ReleasedTime + TimeSpan.FromSeconds(0.2))
        {
            WasReleased = false;
            return true;
        }
        return false;
    }
    public bool GetInputState()
    {
        Update();
        return IsDown;
    }
}
public class AxisState : ArcadeInputState
{
    int Sign = 0;
    int JoystickId;
    int AxisId;


    public AxisState(int intialJoystickId, int initialAxisId, bool positive)
    {
        if (positive)
            Sign = 1;
        else
            Sign = -1;
        JoystickId = intialJoystickId;
        AxisId = initialAxisId;
    }
    int GetAxisValue()
    {
        if (Joystick.GetCapabilities(JoystickId).AxisCount > AxisId)
            return Joystick.GetState(JoystickId).Axes[AxisId];
        else
            return 0;
    }
    protected override void Update()
    {

        if (GetAxisValue() * Sign > 0 && (!IsDown || DateTime.Now > PressedTime + TimeSpan.FromSeconds(0.5)))
        {
            IsDown = true;
            PressedTime = DateTime.Now;
            WasPressed = true;
        }
        if (GetAxisValue() * Sign <= 0 && IsDown)
        {
            IsDown = false;
            WasReleased = true;
            ReleasedTime = DateTime.Now;
        }

    }


}



public class KeyButtonState : ArcadeInputState
{
    Keys Key;
    public KeyButtonState(Keys buttonKey)
    {
        Key = buttonKey;



    }
    protected override void Update()
    {

        if (Keyboard.GetState().IsKeyDown(Key) && (!IsDown || DateTime.Now > PressedTime + TimeSpan.FromSeconds(0.5)))
        {
            IsDown = true;
            PressedTime = DateTime.Now;
            WasPressed = true;
        }
        if (!Keyboard.GetState().IsKeyDown(Key) && IsDown)
        {
            IsDown = false;
            WasReleased = true;
            ReleasedTime = DateTime.Now;
        }
    }
}

public class JoyButtonState : ArcadeInputState
{
    int ButtonId;
    int JoystickId;
    public JoyButtonState(int initialJoystickId, int initialButtonId)
    {
        JoystickId = initialJoystickId;
        ButtonId = initialButtonId;
    }
    protected override void Update()
    {
        if (Joystick.GetCapabilities(JoystickId).ButtonCount > ButtonId)
        {
            if (Joystick.GetState(JoystickId).Buttons[ButtonId] == ButtonState.Pressed
                && (!IsDown || DateTime.Now > PressedTime + TimeSpan.FromSeconds(0.5)))
            {
                IsDown = true;
                PressedTime = DateTime.Now;
                WasPressed = true;
            }
            if (Joystick.GetState(JoystickId).Buttons[ButtonId] == ButtonState.Released && IsDown)
            {
                IsDown = false;
                WasReleased = true;
                ReleasedTime = DateTime.Now;
            }
        }

    }
}

/// <summary>
/// Contains button mapping and score for one player
/// Maps keycodes to ArcadeButtons
/// </summary>
public class ArcadePlayer
{

    KeyButtonState[] Buttons = new KeyButtonState[8];
    int PlayerId;
    AxisState[] JoystickAxes = new AxisState[4];
    JoyButtonState[] JoystickButtons = new JoyButtonState[4];
    bool UsingJoystick = false;

    bool ingame = false;
    int score = 0;

    /// <summary>
    /// Constructor for the ArcadePlayer which sets up the button mapping to a keyboard and joysticks
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="leftButton"></param>
    /// <param name="rightButton"></param>
    /// <param name="upButton"></param>
    /// <param name="downButton"></param>
    /// <param name="redButton"></param>
    /// <param name="greenButton"></param>
    /// <param name="blueButton"></param>
    /// <param name="yellowButton"></param>
    public ArcadePlayer(int playerId, Keys leftButton, Keys rightButton, Keys upButton, Keys downButton, Keys redButton, Keys greenButton, Keys blueButton, Keys yellowButton, bool usingJoystick = false)
    {

        Buttons[0] = new KeyButtonState(leftButton);
        Buttons[1] = new KeyButtonState(rightButton);
        Buttons[2] = new KeyButtonState(upButton);
        Buttons[3] = new KeyButtonState(downButton);
        Buttons[4] = new KeyButtonState(redButton);
        Buttons[5] = new KeyButtonState(greenButton);
        Buttons[6] = new KeyButtonState(blueButton);
        Buttons[7] = new KeyButtonState(yellowButton);
        PlayerId = playerId;
        UsingJoystick = true;

        JoystickAxes[0] = new AxisState(playerId, 0, false);
        JoystickAxes[1] = new AxisState(playerId, 0, true);
        JoystickAxes[2] = new AxisState(playerId, 1, false);
        JoystickAxes[3] = new AxisState(playerId, 1, true);

        for (int i = 0; i < 4; i++)
        {
            JoystickButtons[i] = new JoyButtonState(playerId, i);
        }
    }

    /// <summary>
    /// Add this player to the game
    /// </summary>
    public void AddToGame()
    {
        ingame = true;
    }
    /// <summary>
    /// Check whether this player is in game
    /// </summary>
    /// <returns>true if ingame</returns>
    public bool IsIngame()
    {
        return ingame;
    }
    /// <summary>
    /// Check if a corresponding joystick-button or axis has been pressed
    /// </summary>
    /// <param name="button">the ArcadeButton to check</param>
    /// <returns>true if the button has been pressed</returns>
    private bool GetJoyButtonDown(ArcadeButton button)
    {
        if ((int)button > 3)
        {
            return JoystickButtons[(int)button - 4].GetInputDown();
        }
        else
        {
            return JoystickAxes[(int)button].GetInputDown();
        }
    }
    /// <summary>
    /// Check if an ArcadeButton has been pressed since last frame
    /// </summary>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button has been pressed</returns>
    public bool GetKeyDown(ArcadeButton button)
    {
        if (UsingJoystick)
        {
            if (GetJoyButtonDown(button))
            {
                return true;
            }
        }
        return Buttons[(int)button].GetInputDown();
    }
    /// <summary>
    /// Check if a corresponding joystick-button or axis has been released
    /// </summary>
    /// <param name="button">the ArcadeButton to check</param>
    /// <returns>true if the button has been released</returns>
    private bool GetJoyButtonUp(ArcadeButton button)
    {
        if ((int)button > 3)
        {
            return JoystickButtons[(int)button - 4].GetInputUp();
        }
        else
        {
            return JoystickAxes[(int)button].GetInputUp();
        }
    }
    /// <summary>
    /// Check if an ArcadeButton has been released since last frame
    /// </summary>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button has been released</returns>
    public bool GetKeyUp(ArcadeButton button)
    {
        if (UsingJoystick)
        {
            if (GetJoyButtonUp(button))
            {
                return true;
            }
        }
        return Buttons[(int)button].GetInputUp();
    }
    /// <summary>
    /// Check if a corresponding joystick-button or axis is being hold down
    /// </summary>
    /// <param name="button">the ArcadeButton to check</param>
    /// <returns>true if the button is being hold down</returns>
    private bool GetJoyButton(ArcadeButton button)
    {
        if ((int)button > 3)
        {
            return JoystickButtons[(int)button - 4].GetInputState();
        }
        else
        {
            return JoystickAxes[(int)button].GetInputState();
        }
    }
    /// <summary>
    /// Check if an ArcadeButton is being hold down
    /// </summary>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button is being hold down</returns>
    public bool GetKey(ArcadeButton button)
    {
        if (UsingJoystick)
        {
            if (GetJoyButton(button))
            {
                return true;
            }
        }
        return Buttons[(int)button].GetInputState();
    }
    /// <summary>
    /// Sets the player's score to a fixed number
    /// </summary>
    /// <param name="newScore">the score the player should have</param>
    public void SetScore(int newScore)
    {
        score = newScore;
        if (score < 0)
            score = 0;
    }
    /// <summary>
    /// Adds a number to the player's score
    /// </summary>
    /// <param name="scoreToAdd">the number the score should be increased with</param>
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
    /// <summary>
    /// Get the player's current score
    /// </summary>
    /// <returns>The player's score</returns>
    public int GetScore()
    {
        return score;
    }
    /// <summary>
    /// Subtracts a number from the player's score
    /// </summary>
    /// <param name="scoreToSubtract">the number to subract from the score</param>
    public void SubtractScore(int scoreToSubtract)
    {
        score -= scoreToSubtract;
        if (score < 0)
            score = 0;
    }

}

/// <summary>
/// A wrapper class to work with the Arcade's controls and menu system
/// </summary>
public static class MonoArcade
{
    static ArcadePlayer[] player = new ArcadePlayer[4];
    const string filePath = "arcade.txt";
    static bool Debug = false;


    static void Destructor(object sender, EventArgs e)
    {
        if (!Debug)
            Save();
    }

    /// <summary>
    /// This methods sets MonoArcade into debugging mode
    /// It ignores the transfer file and instead gives manual control to which players should be added
    /// It also sets up the controls for all players to be easily used from one keyboard.
    /// </summary>
    /// <param name="player0">Should player 0 be added?</param>
    /// <param name="player1">Should player 1 be added?</param>
    /// <param name="player2">Should player 2 be added?</param>
    /// <param name="player3">Should player 3 be added?</param>
    public static void ActivateDebug(bool player0, bool player1, bool player2, bool player3)
    {
        player[0] = new ArcadePlayer(0, Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Enter, Keys.RightShift, Keys.RightControl, Keys.Back, true);
        player[1] = new ArcadePlayer(1, Keys.A, Keys.D, Keys.W, Keys.S, Keys.Q, Keys.E, Keys.Z, Keys.X, true);
        player[2] = new ArcadePlayer(2, Keys.F, Keys.H, Keys.T, Keys.G, Keys.R, Keys.Y, Keys.V, Keys.B, true);
        player[3] = new ArcadePlayer(3, Keys.J, Keys.L, Keys.I, Keys.K, Keys.U, Keys.O, Keys.M, Keys.N, true);
        if (player0)
        {
            player[0].AddToGame();
        }
        if (player1)
        {
            player[1].AddToGame();
        }
        if (player2)
        {
            player[2].AddToGame();
        }
        if (player3)
        {
            player[3].AddToGame();
        }
        Debug = true;
    }
    /// <summary>
    /// Checks wether the MonoArcade is in debug mode
    /// </summary>
    /// <returns>true if debug is running</returns>
    public static bool IsDebugging()
    {
        return Debug;
    }
    /// <summary>
    /// Constructor that sets up the control mapping for all players
    /// </summary>
    static MonoArcade()
    {
        AppDomain.CurrentDomain.ProcessExit += Destructor;
        player[0] = new ArcadePlayer(0, Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Enter, Keys.RightShift, Keys.RightControl, Keys.Back, true);
        player[1] = new ArcadePlayer(1, Keys.A, Keys.D, Keys.W, Keys.S, Keys.Q, Keys.E, Keys.Z, Keys.X, true);
        player[2] = new ArcadePlayer(2, Keys.F, Keys.H, Keys.T, Keys.G, Keys.R, Keys.Y, Keys.V, Keys.B, true);
        player[3] = new ArcadePlayer(3, Keys.J, Keys.L, Keys.I, Keys.K, Keys.U, Keys.O, Keys.M, Keys.N, true);
        Load();
    }
    /// <summary>
    /// Reads the transfer file from the Arcade's menu system and adds the active players
    /// </summary>
    static void Load()
    {
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            int i = 0;
            while (!sr.EndOfStream && i < 4)
            {
                string line = sr.ReadLine();
                if (line == "true")
                {
                    player[i].AddToGame();
                }
                i++;
            }

            sr.Close();
        }
        else
        {
            ActivateDebug(true, true, true, true);
        }

    }
    /// <summary>
    /// Saves the score to the transfer file to the Arcade's menu system
    /// </summary>
    public static void Save()
    {
        System.Console.WriteLine("Saving...");
        StreamWriter sw = new StreamWriter(filePath);
        for (int i = 0; i < 4; i++)
        {
            sw.WriteLine(player[i].GetScore().ToString());
        }
        sw.Close();
    }
    /// <summary>
    /// Checks if the playerId is within the bounds
    /// </summary>
    /// <param name="playerId">the playerId to check</param>
    /// <returns>true if the playerId is within bounds</returns>
    static bool checkPlayerId(int playerId)
    {
        if (playerId >= 0 && playerId < 4)
        {
            return true;
        }
        return false;

    }
    /// <summary>
    /// Check if an ArcadeButton for a certain player has been pressed since last frame
    /// </summary>
    /// <param name="playerId">The player to check</param>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button has been pressed</returns>
    /// <summary>
    public static bool GetKeyDown(int playerId, ArcadeButton button)
    {
        if (checkPlayerId(playerId))
        {
            return player[playerId].GetKeyDown(button);
        }
        else
        {
            throw new System.ArgumentException("Arcade.GetKeyDown: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Check if an ArcadeButton for a certain player has been released since last frame
    /// </summary>
    /// <param name="playerId">The player to check</param>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button has been released</returns>
    /// <summary>
    public static bool GetKeyUp(int playerId, ArcadeButton button)
    {
        if (checkPlayerId(playerId))
        {
            return player[playerId].GetKeyUp(button);
        }
        else
        {
            throw new System.ArgumentException("Arcade.GetKeyUp: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Check if an ArcadeButton for a certain player is being hold down
    /// </summary>
    /// <param name="playerId">The player to check</param>
    /// <param name="button">The ArcadeButton to check</param>
    /// <returns>true if the button is being hold down</returns>
    /// <summary>
    public static bool GetKey(int playerId, ArcadeButton button)
    {
        if (checkPlayerId(playerId))
        {
            return player[playerId].GetKey(button);
        }
        else
        {
            throw new System.ArgumentException("Arcade.GetKey: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Checks whether the player is currently in the game
    /// </summary>
    /// <param name="playerId">the player to check</param>
    /// <returns></returns>
    public static bool PlayerIsIngame(int playerId)
    {
        if (playerId >= 0 && playerId < 4)
        {
            return player[playerId].IsIngame();
        }
        else
        {
            throw new System.ArgumentException("Arcade.PlayerIsIngame: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Gets the current score of a player
    /// </summary>
    /// <param name="playerId">The player whose score to get</param>
    /// <returns>the score</returns>
    public static int GetScore(int playerId)
    {
        if (checkPlayerId(playerId))
        {
            return player[playerId].GetScore();
        }
        else
        {
            throw new System.ArgumentException("Arcade.GetScore: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Sets the score of a player to a fixed number
    /// </summary>
    /// <param name="playerId">the player whose score to set</param>
    /// <param name="score">the number which the score should be set to</param>
    public static void SetScore(int playerId, int score)
    {
        if (checkPlayerId(playerId))
        {
            player[playerId].SetScore(score);
        }
        else
        {
            throw new System.ArgumentException("Arcade.SetScore: PlayerId out of bounds");
        }

    }
    /// <summary>
    /// Increases the score of a player by a certain number
    /// </summary>
    /// <param name="playerId">the player whose score to increase</param>
    /// <param name="score">the amount of points to add to the score</param>
    public static void AddScore(int playerId, int score)
    {
        if (checkPlayerId(playerId))
        {
            player[playerId].AddScore(score);
        }
        else
        {
            throw new System.ArgumentException("Arcade.AddScore: PlayerId out of bounds");
        }
    }

    /// <summary>
    /// Decreases the score of a player by a certain number
    /// </summary>
    /// <param name="playerId">the player whose score to decrease</param>
    /// <param name="score">the amount of points the score should be decreased with</param>
    public static void SubtractScore(int playerId, int score)
    {
        if (checkPlayerId(playerId))
        {
            player[playerId].SubtractScore(score);
        }
        else
        {
            throw new System.ArgumentException("Arcade.SubtractScore: PlayerId out of bounds");
        }

    }
}