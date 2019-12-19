using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AGrail;

//这个脚本用于执行调整UI的边缘
public class UIEdgeFix : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> roots = new List<RectTransform>();

    private void Start()
    {
        FixEdge(GameManager.UIInstance.UIEdge);
    }
    public void FixEdge(float value)
    {
        foreach (var root in roots)
        {
            //判断为四周扩展类型的锚点预设
            if (root.anchorMin == Vector2.zero && root.anchorMax == Vector2.one)
            {
                //设置左下
                root.offsetMin = new Vector2(value * 30, 0);
                //设置右上
                root.offsetMax = new Vector2(-value * 30, 0);
            }
        }
    }
}
