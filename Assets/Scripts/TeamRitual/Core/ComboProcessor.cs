using BlackGardenStudios.HitboxStudioPro;
using System.Collections.Generic;
using TeamRitual.Character;
using UnityEngine;

namespace TeamRitual.Core {
public class ComboProcessor {
    private CharacterStateMachine character;

    private const float MIN_DAMAGE_SCALING = 0.15f;
    private Dictionary<AttackPriority,float> starterScalings = new Dictionary<AttackPriority,float>();
    private float[] stepDeductions = new float[] {
        -0.05f, -0.05f, -0.05f, -0.05f, -0.05f, -0.05f, -0.05f, 0, 0, 0, -0.025f, 0, 0, -0.025f, 0, 0, -0.025f, 0
    };

    private int comboTime = 0;
    private List<ContactData> comboHits = new List<ContactData>();
    private float currentScaling = 1.0f;
    private int scalingStep = 0;
    private int groundBounce = 0;
    private int wallBounce = 0;

    private CharacterState recentHitState = null;

    public ComboProcessor(CharacterStateMachine character) {
        this.character = character;

        starterScalings[AttackPriority.NONE] = 0.5f;
        starterScalings[AttackPriority.THROW] = 0.6f;
        starterScalings[AttackPriority.LIGHT] = 0.7f;
        starterScalings[AttackPriority.MEDIUM] = 0.9f;
        starterScalings[AttackPriority.HEAVY] = 1.0f;
        starterScalings[AttackPriority.SPECIAL] = 0.8f;
        starterScalings[AttackPriority.SUPER] = 1.0f;
    }

    public void Update() {
        if (character.enemy.health <= 0f || (character.enemy.currentState.stateType != StateType.HURT && character.enemy.currentState.moveType != MoveType.LYING)) {
            if (this.comboTime != 0) {
                Clear();
            }
        } else {
            this.comboTime++;
        }
    }

    private void Clear() {
        this.comboTime = 0;
        this.comboHits.Clear();
        this.currentScaling = 1.0f;
        this.scalingStep = 0;
        this.recentHitState = null;
        this.groundBounce = 0;
        this.wallBounce = 0;
    }

    public int GetHits() {
        return this.comboHits.Count;
    }

    public void ProcessHit(ContactData hit, CharacterState state) {
        float damageScaling = 1.0f;
        int hitstunDecay = 0;

        //change current scaling
        if (this.comboHits.Count == 0) {
            this.currentScaling = starterScalings[hit.AttackPriority];
        } else {
            //Update damage scaling
            if (this.scalingStep > 0) {
                float deduction = this.stepDeductions[this.scalingStep - 1];
                switch (this.character.GetRingMode()) {
                    case RingMode.THIRD:
                        deduction /= 1.5f;
                        break;
                    case RingMode.FIFTH:
                        deduction *= 1.5f;
                        break;
                }

                this.currentScaling = Mathf.Max(this.currentScaling + deduction, MIN_DAMAGE_SCALING);
            }
            damageScaling = this.currentScaling;

            //Update hitstun scaling
            hitstunDecay = (int) Mathf.Floor(comboTime/60f);
        }

        //Update scaling step on the first hit of the attack
        if (this.recentHitState != null && this.recentHitState.GetType() != state.GetType()) {
            this.scalingStep = (int) Mathf.Max(0, this.scalingStep + state.scalingStep);
        }
        
        //change damage and downedDamage based on scaling
        float minDamage = Mathf.Max(hit.MinDamage, Mathf.Max(1,Mathf.Floor(hit.Damage * (this.character.enemy.maxModeActive ? 0.1f : 0.025f))));
        hit.Damage = Mathf.Max(hit.Damage * damageScaling, minDamage);
        hit.DownedDamage = Mathf.Max(hit.DownedDamage * damageScaling, minDamage);

        hit.GiveSelfPower = hit.GiveSelfPower * damageScaling;

        //change hitstun and downedHitstun based on scaling
        hit.Hitstun = Mathf.Max(hit.Hitstun - hitstunDecay, 0);
        hit.DownedHitstun = Mathf.Max(hit.DownedHitstun - hitstunDecay, 0);

        this.recentHitState = state;
        this.comboHits.Add(hit);
    }

    public int GetDamage(bool downedHit) {
        if (this.comboHits.Count == 0) {
            return 0;
        }
        ContactData lastHit = this.comboHits[this.comboHits.Count - 1];
        float damage = downedHit ? (int) lastHit.DownedDamage : (int) lastHit.Damage;

        damage *= this.character.enemy.maxModeActive ? 1.5f * (this.character.enemy.GetRingMode() == RingMode.SECOND ? 2f : 1) : 1;

        switch (this.character.GetRingMode()) {
            case RingMode.SIXTH:
                damage *= 1.5f;
                break;
            case RingMode.EIGHTH:
                damage *= 1.3f;
                break;
        }

        if (this.character.enemy.GetRingMode() == RingMode.SEVENTH) {
            damage /= 1.2f;
        }

        return (int) Mathf.Floor(damage * GameController.DamageModifier);
    }

    public int GetHitstun(bool downedHit) {
        if (this.comboHits.Count == 0) {
            return 0;
        }
        ContactData lastHit = this.comboHits[this.comboHits.Count - 1];
        return downedHit ? lastHit.DownedHitstun : lastHit.Hitstun;
    }

    public float GetSelfEnergy() {
        if (this.comboHits.Count == 0) {
            return 0;
        }
        return this.comboHits[this.comboHits.Count - 1].GiveSelfPower;
    }

    public void AddWallBounce() {
        if (CanWallBounce()) {
            this.wallBounce++;
        }
    }

    public bool CanWallBounce() {
        return this.wallBounce < this.character.GetMaxWallBounce();
    }

    public bool CanGroundBounce() {
        return this.groundBounce < this.character.GetMaxGroundBounce();
    }

    public void AddGroundBounce() {
        if (CanGroundBounce()) {
            this.groundBounce++;
        }
    }

}
}