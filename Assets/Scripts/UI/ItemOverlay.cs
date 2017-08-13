using Framework.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AGrail
{
    [RequireComponent(typeof(GridLayoutGroup))]
    [RequireComponent(typeof(Dropable))]
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
            GetComponent<Dropable>().OnDropEvent.AddListener(onDrop);
        }

        void Update()
        {
            //依据item数量调节覆盖程度
            Vector2 spacing = layout.spacing;
            if (transform.childCount > noOverLayCnt)
                spacing.x = (layoutSizeX - layoutCellSizeX * transform.childCount) / (transform.childCount - 1);
            else
                spacing.x = 0;

            layout.spacing = spacing;
        }

        private void onDrop(GameObject go, PointerEventData d)
        {
            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, d.position, d.enterEventCamera, out localPos))
            {
                var cnt = transform.childCount;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).localPosition.x >= localPos.x)
                    {
                        cnt = i;
                        break;
                    }
                }
                go.transform.parent = transform;
                go.transform.SetSiblingIndex(cnt);

                BattleData.Instance.MainPlayer.hands.Clear();
                for (int i = 0; i < transform.childCount; i++)
                    BattleData.Instance.MainPlayer.hands.Add(transform.GetChild(i).GetComponent<CardUI>().Card.ID);
            }
        }
    }
}


