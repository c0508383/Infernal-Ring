public enum RingMode {
    //Default ring, no changes
    FIRST = 1,

    //Longer MAX Mode, More damage taken during it
    SECOND = 2,

    //Combo up (combos scale less), energy gain down
    THIRD = 3,

    //Energy gain up, max health down
    FOURTH = 4,

    //Agility up (more airdashes and an extra doublejump, faster walk and jump speeds), Combo down (combos scale a lot more)
    FIFTH = 5,

    //Increased damage, no blocking, super, or EX moves
    SIXTH = 6,

    //Health up, damage down
    SEVENTH = 7,

    //Damage & Health up, Agility down.
    //Only movement options are walking, jumping, crouching, and running forward at half speed.
    EIGHTH = 8,

    //"Flawless Victor" - 30% max HP, meter regen, more ground/wallbounces and airdashes/cancels
    NINTH = 9
}