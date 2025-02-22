using UnityEditor;
using UnityEngine;
// using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace CheesyUtils.EditorTools
{
    public static class TranslatableSprite
    {
        // Menu item to create a new TMP_Text object with custom behavior
        [MenuItem("GameObject/UI/Translatable Sprite", false, 10)]
        public static void CreateTranslatableSprite()
        {
            // Create a new GameObject
            GameObject newObject = new GameObject("TranslatableSprite");
            RectTransform rectTransform = newObject.AddComponent<RectTransform>();
            Image image = newObject.AddComponent<Image>();
            // LocalizeSpriteEvent localizeSpriteEvent = newObject.AddComponent<LocalizeSpriteEvent>();
            
            // Automatically parent to the selected GameObject in the hierarchy
            GameObject parent = Selection.activeGameObject;
            if (parent != null)
            {
                newObject.transform.SetParent(parent.transform, false);
            }

            // Select the newly created object
            Selection.activeGameObject = newObject;
        }
    }
}
