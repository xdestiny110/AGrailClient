namespace AGrail
{
    public enum PlayerAgentState
    {
        Idle = 0x0,
        Polling = 0x1,
        Attacked = 0x2,
        MoDaned = 0x4,
        Weaken = 0x8,
        HealCost = 0x10,
        Discard = 0x20,
        DiscardCovered = 0x40,
        StartUp = 0x80,
        AdditionAction = 0x100,
        SkillResponse = 0x200,

        CanSpecial = 0x400,
        CanAttack = 0x800,
        CanMagic = 0x1000,
        CanResign = 0x2000,

        ActionNone = 0x10000,
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

