namespace TeamRitual.Character {
public class CharacterStateFactory
{
    public CharacterStateMachine context;

    public CharacterStateFactory(CharacterStateMachine currentContext)
    {
        context = currentContext;
    }

    public virtual CharacterState GuardAir() {
        return new CommonStateGuardAir(context,this);
    }

    public virtual CharacterState GuardStand() {
        return new CommonStateGuardStand(context,this);
    }

    public virtual CharacterState GuardCrouch() {
        return new CommonStateGuardCrouch(context,this);
    }

    public virtual CharacterState HurtStand()
    {
        return new CommonStateHurtStand(context, this);
    }

    public virtual CharacterState HurtCrouch()
    {
        return new CommonStateHurtCrouch(context, this);
    }

    public virtual CharacterState HurtAir()
    {
        return new CommonStateHurtAir(context, this);
    }

    public virtual CharacterState HurtBounce()
    {
        return new CommonStateHurtBounce(context, this);
    }

    public virtual CharacterState HurtWallBounce()
    {
        return new CommonStateHurtWallBounce(context, this);
    }

    public virtual CharacterState HurtSlide()
    {
        return new CommonStateHurtSlide(context, this);
    }

    public virtual CharacterState LyingDown()
    {
        return new CommonStateLyingDown(context, this);
    }

    public virtual CharacterState Recover() {
        return new CommonStateRecover(context, this);
    }

    public virtual CharacterState Stand()
    {
        return new CommonStateStand(context, this);
    }
    public virtual CharacterState WalkForward()
    {
        return new CommonStateWalkForward(context, this);
    }
    public virtual CharacterState WalkBackward()
    {
        return new CommonStateWalkBackward(context, this);
    }
    public virtual CharacterState RunForward()
    {
        return new CommonStateRunForward(context, this);
    }
    public virtual CharacterState RunBack()
    {
        return new CommonStateRunBack(context, this);
    }
    public virtual CharacterState AirdashForward()
    {
        return new CommonStateAirdashForward(context, this);
    }
    public virtual CharacterState AirdashBack()
    {
        return new CommonStateAirdashBack(context, this);
    }
    public virtual CharacterState AirjumpStart()
    {
        return new CommonStateAirjumpStart(context, this);
    }
    public virtual CharacterState Airborne()
    {
        return new CommonStateAirborne(context, this);
    }
    public virtual CharacterState CrouchTransition()
    {
        return new CommonStateCrouchTransition(context, this);
    }
    public virtual CharacterState Crouch()
    {
        return new CommonStateCrouch(context, this);
    }
    public virtual CharacterState JumpStart()
    {
        return new CommonStateJumpStart(context, this);
    }
    public virtual CharacterState JumpLand()
    {
        return new CommonStateJumpLand(context, this);
    }
}
}