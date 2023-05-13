using BlackGardenStudios.HitboxStudioPro;
using System.Collections.Generic;
using UnityEngine;

namespace TeamRitual.Character {
public abstract class CharacterState
{
	protected CharacterStateMachine character;
	protected CharacterStateFactory states;

	public int stateTime = -1;
	public string animationName;

	public int moveHit;
	public int moveContact;
	public int hitsToCancel = 1;
	public int maxHits = -1;			//Set by game controller
	public bool jumpCancel = false;
	public int scalingStep = 1;

	public int maxSelfChain = 0;
	public int selfChain = 0;

	public bool EXFlash = false;

	//The variables below can be different for each state, and are only ever defined/mutated in the state's constructor.

	public bool inputChangeState = false;	//State will allow inputs to change state in the character's UpdateState() function.
	public bool faceEnemyStart = false;	//State will adjust the facing variable in a character only at the start of the state.
	public bool faceEnemyAlways = false;		//State will always adjust the facing variable in a character to face the correct direction.

	public AttackPriority attackPriority = AttackPriority.NONE;
    public List<AttackPriority> immunePriorities = new List<AttackPriority>();
    public List<MoveType> immuneMoveTypes = new List<MoveType>();

	public PhysicsType physicsType = PhysicsType.CUSTOM;
	public MoveType moveType = MoveType.STAND;
	public StateType stateType = StateType.IDLE;

	public CharacterState(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
	{
		this.character = currentContext;
		this.states = CharacterStateFactory;
	}

	public virtual void EnterState() {
		this.character.lastContact.HitFrame = -1;
		if (this.faceEnemyStart || this.faceEnemyAlways) {
			character.correctFacing();
		}
		this.character.anim.Play(animationName, -1, 0f);
	}

	public virtual void UpdateState() {
		this.stateTime++;

		string inputStr = this.character.GetInput();

		if (this.faceEnemyAlways) {
			character.correctFacing();
		}
		if(!this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(animationName)) {
            this.character.anim.Play(animationName, -1, 0f);
        }

		//Jump & Dash Cancels
		if (this.attackPriority > AttackPriority.NONE && this.attackPriority <= AttackPriority.HEAVY && this.moveHit >= this.hitsToCancel) {
			if (this.character.airjumpCount < this.character.maxAirjumps && this.character.airdashCount < this.character.maxAirdashes
				&& this.moveType == MoveType.AIR && 
				(inputStr.EndsWith("U") || inputStr.EndsWith("U,F") || inputStr.EndsWith("F,U") || inputStr.EndsWith("U,B")  || inputStr.EndsWith("B,U")|| this.character.inputHandler.held("U"))) {
				this.SwitchState(this.states.AirjumpStart());
			}

			if (this.character.GetEnergy() >= 200 && inputStr.EndsWith("F,F")) {
				this.character.AddEnergy(-200);
				this.character.Flash(new Vector4(1.5f,1.5f,1.5f,1f),4);
				if (this.moveType == MoveType.AIR) {
					this.SwitchState(this.states.AirdashForward());
				} else {
					this.SwitchState(this.states.RunForward());
				}
			}
		}

		if (this.jumpCancel && this.character.enemy.VelY() > 0 && moveHit >= hitsToCancel) {
            if ((inputStr.EndsWith("U") || inputStr.EndsWith("U,F") || inputStr.EndsWith("F,U") || inputStr.EndsWith("U,B")  || inputStr.EndsWith("B,U"))
				|| this.character.inputHandler.held("U")) {
				CommonStateJumpStart jumpStart = this.states.JumpStart() as CommonStateJumpStart;

				ContactData hit = this.character.enemy.lastContact;
				Vector2 hitVelocity = hit.HitVelocity;
				/*
				float yDistance = 1 + this.character.enemy.PosY() - this.character.PosY();
				jumpStart.jumpVelocity = new Vector2(-hitVelocity.x + this.character.velocityJumpForward.x, (Mathf.Clamp(hitVelocity.y,8,float.MaxValue) + yDistance) * 1.4f);
				*/
				float enemyY = this.character.enemy.PosY();
				float enemyVelY = this.character.enemy.VelY();
				float enemyGravity = this.character.enemy.gravity;
				int hitVelTime = hit.HitVelocityTime;

				int ticks = 0;
				float peakY = enemyY;
				while (true)
				{
					if (ticks%10 == 0) {
						if (this.character.enemy.currentState.stateType == StateType.HURT && hitVelTime > 0) {
							hitVelTime--;
						} else {
							if (enemyVelY < 0) {
								break;
							}
							enemyVelY -= enemyGravity;
						}
					}
					peakY += enemyVelY/500f;
					ticks++;
				}
				float jumpVelocityY = Mathf.Abs(this.character.PosY() - peakY) * 4;
				jumpStart.jumpVelocity = new Vector2(-hitVelocity.x + this.character.velocityJumpForward.x, jumpVelocityY);
				
				this.SwitchState(jumpStart);
			}
        }
	}

	public virtual void ExitState() {
		this.ClearInvincibility();
	}

	public virtual void SwitchState(CharacterState newState)
	{
		if (newState.stateType == StateType.ATTACK) {
			bool canCancelInto = (newState.attackPriority >= this.attackPriority && this.attackPriority <= AttackPriority.HEAVY)
				|| (newState.attackPriority > this.attackPriority && this.attackPriority > AttackPriority.HEAVY);

			if (!canCancelInto && this.attackPriority <= AttackPriority.HEAVY) {
				bool exceptions = newState.moveType == MoveType.AIR || character.ReverseBeat();
				if (!exceptions)
					return;
			}

			if (this.character.attackCancels.Contains(newState.GetType().Name)) {
				if (newState.GetType() != this.GetType() || this.selfChain >= this.maxSelfChain) {
					return;
				}
			}
			this.character.attackCancels.Add(newState.GetType().Name);
			if (newState.GetType() == this.GetType()) {
				newState.selfChain = this.selfChain + 1;
			}

			if (this.moveHit >= this.hitsToCancel && this.stateType == StateType.ATTACK) {
				character.inputHandler.ClearInput();
			}
		}



		// exit current state
		ExitState();

		//enter new state
		newState.EnterState();

		//update context of state
		character.currentState = newState;
		//Debug.Log("Switched from " + this + " to " + newState);
	}

	public virtual void OnContact(ContactData data) {
	}

	public virtual bool OnHitEnemy() {
		return true;
	}

	public virtual bool OnEnemyBlocked() {
		return true;
	}

	public virtual bool OnHurt() {
		return true;
	}

    public void MakeInvincible() {
        this.immuneMoveTypes.Add(MoveType.STAND);
        this.immuneMoveTypes.Add(MoveType.CROUCH);
        this.immuneMoveTypes.Add(MoveType.AIR);
        this.immuneMoveTypes.Add(MoveType.LYING);
    }

    public void ClearInvincibility() {
        this.immuneMoveTypes.Clear();
        this.immunePriorities.Clear();
    }
}
}