using UnityEngine;
using UnityEditor;
using AGrail;

public class Test
{
    [MenuItem("Framework/Test/TestCard")]
    public static void TestCard()
    {
        var c = Card.GetCard(55);
        c.HasSkill("");
    }

    [MenuItem("Framework/Test/TestSkill")]
    public static void TestSkill()
    {
        Skill.GetSkill(301);
    }

    [MenuItem("Framework/Test/TestHint")]
    public static void TestHint()
    {
        StateHint.GetHint(StateEnum.Any);
    }
}