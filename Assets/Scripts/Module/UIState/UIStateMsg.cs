using UnityEngine;
using System.Collections;

namespace AGrail
{
    public enum UIStateMsg
    {
        Init,
        ClickCard,
        ClickPlayer,
        ClickSkill,
        ClickBtn,
        ClickArgs,
    }

    public enum StateEnum
    {
        Idle = 0,
        Attack,
        Magic,
        Attacked,
        Modaned,
        Drop,
        Weaken,
        Heal,
        DropCover,
        Any = 10,
        AttackAndMagic,
        Buy,
        Extract,
        Synthetize,
        AdditionalAction,
    }
}


