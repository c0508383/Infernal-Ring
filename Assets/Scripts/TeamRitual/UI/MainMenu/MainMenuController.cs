using System.Collections.Generic;
using TeamRitual.Input;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using BlackGardenStudios.HitboxStudioPro;

namespace TeamRitual.UI {
public class MainMenuController : MonoBehaviour
{
    int ticks = 0;

    List<GameObject> buttons = new List<GameObject>();
    string[] buttonNames = {"Versus","Options","Quit"};

    string[] scenes = {"VersusOptions","Options"};
    int select = 0;

    PlayerInputActions inputActions;
    InputActionMap inputMapP1;
    InputActionMap inputMapP2;
    InputActionMap inputMapAltP2;

    // Start is called before the first frame update
    void Start()
    {
        foreach (string buttonName in buttonNames) {
            buttons.Add(GameObject.Find(buttonName));
        }
        
        this.inputActions = new PlayerInputActions();
        this.inputMapP1 = this.inputActions.P1;
        this.inputMapP2 = this.inputActions.P2;
        this.inputMapAltP2 = this.inputActions.P2Alt;

        foreach (InputAction inputAction in this.inputMapP1.actions) {
            inputAction.Enable();
            inputAction.started += this.P1Input;
        }

        if (EffectSpawner.m_Instance == null) {
            InputDevice controllerDevice = null;
            InputDevice keyboardDevice = null;
            UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice> inputDevices = InputSystem.devices;
            foreach (InputDevice device in inputDevices)
            {
                string lowercaseName = device.name.ToLower();
                bool isKeyboard = lowercaseName.Contains("keyboard");
                bool isController = lowercaseName.Contains("controller") || lowercaseName.Contains("pad");

                if (isKeyboard && keyboardDevice == null) {
                    keyboardDevice = device;
                } else if (isController && controllerDevice == null) {
                    controllerDevice = device;
                }
            }
            if (controllerDevice != null && keyboardDevice != null) {
                this.inputMapP1.Enable();
                this.inputMapP2.Enable();
                this.inputMapAltP2.Disable();
                InputHandler.deviceP1 = controllerDevice;
                InputHandler.deviceP2 = keyboardDevice;
            }
        }
        EffectSpawner.GetSoundEffect(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ticks++;
        for (int i = 0; i < buttons.Count; i++) {
            if (i == select && ticks%40 == 0) {
                if ((ticks/40)%2 == 0) {
                    (buttons[i] as GameObject).GetComponent<TMPro.TMP_Text>().color = new Color(1,0,0.5f,255);
                } else {
                    (buttons[i] as GameObject).GetComponent<TMPro.TMP_Text>().color = new Color(0.75f,0,0,255);
                }
            } else if (i != select) {
                (buttons[i] as GameObject).GetComponent<TMPro.TMP_Text>().color = new Color(0.75f,0,0,255);
            }
        }
    }

    void P1Input(InputAction.CallbackContext context) {
        if (!IsPlayerDevice(context)) return;

        string input = ""+context.action.name[0];
        Debug.Log(input);
        switch (input) {
            case "U":
                this.select = Mathf.Clamp(this.select - 1, 0, buttonNames.Length - 1);
                break;
            case "D":
                this.select = Mathf.Clamp(this.select + 1, 0, buttonNames.Length - 1);
                break;
            case "L":
            case "M":
            case "H":
            case "S":
                foreach (InputAction inputAction in this.inputMapP1.actions) {
                    inputAction.started -= this.P1Input;
                }
                if (this.select < scenes.Length) {
                    for (int i = 0; i < SceneManager.sceneCount; i++) {
                        if (SceneManager.GetSceneAt(i).name == scenes[select]) {
                            return;
                        }
                    }
                    SceneManager.LoadScene(scenes[select],LoadSceneMode.Additive);
                } else {
                    //UnityEditor.EditorApplication.isPlaying = false;
                    Application.Quit();
                }
                break;
        }
    }

    bool IsPlayerDevice(InputAction.CallbackContext context) {
        if (InputHandler.deviceP1 != null && InputHandler.deviceP2 != null) {
            return context.action.activeControl.device == InputHandler.deviceP1;
        }
        return context.action.activeControl.device.name.ToLower().Contains("keyboard");
    }
}
}