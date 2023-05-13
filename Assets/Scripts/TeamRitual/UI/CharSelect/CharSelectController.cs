using System.Collections.Generic;
using TeamRitual.Core;
using TeamRitual.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamRitual.UI {
public class CharSelectController : MonoBehaviour
{
    public static CharSelectController Instance;
    int selectMode = 0; //0 -- Character select, 1 -- Stage select

    public Dictionary<string,Sprite> spriteCache = new Dictionary<string,Sprite>();

    public int time = 0;
    public int endTime = 0;

    UIComponent leftDoor;
    UIComponent rightDoor;

    UIComponent leftPedestal;
    UIComponent rightPedestal;
    Image blackScreen;

    List<UIComponent> PlayerDisplays = new List<UIComponent>();
    List<GameObject> PlayerBodies = new List<GameObject>();
    List<GameObject> CageBars = new List<GameObject>();
    List<TMPro.TMP_Text> PlayerNames = new List<TMPro.TMP_Text>();
    
    UIComponent charSlots;
    List<CharSelectCursor> Cursors = new List<CharSelectCursor>();

    List<GameObject> characterSlots = new List<GameObject>();
    string[] selectedNames = {"Xonin","Xonin"};
    
    PlayerInputActions inputActions;
    List<InputActionMap> inputMaps = new List<InputActionMap>();

    UIComponent stageSelect;
    TMPro.TMP_Text stageName;
    string[] stages = {"Temple","City","Ruin","Dungeon","Swampland","Fallen World","Eclipse","Lab","Ritual"};
    int stageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        CharSelectController.Instance = this;

        this.leftDoor = GameObject.Find("DoorLeft").GetComponent<UIComponent>();
        this.rightDoor = GameObject.Find("DoorRight").GetComponent<UIComponent>();
        this.blackScreen = GameObject.Find("Background").GetComponent<Image>();
        this.leftPedestal = GameObject.Find("PedestalLeft").GetComponent<UIComponent>();
        this.rightPedestal = GameObject.Find("PedestalRight").GetComponent<UIComponent>();

        this.charSlots = GameObject.Find("CharSlots").GetComponent<UIComponent>();
        for (int i = 1; i <= this.CharacterCount(); i++) {
            this.PlayerDisplays.Add(GameObject.Find("Player" + i).GetComponent<UIComponent>());
            this.PlayerBodies.Add(GameObject.Find("Character" + i));
            this.CageBars.Add(GameObject.Find("Bars" + i));
            this.Cursors.Add(GameObject.Find("P"+i+"Cursor").GetComponent<CharSelectCursor>());
            this.PlayerNames.Add(GameObject.Find("Character"+i+"Name").GetComponent<TMPro.TMP_Text>());
            this.characterSlots.Add(GameObject.Find("Slot"+i));
        }

        this.stageSelect = GameObject.Find("StageSelect").GetComponent<UIComponent>();
        this.stageName = GameObject.Find("StageName").GetComponent<TMPro.TMP_Text>();

        this.inputActions = new PlayerInputActions();
        this.inputMaps.Add(this.inputActions.P1);
        this.inputMaps.Add(InputHandler.deviceP2 == null ? inputActions.P2Alt : inputActions.P2);
        foreach (InputActionMap map in this.inputMaps) {
            foreach (InputAction inputAction in map.actions) {
                inputAction.Enable();
                inputAction.started += this.PlayerInput;
            }
        }
    }

    void PlayerInput(InputAction.CallbackContext context) {
        int player = 0;
        for (int i = 1; i <= this.inputMaps.Count; i++) {
            if (this.inputMaps[i-1] == context.action.actionMap) {
                player = i;
            }
        }
        if (player < 1 || !IsPlayerDevice(context,player)) {
            return;
        }

        string input = ""+context.action.name[0];
        if (this.selectMode == 0) {//Character select mode
            switch (input) {
                case "F":
                    this.Cursors[player-1].SelectForward(1);
                    break;
                case "B":
                    this.Cursors[player-1].SelectBack(1);
                    break;
                case "L":
                case "M":
                case "H":
                case "S":
                    this.Cursors[player-1].LockSelection();
                    this.CageBars[player-1].GetComponent<CageAnimation>().Play();
                    break;
            }
        } else if (selectMode == 1 && player == 1) {//Stage select mode
            switch (input) {
                case "F":
                    this.stageIndex++;
                    break;
                case "B":
                    this.stageIndex--;
                    break;
                case "L":
                case "M":
                case "H":
                case "S":
                    for (int i = 0; i < SceneManager.sceneCount; i++) {
                        if (SceneManager.GetSceneAt(i).name == "VersusMode") {
                            return;
                        }
                    }
                    this.selectMode = 10;
                    this.endTime = this.time;
                    break;
            }
        }
    }

    bool IsPlayerDevice(InputAction.CallbackContext context, int player) {
        if (InputHandler.deviceP1 != null && InputHandler.deviceP2 != null) {
            InputDevice device = player == 1 ? InputHandler.deviceP1 : InputHandler.deviceP2;
            return context.action.activeControl.device == device;
        }
        return context.action.activeControl.device.name.ToLower().Contains("keyboard");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.time++;

        for (int i = 0; i < this.CharacterCount(); i++) {
            this.selectedNames[i] = this.GetName(Cursors[i].GetSelectedSlot());
            Cursors[i].transform.position = characterSlots[Cursors[i].GetSelectedSlot()].transform.position;
        }

        if (this.time > 0) {
            for (int i = 0; i < this.CharacterCount(); i++) {
                this.PlayerNames[i].text = selectedNames[i];
                string imagePath = "Sprites/CharacterSelect/Characters/CharSelect_"+selectedNames[i];
                this.PlayerBodies[i].GetComponent<Image>().sprite = this.GetSprite(imagePath);
            }

            stageIndex = Mathf.Clamp(stageIndex, 0, stages.Length-1);
            this.stageSelect.GetComponent<Image>().sprite = this.GetSprite("Sprites/CharacterSelect/Stage/StageSelect_"+stages[stageIndex]);
            this.stageName.text = this.stages[stageIndex];

            switch (selectMode) {
                case 0:
                    bool selectionsLocked = true;
                    for (int i = 0; i < this.CharacterCount(); i++) {
                        if (!Cursors[i].SelectionLocked()) {
                            selectionsLocked = false;
                            break;
                        }
                    }
                    if (selectionsLocked) {
                        selectMode = 1;
                    }
                    break;
            }
        
            if (this.time >= 10) {
                if (this.leftDoor.ReachedDest()) {
                    for (int i = 0; i < SceneManager.sceneCount; i++) {
                        if (SceneManager.GetSceneAt(i).name == "VersusOptions") {
                            SceneManager.UnloadSceneAsync("VersusOptions");
                        }
                        if (SceneManager.GetSceneAt(i).name == "VersusMode" && this.endTime == 0) {
                            SceneManager.UnloadSceneAsync("VersusMode");
                        }
                    }
                }
                if (selectMode == 0) {
                    for (int i = 0; i < this.CharacterCount(); i++) {
                        this.PlayerDisplays[i].destY = 100f;
                    }
                    this.leftPedestal.destY = this.rightPedestal.destY = -390f;
                    this.charSlots.destY = -150f;
                } else if (selectMode == 1) {
                    this.charSlots.destY = -700f;
                    this.stageSelect.destY = 0;
                } else if (selectMode == 10) {
                    for (int i = 0; i < this.CharacterCount(); i++) {
                        this.PlayerDisplays[i].destY = 1850f;
                    }
                    this.stageSelect.destY = -1000;
                    this.leftPedestal.destY = this.rightPedestal.destY = -1700f;

                    float modeEndTime = this.time - this.endTime;
                    if (modeEndTime == 410) {
                        Camera.main.enabled = false;
                        SceneManager.LoadSceneAsync("VersusMode",LoadSceneMode.Additive);
                        GameController.stageName = this.stages[this.stageIndex];                       
                        for (int i = 0; i < this.CharacterCount(); i++) {
                            GameController.characterNames[i] = this.GetName(this.Cursors[i].GetSelectedSlot());
                        }
                    }
                    if (modeEndTime == 420) {
                        this.leftDoor.positionLerp = 4f;
                        this.rightDoor.positionLerp = 4f;
                        this.leftDoor.destX = -1220;
                        this.rightDoor.destX = 1220;
                    } else if (modeEndTime > 420 && this.leftDoor.ReachedDest()) {
                        SceneManager.UnloadSceneAsync("CharSelect");
                    }
                }
            }
        }
    }

    public int CharacterCount() {
        return 2;
    }

    public string GetName(int slot) {
        switch (slot) {
            case 0:
                return "Xonin";
            case 1:
                return "Arracadas";
        }
        return "Xonin";
    }

    Sprite GetSprite(string directory) {
        Sprite sprite = null;
        if (this.spriteCache.ContainsKey(directory)) {
            sprite = this.spriteCache[directory];
        } else {
            sprite = Resources.Load<Sprite>(directory);
            this.spriteCache[directory] = sprite;
        }
        return sprite;
    }
}
}