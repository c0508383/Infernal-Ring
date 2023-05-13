using System.Collections.Generic;
using TeamRitual.Core;
using TeamRitual.Input;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TeamRitual.UI {
public class VersusOptionsController : MonoBehaviour
{
    string[] optionNames = {"Wins","Time","Life","Energy","Damage"};
    List<GameObject> optionObjects = new List<GameObject>();
    List<TMPro.TMP_Text> optionTitles = new List<TMPro.TMP_Text>();
    List<TMPro.TMP_Text> optionValueTexts = new List<TMPro.TMP_Text>();
    TMPro.TMP_Text confirmText;
    Dictionary<string,int[]> optionValues = new Dictionary<string,int[]>();

    int optionSelected;
    Dictionary<int,int> valuesSelected = new Dictionary<int,int>();

    PlayerInputActions inputActions;
    InputActionMap inputMap;

    int ticks;

    void Start() {
        this.inputActions = new PlayerInputActions();
        this.inputMap = this.inputActions.P1;

        foreach (InputAction inputAction in this.inputMap.actions) {
            inputAction.Enable();
            inputAction.performed += Input;
        }
        
        int i = 0;
        foreach (string s in this.optionNames) {
            GameObject gameObject = GameObject.Find(s);
            this.optionObjects.Add(gameObject);
            this.optionTitles.Add(gameObject.GetComponent<TMPro.TMP_Text>());
            this.optionValueTexts.Add(GameObject.Find(s + "Value").GetComponent<TMPro.TMP_Text>());
            this.valuesSelected.Add(i,0);
            i++;
        }

        this.confirmText = GameObject.Find("Confirm").GetComponent<TMPro.TMP_Text>();

        this.optionValues.Add("Wins", new int[]{2,3,4,5,6,7,8,9,10,1});
        this.optionValues.Add("Time", new int[]{90,120,150,180,240,300,int.MaxValue,10,30,60});
        this.optionValues.Add("Life", new int[]{100,150,200,250,300,10,50});
        this.optionValues.Add("Energy", new int[]{100,150,200,250,300,10,50});
        this.optionValues.Add("Damage", new int[]{100,150,200,250,300,10,50});

        SetTextValues();
    }

    bool IsPlayerDevice(InputAction.CallbackContext context) {
        if (InputHandler.deviceP1 != null) {
            return context.action.activeControl.device == InputHandler.deviceP1;
        }
        return context.action.activeControl.device.name.ToLower().Contains("keyboard");
    }

    void Input(InputAction.CallbackContext context) {
        if (context.action == this.inputActions.P1.Up) {
            this.optionSelected--;
        }
        if (context.action == this.inputActions.P1.Down) {
            this.optionSelected++;
        }
        if (this.optionSelected < optionNames.Length && this.valuesSelected.ContainsKey(this.optionSelected)) {
            string name = this.optionNames[this.optionSelected];
            int valueSelected = this.valuesSelected[this.optionSelected];
            //Increase/Decrease value index
            if (context.action == this.inputActions.P1.Forward) {
                valueSelected++;
            }
            if (context.action == this.inputActions.P1.Back) {
                valueSelected--;
            }
            //Wrap value index around
            if (valueSelected < 0) {
                valueSelected = this.optionValues[name].Length - 1;
            } else if (valueSelected >= this.optionValues[name].Length) {
                valueSelected = 0;
            }

            this.valuesSelected[this.optionSelected] = valueSelected;
        } else {
            if (context.action == this.inputActions.P1.Light) {
                Confirm();
            }
        }

        optionSelected = Mathf.Clamp(optionSelected, 0, optionNames.Length);

        SetTextValues();
    }

    void Confirm() {
        for (int i = 0; i < this.optionValueTexts.Count; i++) {
            string name = this.optionNames[i];
            int value = this.optionValues[name][this.valuesSelected[i]];

            switch (name) {
                case "Wins":
                    GameController.WinsNeeded = value;
                    break;
                case "Time":
                    GameController.MaxTimerTime = value == int.MaxValue ? double.PositiveInfinity : value;
                    break;
                case "Life":
                    GameController.HealthModifier = value/100f;
                    break;
                case "Energy":
                    GameController.EnergyModifier = value/100f;
                    break;
                case "Damage":
                    GameController.DamageModifier = value/100f;
                    break;
            }
        }
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            if (SceneManager.GetSceneAt(i).name == "CharSelect") {
                return;
            }
        }
        foreach (InputAction inputAction in this.inputMap.actions) {
            inputAction.performed -= Input;
        }
        SceneManager.LoadScene("CharSelect", LoadSceneMode.Additive);
    }

    void SetTextValues() {
        for (int i = 0; i < this.optionValueTexts.Count; i++) {
            string name = this.optionNames[i];
            this.optionValueTexts[i].text = SetText(name,this.optionValues[name][this.valuesSelected[i]]);
        }
    }

    string SetText(string option, int value) {
        switch (option) {
            case "Time":
                if (value == int.MaxValue) {
                    return "Infinite";
                }
                int minutes = (int) Mathf.Floor(value/60);
                int seconds = value - minutes*60;
                return (minutes > 0 ? minutes + "m" : "") + (minutes > 0 && seconds > 0 ? ", " : "") + (seconds > 0 ? seconds + "s" : "");
            case "Life":
            case "Energy":
            case "Damage":
                return value + "%";
        }
        return "" + value;
    }

    void FixedUpdate() {
        this.ticks++;
        
        if (this.optionObjects[0].GetComponent<UIComponent>().ReachedDest()) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                if (SceneManager.GetSceneAt(i).name == "Main Menu") {
                    SceneManager.UnloadSceneAsync("Main Menu");
                }
            }
        }

        for (int i = 0; i < this.optionNames.Length; i++) {
            this.optionTitles[i].color = Color.white;
            this.optionValueTexts[i].color = Color.white;
        }
        this.confirmText.color = Color.white;

        if (this.optionSelected < optionNames.Length) {
            for (int i = 0; i < this.optionNames.Length; i++) {
                string name = this.optionNames[i];
                if (this.optionSelected == i) {
                    Color flashColor = Mathf.Ceil(ticks/20)%2 == 0 ? Color.green : new Color(0,0.5f,0,1);
                    this.optionTitles[i].color = flashColor;
                    this.optionValueTexts[i].color = flashColor;
                }
            }
        } else {
            Color flashColor = Mathf.Ceil(ticks/20)%2 == 0 ? Color.red : new Color(0.5f,0.1f,0.5f,1);
            this.confirmText.color = flashColor;
        }
    }
}
}