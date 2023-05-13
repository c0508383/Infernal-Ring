using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using TeamRitual.Input;
using UnityEngine;

namespace TeamRitual.Character {
public class ArracadasStateMachine : CharacterStateMachine
{
    int bullets = 3;
    int maxBullets = 7;

    public ArracadasStateMachine() : base() {
        this.states = new ArracadasStateFactory(this);
        this.characterName = "Arracadas";

        this.width = 2f;
        this.height = 10f;
        this.velocityRunBack = new Vector2(-14,6);
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
        if (this.currentState.inputChangeState || this.currentState.moveHit >= this.currentState.hitsToCancel && this.currentState.stateType == StateType.ATTACK) {
            bool airborneState = this.currentState.moveType == MoveType.AIR;
            bool standingState = this.currentState.moveType == MoveType.STAND;
            bool crouchingState = this.currentState.moveType == MoveType.CROUCH;

            //Air OK moves
            if (standingState || crouchingState || airborneState) {
            }

            if (standingState || crouchingState) {
                if ((inputStr.EndsWith("D,D+F,D,D+F,L") || inputStr.EndsWith("D,D+F,D,D+F,M") || inputStr.EndsWith("D,D+F,D,D+F,H"))
                    && this.GetEnergy() >= (this.maxModeActive ? 500f : 1000f) && this.GetRingMode() != RingMode.SIXTH) {
                    this.AddEnergy(this.maxModeActive ? -500f : -1000f);
                    this.currentState.SwitchState((states as ArracadasStateFactory).UltimateStart());
                    return;
                }
                
                if (inputStr.EndsWith("D,D+F,L") || inputStr.EndsWith("D,D+F,D+L")) {
                    if (this.bullets > 0) {
                        this.currentState.SwitchState((states as ArracadasStateFactory).FireLight());
                    }
                    return;
                }
                if (inputStr.EndsWith("D,D+F,M") || inputStr.EndsWith("D,D+F,D+M")) {
                    if (this.bullets > 0) {
                        this.currentState.SwitchState((states as ArracadasStateFactory).FireMedium());
                    }
                    return;
                }
                if (inputStr.EndsWith("D,D+F,H") || inputStr.EndsWith("D,D+F,D+H")) {
                    if (this.bullets > 0 && !(this.currentState is ArracadasFireHeavy)) {
                        if ((this.GetEnergy() >= 500f || this.maxModeActive) && this.GetRingMode() != RingMode.SIXTH) {
                            this.currentState.SwitchState((states as ArracadasStateFactory).FireHeavy());
                        } else {
                            this.currentState.SwitchState((states as ArracadasStateFactory).FireMedium());
                        }
                    }
                    return;
                }

                if (inputStr.EndsWith("D+L")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).CrouchLightAttack());
                    return;
                }
                if (inputStr.EndsWith("D+M")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).CrouchMediumAttack());
                    return;
                }
                if (inputStr.EndsWith("D+H")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).CrouchHeavyAttack());
                    return;
                }
                if (inputStr.EndsWith("L")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).StandLightAttack());
                    return;
                }
                if (inputStr.EndsWith("M")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).StandMediumAttack());
                    return;
                }
                if (inputStr.EndsWith("H")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).StandHeavyAttack());
                    return;
                }

                if (inputStr.EndsWith("S") && this.bullets < this.maxBullets) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).Load());
                    return;
                }
            }

            if (standingState) {
                
            }
            if (crouchingState) {

            }
            if (airborneState) {
                if (inputStr.EndsWith("L")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).AirLightAttack());
                    return;
                }
                if (inputStr.EndsWith("M")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).AirMediumAttack());
                    return;
                }
                if (inputStr.EndsWith("H")) {
                    this.currentState.SwitchState((states as ArracadasStateFactory).AirHeavyAttack());
                    return;
                }
            }
        }
        base.ChangeStateOnInput();
    }

    public void UseBullet() {
        if (this.bullets > 0) {
            this.bullets--;
        }
    }

    public void LoadBullet() {
        if (this.bullets < this.GetMaxBullets()) {
            this.bullets++;
        }
    }

    public int GetBullets() {
        return this.bullets;
    }

    public int GetMaxBullets() {
        return this.maxBullets;
    }
}
}