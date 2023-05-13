using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Character;
using TeamRitual.UI;

namespace TeamRitual.Core {
public class GameController : MonoBehaviour {
    public static GameController Instance;
    public GCStateMachine gcStateMachine;
    
    private CameraFocus cameraFocus = CameraFocus.BOTH;
    private float destCameraZoom = 5f;
    private const float DEFAULT_ZOOM_LERP = 5f;
    private const float DEFAULT_CAMERA_LERP = 10f;
    private float cameraZoomLerp = 6f;
    private float cameraLerp = 10f;
    
    public List<PlayerGameObj> Players;
    public static List<string> characterNames = new List<string>()
    {
        "Arracadas",
        "Xonin"
    };
    public List<int> selectedPalettes = new List<int>()
    {
        1,
        3
    };
    private ContactSummary P1_Hits;
    private ContactSummary P2_Hits;

    public static string stageName = "Temple";
    private GameObject StageObj;

    BoxCollider2D StageBoundBox;
    Vector3 minBounds;
    Vector3 maxBounds;

    public int Global_Time;
    public double remainingTimerTime;
    public int playerPaused = -1;
    public int pause;
    public int trueGameTicks;

    public int currentRound = 1;

    public static double MaxTimerTime = 90;
    public static int WinsNeeded = 2;
    public static float HealthModifier = 1f;
    public static float DamageModifier = 1f;
    public static float EnergyModifier = 1f;

    [SerializeField]
    public GameObject TimerUI;
    public GameObject canvasGO;
    public List<Image> HealthBarsUI = new List<Image>();
    public List<Image> HealthBarsUIChange = new List<Image>();
    public List<Image> EnergyBarsUI = new List<Image>();
    public List<ComboCounter> ComboUI = new List<ComboCounter>();
    public List<Image> RingsUI = new List<Image>();
    public static List<Sprite> RingImages = new List<Sprite>();
    public List<Image> P1WinImages = new List<Image>();
    public List<Image> P2WinImages = new List<Image>();
    
    public SoundHandler soundHandler;
    public PlayerInputActions inputActions;

    public GameController() {
        Instance = this;
    }

    void Start()
    {
        this.inputActions = new PlayerInputActions();
        this.soundHandler = new SoundHandler(GetComponents<AudioSource>());

        GameObject canvasGO = Instantiate(Resources.Load("Prefabs/HUD/HUDPrefab_GameCanvas", typeof(GameObject)),
            Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
        TimerUI = GameObject.Find("Timer");
        TimerUI.transform.GetComponent<TMPro.TMP_Text>().text = "" + MaxTimerTime;
        remainingTimerTime = MaxTimerTime;
        for (int i = 1; i <= 2; i++) {
            HealthBarsUI.Add(GameObject.Find("P"+i+"HealthBarFill").GetComponent<Image>());
            EnergyBarsUI.Add(GameObject.Find("P"+i+"EnergyFill").GetComponent<Image>());
            RingsUI.Add(GameObject.Find("P"+i+"Ring").GetComponent<Image>());
            HealthBarsUIChange.Add(GameObject.Find("P"+i+"HealthBarChange").GetComponent<Image>());
            ComboUI.Add(GameObject.Find("P"+i+"Combo").GetComponent<ComboCounter>());
            ComboUI[ComboUI.Count-1].text = GameObject.Find("P"+i+"ComboText").GetComponent<TMPro.TMP_Text>();
            ComboUI[ComboUI.Count-1].image = GameObject.Find("P"+i+"ComboImage").GetComponent<Image>();
            GameObject.Find("P"+i+"Name").GetComponent<TMPro.TMP_Text>().text = characterNames[i-1];
        }

        RingImages.Add(Resources.Load<Sprite>("Sprites/Versus/HUD/ring/ringzero"));
        for (int i = 1; i <= 9; i++) {
            RingImages.Add(Resources.Load<Sprite>("Sprites/Versus/HUD/ring/ring"+i));
        }

        for (int index = 1; index <= 10; ++index) {
            this.P1WinImages.Add(GameObject.Find("P1Win" + index.ToString()).GetComponent<Image>());
            this.P2WinImages.Add(GameObject.Find("P2Win" + index.ToString()).GetComponent<Image>());
            this.P1WinImages[index - 1].color = new Color(1f, 1f, 1f, 0.0f);
            this.P2WinImages[index - 1].color = new Color(1f, 1f, 1f, 0.0f);
        }

        this.StageObj = Instantiate(Resources.Load("Prefabs/Stages/StagePrefab_" + stageName, typeof(GameObject))) as GameObject;
        this.StageObj.transform.SetParent(this.transform);
        this.StageBoundBox = StageObj.GetComponent<BoxCollider2D>();
        this.minBounds = this.StageBoundBox.bounds.min;
        this.maxBounds = this.StageBoundBox.bounds.max;

        for (int i = 0; i < 2; i++) {
            GameObject playerGO = Instantiate(Resources.Load("Prefabs/Characters/CharacterPrefab_"+characterNames[i], typeof(GameObject)),
                Vector3.zero, Quaternion.identity, this.gameObject.transform) as GameObject;
            Players.Add(playerGO.GetComponent<PlayerGameObj>());
        }

        for (int i = 0; i < 2; i++)
        {
            //Instantiates player objects and their state machines, then gives an input handler from the player to their character (machine).
            //Both state machines set to XoninStateMachine for now, will be able to set the state machines to different characters later.
            PlayerGameObj playerObj = Players[i];
            playerObj.gameObject.SetActive(true);

            playerObj.stateMachine = CreateStateMachine(playerObj,characterNames[i]);
            playerObj.inputHandler = new Input.InputHandler(playerObj.stateMachine);
            playerObj.stateMachine.inputHandler = playerObj.inputHandler;
            playerObj.stateMachine.soundHandler = playerObj.soundHandler;
            playerObj.stateMachine.playerNumber = i;
            playerObj.characterName = characterNames[i];

            playerObj.SetupInput(inputActions);

            playerObj.stateMachine.anim = playerObj.GetComponent<Animator>();
            playerObj.stateMachine.body = playerObj.GetComponent<Rigidbody2D>();
            playerObj.stateMachine.spriteRenderer = playerObj.GetComponent<SpriteRenderer>();

            if (i > 0) {
                Players[i-1].enemy = Players[i];
                Players[i].enemy = Players[i-1];
                Players[i-1].stateMachine.enemy = Players[i].stateMachine;
                Players[i].stateMachine.enemy = Players[i-1].stateMachine;
            }

            //Swap palettes if characters and selected palettes are the same
            if (i > 0 && selectedPalettes[i] == selectedPalettes[i-1] && Players[i].name == Players[i-1].name) {
                playerObj.SetPalette(playerObj.NextPaletteIndex(selectedPalettes[i]));
            } else {
                playerObj.SetPalette(selectedPalettes[i]);
            }
        }

        float startPosP1 = -12.0f;
        for (int i = 0; i < 2; i++)
        {
            Players[i].stateMachine.body.position = new Vector3(startPosP1 * Mathf.Pow(-1,i),6,0);
            Players[i].stateMachine.currentState.SwitchState(Players[i].stateMachine.states.Airborne());
            Players[i].stateMachine.VelX(13);
        }

        gcStateMachine = new GCStateMachine(this);
        gcStateMachine.currentState = gcStateMachine.states.Intro();
        
        GameController.Instance.CameraFocusDisable();
        GameController.Instance.SetCameraPos((float) ((Players[0].stateMachine.PosX() + Players[1].stateMachine.PosX()) / 2.0),
            (float) ((Players[0].stateMachine.height + Players[1].stateMachine.height) / 4.0) - 1f);
    }

    CharacterStateMachine CreateStateMachine(PlayerGameObj playerGameObj, string characterName) {
        Debug.Log("Creating state machine for " + characterName);
        CharacterStateMachine stateMachine = ScriptableObject.CreateInstance<CharacterStateMachine>();
        switch (characterName) {
            case "Arracadas":
                stateMachine = ScriptableObject.CreateInstance<ArracadasStateMachine>();
                break;
            case "Xonin":
                stateMachine = ScriptableObject.CreateInstance<XoninStateMachine>();
                break;
        }
        return stateMachine;
    }

    //Actual Game Controller loop. "Loop steps" in the design go here.
    void FixedUpdate()
    {
        for (int i = 0; i < Players.Count; i++)
        {
             if (this.pause == 0 || i == this.playerPaused) {
                Players[i].stateMachine.ApplyVelocity();
             }
        }

        if (this.gcStateMachine.currentState is GCStateIntro && trueGameTicks%3 == 0) {
            for (int i = 0; i < GameController.Instance.Players.Count; i++)
            {
                GameController.Instance.Players[i].stateMachine.health += (int) (GameController.Instance.Players[i].stateMachine.maxHealth * 1f/100f);
                if (GameController.Instance.Players[i].stateMachine.health > GameController.Instance.Players[i].stateMachine.maxHealth) {
                    GameController.Instance.Players[i].stateMachine.health = GameController.Instance.Players[i].stateMachine.maxHealth;
                }
            }
        }

        //True game ticks happen ten times as fast, every 0.00167s instead of 0.0167. This is for things that must be
        //calculated quickly like hitbox collisions.
        trueGameTicks++;
        if (trueGameTicks%10 != 0) {
            return;
        }

        Global_Time++;

        for (int i = 0; i < Players.Count; i++)
        {
            if (this.pause > 0) {
                if (i != this.playerPaused) {
                    if (Players[i].m_Animator != null) {
                        Players[i].m_Animator.enabled = false;
                    }
                    if (Players[i].m_RigidBody != null) {
                        Players[i].m_RigidBody.Sleep();
                    }
                }
            } else {
                this.playerPaused = -1;
                if (Players[i].m_Animator != null) {
                    Players[i].m_Animator.enabled = true;
                }
                if (Players[i].m_RigidBody != null) {
                    Players[i].m_RigidBody.WakeUp();
                }
            }

            if (Players[i].stateMachine != null && (i == this.playerPaused || this.pause == 0)) {
                Players[i].stateMachine.UpdateStates();
            }
            Players[i].inputHandler.UpdateBufferTime();
            Players[i].stateMachine.UpdateEffects();
        }

        if (this.pause > 0) {
            this.pause--;
        } else {
            P1_Hits = Players[1].stateMachine.contactSummary;
            P2_Hits = Players[0].stateMachine.contactSummary;
        }

        if (P1_Hits.NotEmpty() && Players[0].stateMachine.currentState.stateType == StateType.ATTACK
         || P2_Hits.NotEmpty() && Players[1].stateMachine.currentState.stateType == StateType.ATTACK) {
            int scoreP1 = 0;
            int maxPriorityP1 = 0;
            foreach (ContactData data in P1_Hits.bodyColData) {
                scoreP1 += (int)data.AttackPriority;
                if ((int)data.AttackPriority > maxPriorityP1) {
                    maxPriorityP1 = (int)data.AttackPriority;
                }
            }
            
            int scoreP2 = 0;
            int maxPriorityP2 = 0;
            foreach (ContactData data in P2_Hits.bodyColData) {
                scoreP2 += (int)data.AttackPriority;
                if ((int)data.AttackPriority > maxPriorityP2) {
                    maxPriorityP2 = (int)data.AttackPriority;
                }
            }

            int winningPriority = scoreP1 == scoreP2 ? scoreP1 : scoreP1 > scoreP2 ? maxPriorityP1 : maxPriorityP2;
            List<ContactData> winningHits = scoreP1 == scoreP2 ? null : scoreP1 > scoreP2 ? P1_Hits.bodyColData : P2_Hits.bodyColData;
            CharacterStateMachine characterHitting = scoreP1 == scoreP2 ? null : scoreP1 > scoreP2 ? Players[0].stateMachine : Players[1].stateMachine;
            CharacterStateMachine characterHurt = scoreP1 == scoreP2 ? null : scoreP1 < scoreP2 ? Players[0].stateMachine : Players[1].stateMachine;
            if (winningHits != null) {
                winningHits.RemoveAll(hit => (int) hit.AttackPriority < winningPriority);

                foreach (ContactData hit in winningHits) {
                    //Debug.Log(hit.AnimationName + " " + characterHitting.GetCurrentAnimationName() + " " + characterHitting.currentState.animationName);
                    //Debug.Log(characterHitting.currentState.moveContact + " " + hit.AttackHits + " ");
                    if (hit.PlayerIsSource) {
                        if (characterHitting.currentState.moveContact >= characterHitting.currentState.maxHits || 
                            hit.AnimationName != characterHitting.GetCurrentAnimationName() ||
                            hit.AnimationName != characterHitting.currentState.animationName) {
                            break;
                        }
                    }

                    bool blocked = false;
                    if (characterHurt.currentState.inputChangeState || characterHurt.blockstun > 0) {
                        bool enemyHoldingBack = characterHurt.inputHandler.held(characterHurt.inputHandler.BackInput(characterHurt));
                        bool enemyHoldingDown = characterHurt.inputHandler.held("D");
                        if (enemyHoldingBack && hit.GuardType != GuardType.UNBLOCKABLE) {//If holding back and attack isn't unblockable
                            switch (hit.GuardType) {
                                case GuardType.MID:
                                    blocked = true;
                                    break;
                                case GuardType.LOW:
                                    blocked = enemyHoldingDown && characterHurt.currentState.moveType != MoveType.AIR;
                                    break;
                                case GuardType.HIGH:
                                    blocked = !enemyHoldingDown || characterHurt.currentState.moveType == MoveType.AIR;
                                    break;
                            }
                        }
                    }

                    if (blocked) {
                        blocked = characterHurt.Block(hit, hit.GuardType);
                        if (blocked) {
                            characterHitting.currentState.OnEnemyBlocked();
                            characterHitting.currentState.moveContact++;
                        }
                    }
                    if (!blocked) {
                        if (characterHurt.Hit(hit)) {
                            characterHitting.currentState.moveContact++;
                            characterHitting.currentState.moveHit++;
                            characterHitting.currentState.OnHitEnemy();
                            if (characterHitting.attackCancels.Count == 0) {
                                characterHitting.attackCancels.Add(characterHitting.currentState.GetType().Name);
                            }
                            //Debug.Log("Hits landed: "+characterHitting.currentState.moveContact +", Max hits: "+ hit.AttackHits);
                        }
                    }
                }
            } else if (winningPriority > 0 && (Players[0].stateMachine.currentState.stateType == StateType.ATTACK || Players[1].stateMachine.currentState.stateType == StateType.ATTACK)) {
                Pause(10);
                if (P1_Hits.bodyColData.Count > 0) {
                    EffectSpawner.PlayHitEffect(
                        15, P1_Hits.bodyColData[0].Point, Players[0].stateMachine.spriteRenderer.sortingOrder + 1, !P1_Hits.bodyColData[0].TheirHitbox.Owner.FlipX
                    );
                } else if (P2_Hits.bodyColData.Count > 0) {
                    EffectSpawner.PlayHitEffect(
                        15, P2_Hits.bodyColData[0].Point, Players[1].stateMachine.spriteRenderer.sortingOrder + 1, !P2_Hits.bodyColData[0].TheirHitbox.Owner.FlipX
                    );
                }
                Players[0].stateMachine.Flash(new Vector4(2f,2f,2f,1f),5);
                Players[1].stateMachine.Flash(new Vector4(2f,2f,2f,1f),5);
            }
        }

        //Always clear hit lists at the end
        P1_Hits.Clear();
        P2_Hits.Clear();

        if (pause == 0 && Global_Time%80 == 0 && Players[0].stateMachine.health > 0 && Players[1].stateMachine.health > 0
            && this.gcStateMachine.currentState is GCStateFight) {
            CountDownTimer();
        }
        UpdateUI();
        this.gcStateMachine.currentState.UpdateState();
    }

    void Update() {//Camera movement by linearly interpolating through points
        float destCameraX = this.GetCameraX();
        float destCameraY = this.GetCameraY();
        switch (this.cameraFocus)
        {
            case CameraFocus.PLAYER1:
                destCameraX = this.Players[0].m_RigidBody.position.x;
                destCameraY = this.Players[0].m_RigidBody.position.y + this.Players[0].stateMachine.height/2 - 1;
                break;
            case CameraFocus.PLAYER2:
                destCameraX = this.Players[1].m_RigidBody.position.x;
                destCameraY = this.Players[1].m_RigidBody.position.y + this.Players[1].stateMachine.height/2 - 1;
                break;
            case CameraFocus.BOTH:
                destCameraX = (float) (((double) this.Players[0].m_RigidBody.position.x + (double) this.Players[1].m_RigidBody.position.x) / 2.0);
                destCameraY = (float) (((double) this.Players[0].m_RigidBody.position.y + (double) this.Players[1].m_RigidBody.position.y) / 2.0);
                destCameraY += (float) ((Players[0].stateMachine.height + Players[1].stateMachine.height) / 4.0) - 1f;
                break;
        }

        float halfCameraHeight = Camera.main.orthographicSize;
        float halfCameraWidth = halfCameraHeight * Screen.width / Screen.height;
        destCameraX = Mathf.Clamp(destCameraX, minBounds.x + halfCameraWidth, maxBounds.x - halfCameraWidth);
        destCameraY = Mathf.Clamp(destCameraY, minBounds.y + halfCameraHeight, maxBounds.y - halfCameraHeight);

        this.LerpCamera(new Vector3(destCameraX, destCameraY, -10f));

        this.UpdateCameraZoom();
    }

    public void CameraFocusCharacter(CharacterStateMachine stateMachine) {
        this.cameraFocus = stateMachine.playerNumber == 0 ? CameraFocus.PLAYER1 : stateMachine.playerNumber == 1 ? CameraFocus.PLAYER2 : CameraFocus.BOTH;
    }

    public void CameraFocusDisable() {
        this.cameraFocus = CameraFocus.NONE;
    }

    public void CameraFocusReset() {
        this.cameraFocus = CameraFocus.BOTH;
    }

    public void SetCameraPos(Vector2 pos) {
        Vector3 cameraDestination = new Vector3(pos.x, pos.y,-10);
        Camera.main.transform.position = cameraDestination;
    }

    public void SetCameraPos(float x, float y) {
        Vector3 cameraDestination = new Vector3(x, y,-10);
        Camera.main.transform.position = cameraDestination;
    }

    public Vector3 GetCameraPos() {
        return Camera.main.transform.position;
    }

    public float GetCameraX() {
        return Camera.main.transform.position.x;
    }

    public float GetCameraY() {
        return Camera.main.transform.position.y;
    }
    void LerpCamera(Vector3 cameraDestination) {
    // Clamp cameraDestination to stay within bounds
    float halfCameraHeight = Camera.main.orthographicSize;
    float halfCameraWidth = halfCameraHeight * Screen.width / Screen.height;
    Vector3 clampedDestination = new Vector3(
        Mathf.Clamp(cameraDestination.x, minBounds.x + halfCameraWidth, maxBounds.x - halfCameraWidth),
        Mathf.Clamp(cameraDestination.y, minBounds.y + halfCameraHeight, maxBounds.y - halfCameraHeight),
        cameraDestination.z
    );

    // Lerp towards clamped cameraDestination
    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, clampedDestination, cameraLerp * Time.deltaTime);
}


    public void SetCameraLerp(float lerp) {
        cameraLerp = lerp;
    }
    public void ResetCameraLerp() {
        this.cameraLerp = DEFAULT_CAMERA_LERP;
    }
    public float GetCameraLerp() {
        return cameraLerp;
    }

    public void SetCameraZoom(float zoom) => this.destCameraZoom = zoom;

    public void ResetCameraZoom() => this.destCameraZoom = DEFAULT_ZOOM_LERP;

    public void UpdateCameraZoom() => Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, this.destCameraZoom, this.cameraZoomLerp * Time.deltaTime);

    public float StageMinBound() {
        return minBounds.x;
    }

    public float StageMaxBound() {
        return maxBounds.x;
    }

    //Used for hitpause
    public void Pause(int time) {
        this.playerPaused = -1;
        this.pause = time;
    }

    //Pauses the game and disables physics for one character. Useful in super/dramatic pauses where only one character is affected.
    public void CharacterPause(CharacterStateMachine character, int time) {
        for (int i = 0; i < 2; i++)
        {
            if (character ==  Players[i].stateMachine) {
                this.playerPaused = i;
                break;
            }
        }
        this.pause = time;
    }

    public Color32 GetRingColor(RingMode mode) {
        switch (mode) {
            case RingMode.SECOND:
                return Color.cyan;
            case RingMode.SEVENTH:
                return Color.green;
            case RingMode.FIFTH:
                return new Color32(200,0,255,255);
            case RingMode.EIGHTH:
                return new Color32(0,0,50,255);
        }

        return new Color32(255,(byte)(220 - (int)mode*220/9),(byte)((int)mode*100/9),255);
    }

    public bool AnimationOver(Animator anim, string animationName) {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < 2; i++) {
            CharacterStateMachine sm = Players[i].stateMachine;
            if (sm.health >= 0)
            {
                HealthBarsUI[i].fillAmount = sm.health/sm.maxHealth;
                HealthBarsUIChange[i].fillAmount =  Mathf.Lerp(HealthBarsUIChange[i].fillAmount, sm.health/sm.maxHealth, 0.05f);
            }
            if (sm.GetEnergy() >= 0) {
                EnergyBarsUI[i].fillAmount = sm.GetEnergy() * (1f/sm.GetMaxEnergy());
            }
            EnergyBarsUI[i].color = GetRingColor(sm.GetRingMode());
            RingsUI[i].color = GetRingColor(sm.GetRingMode());
            RingsUI[i].sprite = RingImages[(int)sm.GetRingMode()];
            UpdateHits(i,sm.comboProcessor.GetHits(),sm.GetRingMode());
        }
    }

    public void UpdateHits(int player, int hits, RingMode ringMode) {
        if (hits > 1) {
            ComboUI[player].SetHits(hits, ringMode);
        } else {
            ComboUI[player].ClearHits();
        }
    }

    void CountDownTimer()
    {
        TMPro.TMP_Text timerText = TimerUI.transform.GetComponent<TMPro.TMP_Text>();
        if (this.remainingTimerTime > 0)
        {
            timerText.text = "" + remainingTimerTime;
            this.remainingTimerTime--;
        }
    }
}
}