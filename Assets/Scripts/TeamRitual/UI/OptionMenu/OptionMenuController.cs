using System.Collections.Generic;
using TeamRitual.Input;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TeamRitual.UI {
public class OptionMenuController : MonoBehaviour
{
    public OptionsDevice P1Device = null;
    public OptionsDevice P2Device = null;
    List<OptionsDevice> devices = new List<OptionsDevice>();
    InputDevice keyboardDevice;

    PlayerInputActions inputActions;
    InputActionMap inputMapP1;
    InputActionMap inputMapP2;
    InputActionMap inputMapAltP2;

    GameObject saveObj;

    int ticks;

    // Start is called before the first frame update
    void Start()
    {
        this.inputActions = new PlayerInputActions();
        this.inputMapP1 = this.inputActions.P1;
        this.inputMapP2 = this.inputActions.P2;
        this.inputMapAltP2 = this.inputActions.P2Alt;
        foreach (InputAction inputAction in this.inputMapP1.actions) {
            inputAction.Enable();
        }
        foreach (InputAction inputAction in this.inputMapP2.actions) {
            inputAction.Enable();
        }

        this.saveObj = GameObject.Find("Save");

        UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice> inputDevices = InputSystem.devices;
        foreach (InputDevice device in inputDevices)
        {
            if (this.devices.Count >= 4) {
                break;
            }

            string lowercaseName = device.name.ToLower();
            bool isKeyboard = lowercaseName.Contains("keyboard");
            bool isController = lowercaseName.Contains("controller") || lowercaseName.Contains("pad");
            
            if (isKeyboard || isController) {
                if (isKeyboard) {
                    this.keyboardDevice = device;
                }

                GameObject gameObject = Instantiate(Resources.Load("Prefabs/UI/OptionsDevicePrefab", typeof(GameObject))) as GameObject;
                gameObject.transform.SetParent(GameObject.Find("OptionsCanvas").transform);
                gameObject.transform.localPosition = new Vector3(0,1000,0);

                OptionsDevice optionsDevice = gameObject.GetComponent<OptionsDevice>();
                optionsDevice.device = device;
                optionsDevice.slot = this.devices.Count;
                optionsDevice.player = InputHandler.deviceP1 == device ? 1 : InputHandler.deviceP2 == device ? 2 : 0;
                if (InputHandler.deviceP1 == device) {
                    this.P1Device = optionsDevice;
                } else if (InputHandler.deviceP2 == device) {
                    this.P2Device = optionsDevice;
                }

                this.devices.Add(optionsDevice);
                
                GameObject leftArrow = GameObject.Find("LeftArrow");
                leftArrow.name += optionsDevice.slot;
                GameObject rightArrow = GameObject.Find("RightArrow");
                rightArrow.name += optionsDevice.slot;
                GameObject deviceName = GameObject.Find("DeviceName");
                deviceName.name += optionsDevice.slot;
                deviceName.GetComponent<TMPro.TMP_Text>().text = device.name;
            }
        }

        if (this.devices.Count > 0) {
            int i = 0;
            foreach (OptionsDevice optionsDevice in this.devices)
            {
                optionsDevice.destY = 90 * (this.devices.Count - 1) - 180 * i++;
                optionsDevice.setDestinations = true;
            }
        }
    }

    void FixedUpdate() {
        bool escPressed = false;
        bool enterPressed = false;

        foreach (OptionsDevice optionsDevice in devices)
        {
            if (optionsDevice.inputCooldown > 0) {
                continue;
            }

            InputDevice device = optionsDevice.device;

            string lowercaseName = device.name.ToLower();
            bool isKeyboard = lowercaseName.Contains("keyboard");
            bool isController = lowercaseName.Contains("controller") || lowercaseName.Contains("pad");
            
            bool left = false;
            bool right = false;

            if (isKeyboard) {
                left = device.TryGetChildControl("A").EvaluateMagnitude() > 0 || device.TryGetChildControl("LeftArrow").EvaluateMagnitude() > 0;
                right = device.TryGetChildControl("D").EvaluateMagnitude() > 0 || device.TryGetChildControl("RightArrow").EvaluateMagnitude() > 0;
                enterPressed = device.TryGetChildControl("Enter").EvaluateMagnitude() > 0;
                escPressed = device.TryGetChildControl("Escape").EvaluateMagnitude() > 0;
            } else if (isController) {
                UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputControl> inputControls = device.allControls;
                foreach (InputControl control in inputControls) {
                    if (control is InputControl<Vector2>) {
                        left = Mathf.Max((control as InputControl<Vector2>).ReadValue().x * -1, 0) > 0 && !left;
                        right = Mathf.Max((control as InputControl<Vector2>).ReadValue().x, 0) > 0 && !right;

                        if (left || right) {
                            break;
                        }
                    }
                }
            }

            if (devices.Count > 1) {
                switch (optionsDevice.player) {
                    case 0:
                        if (left && this.P1Device == null) {
                            optionsDevice.SetPlayer(1);
                            this.P1Device = optionsDevice;
                            return;
                        }
                        if (right && this.P2Device == null) {
                            optionsDevice.SetPlayer(2);
                            this.P2Device = optionsDevice;
                            return;
                        }
                        break;
                    case 1:
                        if (right) {
                            optionsDevice.SetPlayer(0);
                            this.P1Device = null;
                            return;
                        }
                        break;
                    case 2:
                        if (left) {
                            optionsDevice.SetPlayer(0);
                            this.P2Device = null;
                            return;
                        }
                        break;
                }
            }
        }

        if (devices.Count > 1) {
            if (this.P1Device != null && this.P2Device != null || (this.P1Device == null && this.P2Device == null)) {
                saveObj.SetActive(true);
                if (enterPressed) {
                    Save();
                }
            } else {
                saveObj.SetActive(false);
            }
        }

        if (ticks >= 600) {
            if (ticks == 600) {
                SceneManager.UnloadSceneAsync("Main Menu");
            }
            if (escPressed) {
                Exit();
            }
        }

        ticks++;
    }

    void Save() {
        inputMapP1.Enable();
        inputMapP2.Disable();
        inputMapAltP2.Disable();
        if (this.P1Device != null && this.P2Device != null) {
            InputHandler.deviceP1 = this.P1Device.device;
            InputHandler.deviceP2 = this.P2Device.device;
            this.inputMapP2.Enable();
        } else {
            InputHandler.deviceP1 = null;
            InputHandler.deviceP2 = null;
            this.inputMapAltP2.Enable();
        }
        Exit();
    }

    void Exit() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            if (SceneManager.GetSceneAt(i).name == "Main Menu") {
                return;
            }
        }
        SceneManager.LoadScene("Main Menu");
    }
}
}