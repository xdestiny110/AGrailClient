namespace AGrail
{
    public enum PlayerAgentState
    {
        Idle = 0x1,        
        Attacked = 0x2,
        MoDaned = 0x4,
        Weaken = 0x8,
        HealCost = 0x10,
        Discard = 0x20,
        StartUp = 0x40,

        CanSpecial = 0x80,
        CanAttack = 0x100,
        CanMagic = 0x200,
        CanSkill = 0x400,



    }
}

