namespace TeamRitual.Character {
public class XoninStateFactory : CharacterStateFactory
{
    public XoninStateFactory(XoninStateMachine currentContext) : base(currentContext) { }

    public override CharacterState RunBack()
    {
        return new XoninRunBack(context, this);
    }

    public CharacterState StandLightAttack()
    {
        return new XoninStandLightAttack(context, this);
    }

    public CharacterState CrouchLightAttack()
    {
        return new XoninCrouchLightAttack(context, this);
    }

    public CharacterState AirLightAttack()
    {
        return new XoninAirLightAttack(context, this);
    }

    public CharacterState StandMediumAttack()
    {
        return new XoninStandMediumAttack(context, this);
    }

    public CharacterState CrouchMediumAttack()
    {
        return new XoninCrouchMediumAttack(context, this);
    }

    public CharacterState AirMediumAttack()
    {
        return new XoninAirMediumAttack(context, this);
    }

    public CharacterState StandHeavyAttack()
    {
        return new XoninStandHeavyAttack(context, this);
    }

    public CharacterState CrouchHeavyAttack()
    {
        return new XoninCrouchHeavyAttack(context, this);
    }

    public CharacterState AirHeavyAttack()
    {
        return new XoninAirHeavyAttack(context, this);
    }

    public CharacterState Special1Light() {
        return new XoninSpecial1Light(context, this);
    }
    public CharacterState Special1Medium() {
        return new XoninSpecial1Medium(context, this);
    }
    public CharacterState Special1Heavy() {
        return new XoninSpecial1Heavy(context, this);
    }

    public CharacterState Special2LightRise() {
        return new XoninSpecial2LightRise(context, this);
    }
    public CharacterState Special2LightChop() {
        return new XoninSpecial2LightChop(context, this);
    }
    public CharacterState Special2LightLand() {
        return new XoninSpecial2LightLand(context, this);
    }

    public CharacterState Special2MediumRise() {
        return new XoninSpecial2MediumRise(context, this);
    }
    public CharacterState Special2MediumChop() {
        return new XoninSpecial2MediumChop(context, this);
    }
    public CharacterState Special2MediumLand() {
        return new XoninSpecial2MediumLand(context, this);
    }

    public CharacterState Special2HeavyRise() {
        return new XoninSpecial2HeavyRise(context, this);
    }
    public CharacterState Special2HeavyChop() {
        return new XoninSpecial2HeavyChop(context, this);
    }
    public CharacterState Special2HeavyLand() {
        return new XoninSpecial2HeavyLand(context, this);
    }

    public CharacterState UniqueCrane() {
        return new XoninUniqueCrane(context, this);
    }
    public CharacterState UniqueCraneCounter() {
        return new XoninUniqueCraneCounter(context, this);
    }
    public CharacterState UniqueTiger() {
        return new XoninUniqueTiger(context, this);
    }
    public CharacterState UniqueTigerCounter() {
        return new XoninUniqueTigerCounter(context, this);
    }
    public CharacterState UniqueDragon() {
        return new XoninUniqueDragon(context, this);
    }

    public CharacterState Ultimate1Start() {
        return new XoninUltimate1Start(context, this);
    }
    public CharacterState Ultimate1Punching1() {
        return new XoninUltimate1Punching1(context, this);
    }
    public CharacterState Ultimate1Punching2() {
        return new XoninUltimate1Punching2(context, this);
    }
    public CharacterState Ultimate1Punching3() {
        return new XoninUltimate1Punching3(context, this);
    }
    public CharacterState Ultimate1PunchingEnd() {
        return new XoninUltimate1PunchingEnd(context, this);
    }
}
}