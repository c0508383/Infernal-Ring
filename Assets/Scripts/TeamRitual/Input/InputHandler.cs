using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TeamRitual.Character;
using TeamRitual.Core;

namespace TeamRitual.Input {
public class InputHandler {
    public static InputDevice deviceP1 = null;
    public static InputDevice deviceP2 = null;

    CharacterStateMachine character = null;

    public List<string> releasedKeys = new List<string>();
    public List<string> heldKeys = new List<string>();
    public string command = "";
    string prevCommand = "";
    public int currentBufferTime;
    public const int MAX_BUFF_TIME = 10;

    public string characterInput = "";
    string prevCharacterInput = "";

    public int lastInputTime = 0;

    public InputHandler() {
    }

    public InputHandler(CharacterStateMachine character) {
        this.character = character;
    }

    public void UpdateBufferTime() {
        if (currentBufferTime > 0) {
            currentBufferTime--;
        } else {
            if (this.character != null && this.character.currentState.stateType != StateType.ATTACK) {
                ClearInput();
            }
        }
    }

    public void ClearInput() {
        this.command = "";
        this.characterInput = "";
        this.prevCommand = "";
        this.prevCharacterInput = "";
        this.releasedKeys.Clear();
    }

    public static bool IsAttackInput(string input) {
        return !(input.EndsWith("U") || input.EndsWith("D") || input.EndsWith("B") || input.EndsWith("F"));
    }

    public void receiveInput(string input) {
        if (currentBufferTime < 0 || input == "")
            return;
        
        this.prevCommand = this.command;
        this.prevCharacterInput = this.characterInput;

        if (this.command.Length > 0) {
            this.command += ",";
        }
        if (this.held("D")) {
            input = "D+" + input;
        }
        this.command += input;
        this.currentBufferTime = MAX_BUFF_TIME;
        
        if (character != null) {
            this.characterInput = this.getCharacterInput(character);
            if (this.prevCharacterInput != this.characterInput) {
                lastInputTime = GameController.Instance.trueGameTicks;
            }
        } else if (this.command != this.prevCommand) {
            lastInputTime = GameController.Instance.trueGameTicks;
        }

        Debug.Log(this.command);
    }

    public void addReleasedInput(string input) {
        heldKeys.Remove(input);
        if (!releasedKeys.Contains(input)) {
            releasedKeys.Add(input);
        }
    }
    
    public void addHeldInput(string input) {
        releasedKeys.Remove(input);
        if (!heldKeys.Contains(input)) {
            heldKeys.Add(input);
        }
    }

    public bool released(string input) {
        return releasedKeys.Contains(input);
    }

    public bool held(string input) {
        return heldKeys.Contains(input);
    }

    //Gets new character input based on the direction they're facing.
    //Inverts F and B inputs if the character is facing the -x direction (facing == -1)
    public string getCharacterInput(CharacterStateMachine character) {
        //Invert left/right inputs if facing the negative x direction
        string inputStr = String.Copy(command);

        if (character != null && character.facing == -1) {
            inputStr = inputStr.Replace("F","$");//Set forward to some unused meaningless char to swap B and F inputs correctly
            inputStr = inputStr.Replace("B","F");
            inputStr = inputStr.Replace("$","B");
        }

        return inputStr;
    }


    public string ForwardInput(CharacterStateMachine character) {
        return character != null && character.facing == 1 ? "F" : "B";
    }

    public string BackInput(CharacterStateMachine character) {
        return character != null && character.facing == 1 ? "B" : "F";
    }
}
}