using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Input;
using TeamRitual.Character;

namespace TeamRitual.Core {
public class PlayerGameObj : MonoBehaviour, ICharacter
{
    [SerializeField]
    public string characterName;
    public SoundHandler soundHandler;
    
    public PlayerGameObj enemy;
    public CharacterStateMachine stateMachine;
    public int wins = 0;

    public InputHandler inputHandler;
    public InputActionMap inputMap;

    [SerializeField]
    protected SpritePalette m_ActivePalette;
    [SerializeField]
    protected SpritePaletteGroup m_PaletteGroup;
    public int paletteNumber;

    public bool paletteSelected;
    public bool modeSelected;

    protected bool LockFlip { get; set; }

    protected Transform m_Transform;
    public Transform Transform { get { return m_Transform; } }
    public Animator m_Animator;
    public Rigidbody2D m_RigidBody;

    public Rigidbody2D RigidBody { get { return m_RigidBody; } }
    protected SpriteRenderer m_Renderer;

    protected Color m_DefaultColor;

    private HitboxManager m_HitboxManager;

    void Awake() {
        soundHandler = new SoundHandler(GetComponents<AudioSource>());
        m_Transform = transform;
        m_Transform.localScale = new Vector2(4.0f,4.0f);
        m_Renderer = GetComponent<SpriteRenderer>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_HitboxManager = GetComponent<HitboxManager>();
        m_DefaultColor = m_Renderer.color;
    }

    void OnDisable() {
        foreach (InputAction inputAction in inputMap.actions) {
            inputAction.Disable();
        }
    }

    public void SetupInput(PlayerInputActions inputActions) {
        switch (this.stateMachine.playerNumber) {
            case 0:
                this.inputMap = inputActions.P1;
                break;
            case 1:
                this.inputMap = InputHandler.deviceP2 == null ? inputActions.P2Alt : inputActions.P2;
                break;
        }

        foreach (InputAction inputAction in this.inputMap.actions) {
            inputAction.Enable();
            inputAction.performed += this.PressedInput;
            inputAction.canceled += this.ReleasedInput;
        }
    }

    void PressedInput(InputAction.CallbackContext context) {
        if (!IsPlayerDevice(context)) return;

        this.inputHandler.receiveInput(""+context.action.name[0]);
        this.inputHandler.addHeldInput(""+context.action.name[0]);
        this.stateMachine.ChangeStateOnInput();
    }

    void ReleasedInput(InputAction.CallbackContext context) {
        if (!IsPlayerDevice(context)) return;

        this.inputHandler.addReleasedInput(""+context.action.name[0]);
        this.stateMachine.ChangeStateOnInput();
    }

    bool IsPlayerDevice(InputAction.CallbackContext context) {
        if (InputHandler.deviceP1 != null && InputHandler.deviceP2 != null) {
            InputDevice device = this.stateMachine.playerNumber == 0 ? InputHandler.deviceP1 : InputHandler.deviceP2;
            return context.action.activeControl.device == device;
        }
        return context.action.activeControl.device.name.ToLower().Contains("keyboard");
    }

    void Update() {
        this.stateMachine.ChangeStateOnInput();

        string input = this.inputHandler.command;
        if (!this.paletteSelected) {
            if (input.EndsWith("F")) {
                this.inputHandler.command += ",";
                this.stateMachine.Flash(new Vector4(20f,20f,20f,1f),15);
                this.paletteForward();
            } else if (input.EndsWith("B")) {
                this.inputHandler.command += ",";
                this.stateMachine.Flash(new Vector4(20f,20f,20f,1f),15);
                this.paletteBack();
            } else if (input.EndsWith("L") || input.EndsWith("M") || input.EndsWith("H") || GameController.Instance.Global_Time >= 200) {
                GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1001), true);
                this.stateMachine.Flash(new Vector4(20f,20f,20f,1f),30);
                this.inputHandler.command += ",";
                this.paletteSelected = true;
            }
        } else if (!this.modeSelected) {
            if (input.EndsWith("F")) {
                this.inputHandler.command += ",";
                this.modeForward();
            } else if (input.EndsWith("B")) {
                this.inputHandler.command += ",";
                this.modeBack();
            } else if (input.EndsWith("L") || input.EndsWith("M") || input.EndsWith("H") || GameController.Instance.Global_Time >= 400) {
                GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1011), true);
                EffectSpawner.PlayHitEffect(
                    1011, new Vector2(this.stateMachine.PosX(),this.stateMachine.height/2f), m_Renderer.sortingOrder + 1, true,
                    GameController.Instance.GetRingColor(this.stateMachine.GetRingMode())
                );
                this.stateMachine.Flash(new Vector4(20f,1f,1f,1f),30);
                this.inputHandler.command += ",";
                this.modeSelected = true;
            }
        }
    }

    public void modeForward() {
        int modeNum = (int) this.stateMachine.GetRingMode() + 1;
        RingMode newMode = RingMode.FIRST;

        foreach (int mn in Enum.GetValues(typeof(RingMode))) {
            if (mn == modeNum) {
                newMode = (RingMode) mn;
                break;
            }
        }

        this.stateMachine.SetRingMode(newMode);
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1010), true);
        EffectSpawner.PlayHitEffect(
            1010, new Vector2(this.stateMachine.PosX(),this.stateMachine.height/2f), m_Renderer.sortingOrder - 1, true,
            GameController.Instance.GetRingColor(this.stateMachine.GetRingMode())
        );
    }

    public void modeBack() {
        int modeNum = (int) this.stateMachine.GetRingMode() - 1;
        RingMode newMode = RingMode.NINTH;

        foreach (int mn in Enum.GetValues(typeof(RingMode))) {
            if (mn == modeNum) {
                newMode = (RingMode) mn;
                break;
            }
        }

        this.stateMachine.SetRingMode(newMode);
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1010), true);
        EffectSpawner.PlayHitEffect(
            1010, new Vector2(this.stateMachine.PosX(),this.stateMachine.height/2f), m_Renderer.sortingOrder - 1, true,
            GameController.Instance.GetRingColor(this.stateMachine.GetRingMode())
        );
    }

    public void paletteForward() {
        paletteNumber = this.paletteNumber + 1;
        if (paletteNumber == this.enemy.paletteNumber && this.stateMachine.GetType() == this.enemy.stateMachine.GetType()) {
            this.paletteForward();
            return;
        }
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1000), true);
        this.SetPalette(paletteNumber);
    }

    public void paletteBack() {
        paletteNumber = this.paletteNumber - 1;
        if (paletteNumber == this.enemy.paletteNumber && this.stateMachine.GetType() == this.enemy.stateMachine.GetType()) {
            this.paletteBack();
            return;
        }
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(1000), true);
        this.SetPalette(paletteNumber);
    }

    public void SetPalette(int paletteNumber) {
        if (paletteNumber > m_PaletteGroup.Palettes.Length) {
            paletteNumber = 1;
        } else if (paletteNumber <= 0) {
            paletteNumber = m_PaletteGroup.Palettes.Length;
        }
        m_ActivePalette = m_PaletteGroup.Palettes[paletteNumber-1];
        this.paletteNumber = paletteNumber;
        SetPalette(m_ActivePalette);
    }

    public void SetPalette(SpritePalette palette)
    {
        m_ActivePalette = palette;

        if (palette != null)
        {
            var block = new MaterialPropertyBlock();

            if (m_Renderer == null) m_Renderer = GetComponent<SpriteRenderer>();
            m_Renderer.GetPropertyBlock(block);
            block.SetTexture("_SwapTex", palette.Texture);
            m_Renderer.SetPropertyBlock(block);
        }
    }

    public int NextPaletteIndex(int exceptPal) {
        for (int p = 0; p < this.m_PaletteGroup.Palettes.Length; p++) {
            if (p+1 != exceptPal) {
                Debug.Log(p+1);
                return p+1;
            }
        }

        return 0;
    }

    public SpritePalette ActivePalette { get { return m_ActivePalette; } }
    public SpritePaletteGroup PaletteGroup { get { return m_PaletteGroup; } }

    public bool FlipX
    {
        get { return m_Renderer.flipX; }
        protected set { if(LockFlip == false) m_Renderer.flipX = value; }
    }

    public virtual void HitboxContact(ContactData data)
    {
        if (stateMachine.enemy.currentState.maxHits == -1) {
            stateMachine.enemy.currentState.maxHits = data.AttackHits;
        }
        this.stateMachine.HitboxContact(data);
    }
}
}