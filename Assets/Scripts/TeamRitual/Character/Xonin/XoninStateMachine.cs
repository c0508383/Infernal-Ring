using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using TeamRitual.Input;
using UnityEngine;

namespace TeamRitual.Character {
public class XoninStateMachine : CharacterStateMachine
{
    public XoninStateMachine() : base() {
        this.states = new XoninStateFactory(this);
        this.characterName = "Xonin";

        this.width = 3f;
        this.height = 6f;
        this.velocityRunBack = new Vector2(-10,7);
    }

    public override ContactSummary UpdateStates()
    {
        ContactSummary summary =  base.UpdateStates();

        this.currentState.UpdateState();
        
        return summary;
    }

    public override void ChangeStateOnInput() {
        if (!(GameController.Instance.gcStateMachine.currentState is GCStateFight && GameController.Instance.pause == 0)) {
            base.ChangeStateOnInput();
            return;
        }
        string inputStr = this.GetInput();
        if (this.lastInputTime < this.inputHandler.lastInputTime && inputStr.Length > 0 &&
            (this.currentState.inputChangeState || this.currentState.moveContact >= this.currentState.hitsToCancel && this.currentState.stateType == StateType.ATTACK)) {
            this.lastInputTime = this.inputHandler.lastInputTime;

            bool airborneState = this.currentState.moveType == MoveType.AIR;
            bool standingState = this.currentState.moveType == MoveType.STAND;
            bool crouchingState = this.currentState.moveType == MoveType.CROUCH;

            //Air OK moves
            if (standingState || crouchingState || airborneState) {
                if ((inputStr.EndsWith("D,D+F,D,D+F,L") || inputStr.EndsWith("D,D+F,D,D+F,M") || inputStr.EndsWith("D,D+F,D,D+F,H"))
                    && this.GetEnergy() >= (this.maxModeActive ? 500f : 1000f) && this.GetRingMode() != RingMode.SIXTH) {
                    this.AddEnergy(this.maxModeActive ? -500f : -1000f);
                    this.currentState.SwitchState((states as XoninStateFactory).Ultimate1Start());
                    return;
                }

                if (inputStr.EndsWith("D,D+F,L") || inputStr.EndsWith("D,D+F,D+L")) {
                    this.currentState.SwitchState((states as XoninStateFactory).Special1Light());
                    return;
                }
                if (inputStr.EndsWith("D,D+F,M") || inputStr.EndsWith("D,D+F,D+M")) {
                    this.currentState.SwitchState((states as XoninStateFactory).Special1Medium());
                    return;
                }
                if (inputStr.EndsWith("D,D+F,H") || inputStr.EndsWith("D,D+F,D+H")) {
                    if ((this.GetEnergy() >= 500f || this.maxModeActive) && this.GetRingMode() != RingMode.SIXTH) {
                        this.currentState.SwitchState((states as XoninStateFactory).Special1Heavy());
                    } else {
                        this.currentState.SwitchState((states as XoninStateFactory).Special1Medium());
                    }
                    return;
                }

                if (inputStr.EndsWith("D,D+B,L") || inputStr.EndsWith("D,D+B,D+L")) {
                    this.currentState.SwitchState((states as XoninStateFactory).Special2LightRise());
                    return;
                }
                if (inputStr.EndsWith("D,D+B,M") || inputStr.EndsWith("D,D+B,D+M")) {
                    this.currentState.SwitchState((states as XoninStateFactory).Special2MediumRise());
                    return;
                }
                if (inputStr.EndsWith("D,D+B,H") || inputStr.EndsWith("D,D+B,D+H")) {
                    if ((this.GetEnergy() >= 500f || this.maxModeActive) && this.GetRingMode() != RingMode.SIXTH) {
                        this.currentState.SwitchState((states as XoninStateFactory).Special2HeavyRise());
                    } else {
                        this.currentState.SwitchState((states as XoninStateFactory).Special2MediumRise());
                    }
                    return;
                }
            }

            if (standingState || crouchingState) {
                if (inputStr.EndsWith("B,S") || (inputHandler.held(inputHandler.BackInput(this)) && inputStr.EndsWith("S"))) {
                    this.currentState.SwitchState((states as XoninStateFactory).UniqueCrane());
                    return;
                } else if (inputStr.EndsWith("F,S") || (inputHandler.held(inputHandler.ForwardInput(this)) && inputStr.EndsWith("S"))) {
                    this.currentState.SwitchState((states as XoninStateFactory).UniqueTiger());
                    return;
                } else if (inputStr.EndsWith("S") && (this.GetEnergy() >= 500f || this.maxModeActive)) {
                    this.currentState.SwitchState((states as XoninStateFactory).UniqueDragon());
                    return;
                }
            }

            if (standingState || crouchingState) {
                if (inputStr.EndsWith("D+L")) {
                    this.currentState.SwitchState((states as XoninStateFactory).CrouchLightAttack());
                    return;
                } else if (inputStr.EndsWith("D+M")) {
                    this.currentState.SwitchState((states as XoninStateFactory).CrouchMediumAttack());
                    return;
                } else if (inputStr.EndsWith("D+H")) {
                    this.currentState.SwitchState((states as XoninStateFactory).CrouchHeavyAttack());
                    return;
                }
                if (inputStr.EndsWith("L")) {
                    this.currentState.SwitchState((states as XoninStateFactory).StandLightAttack());
                    return;
                } else if (inputStr.EndsWith("M")) {
                    this.currentState.SwitchState((states as XoninStateFactory).StandMediumAttack());
                    return;
                } else if (inputStr.EndsWith("H")) {
                    this.currentState.SwitchState((states as XoninStateFactory).StandHeavyAttack());
                    return;
                }
            }
            if (airborneState) {
                if (inputStr.EndsWith("L")) {
                    this.currentState.SwitchState((states as XoninStateFactory).AirLightAttack());
                    return;
                } else if (inputStr.EndsWith("M")) {
                    this.currentState.SwitchState((states as XoninStateFactory).AirMediumAttack());
                    return;
                } else if (inputStr.EndsWith("H")) {
                    this.currentState.SwitchState((states as XoninStateFactory).AirHeavyAttack());
                    return;
                }
            }
        }
        base.ChangeStateOnInput();
    }
}
}