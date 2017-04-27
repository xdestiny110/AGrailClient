using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ItemOverlay : MonoBehaviour
    {
        private GridLayoutGroup layout = null;
        private int noOverLayCnt = 0;
        private float layoutSizeX = 0;
        private float layoutCellSizeX = 0;

        void Awake()
        {
            layout = GetComponent<GridLayoutGroup>();
            layoutCellSizeX = layout.cellSize.x;
            layoutSizeX = (transform as RectTransform).sizeDelta.x;
            noOverLayCnt = (int)(layoutSizeX / layoutCellSizeX);
        }

        void Update()
        {
            //依据item数量调节覆盖程度
            Vector2 spacing = layout.spacing;
            if (transform.childCount > noOverLayCnt)            
                spacing.x = (layoutCellSizeX * transform.childCount - layoutSizeX) / (transform.childCount - 1);
            else
                spacing.x = 0;

            layout.spacing = spacing;
        }
    }
}


