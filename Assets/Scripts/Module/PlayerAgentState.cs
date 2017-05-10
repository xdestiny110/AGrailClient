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
        AdditionAction = 0x80,
        SkillResponse = 0x100,

        CanSpecial = 0x200,
        CanAttack = 0x400,
        CanMagic = 0x800,
        CanResign = 0x1000,
    }

    public static class PlayerAgentStateExtension
    {
        public static bool Check(this int agent, PlayerAgentState state)
        {
            if ((agent & (int)state) != 0)
                return true;
            return false;
        }
    }
    
}

