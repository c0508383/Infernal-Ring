namespace TeamRitual.Character {
public class ArracadasStateFactory : CharacterStateFactory
{
    public ArracadasStateFactory(ArracadasStateMachine currentContext) : base(currentContext) { }

    public override CharacterState RunBack()
    {
        return new ArracadasRunBack(context, this);
    }

    public CharacterState CrouchLightAttack()
    {
        return new ArracadasCrouchLightAttack(context, this);
    }

    public CharacterState AirLightAttack()
    {
        return new ArracadasAirLightAttack(context, this);
    }

    public CharacterState StandLightAttack()
    {
        return new ArracadasStandLightAttack(context, this);
    }

    public CharacterState CrouchMediumAttack()
    {
        return new ArracadasCrouchMediumAttack(context, this);
    }

    public CharacterState AirMediumAttack()
    {
        return new ArracadasAirMediumAttack(context, this);
    }

    public CharacterState StandMediumAttack()
    {
        return new ArracadasStandMediumAttack(context, this);
    }

    public CharacterState CrouchHeavyAttack()
    {
        return new ArracadasCrouchHeavyAttack(context, this);
    }

    public CharacterState AirHeavyAttack()
    {
        return new ArracadasAirHeavyAttack(context, this);
    }

    public CharacterState StandHeavyAttack()
    {
        return new ArracadasStandHeavyAttack(context, this);
    }

    public CharacterState UltimateStart() {
        return new ArracadasUltimateStart(context, this);
    }
    
    public CharacterState UltimatePunching() {
        return new ArracadasUltimatePunching(context, this);
    }
    
    public CharacterState Load() {
        return new ArracadasLoad(context, this);
    }

    public CharacterState FireLight() {
        return new ArracadasFireLight(context, this);
    }
    public CharacterState FireMedium() {
        return new ArracadasFireMedium(context, this);
    }
    public CharacterState FireHeavy() {
        return new ArracadasFireHeavy(context, this);
    }
}
}