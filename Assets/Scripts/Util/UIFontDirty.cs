using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class UIFontDirty : MonoBehaviour
    {
        bool isDirty = false;
        Font dirtyFont = null;

        void Awake()
        {
            Font.textureRebuilt += f =>
            {
                isDirty = true;
                dirtyFont = f;
            };
        }

        void LateUpdate()
        {
            if (isDirty)
            {
                isDirty = false;
                foreach (Text text in FindObjectsOfType<Text>())
                {
                    if (text.font == dirtyFont)
                    {
                        text.FontTextureChanged();
                    }
                }
                dirtyFont = null;
            }
        }
    }
}


