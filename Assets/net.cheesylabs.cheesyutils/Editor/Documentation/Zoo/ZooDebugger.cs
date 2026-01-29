using System.Collections.Generic;
using UnityEngine;

namespace CheesyUtils.Editor.Documentation
{
    public static class ZooDebugger
    {
        /// <summary>
        /// Validates a GameObject to ensure it meets the expected criteria for a zoo asset.
        /// </summary>
        /// <param name="instance">The GameObject to validate.</param>
        /// <param name="errors">A list to capture descriptive error messages if validation fails.</param>
        /// <returns>
        /// Returns true if the validation encounters any errors; otherwise, returns false.
        /// </returns>
        public static bool ValidateZooItem(GameObject instance, out List<string> errors)
        {
            errors = new List<string>();

            MeshFilter mf = instance.GetComponentInChildren<MeshFilter>();
            SkinnedMeshRenderer smr = instance.GetComponentInChildren<SkinnedMeshRenderer>();
            
            if (!mf && !smr)
            {
                errors.Add("Missing Mesh Filter/Renderer");
            }
            else if (mf && !mf.sharedMesh)
            {
                errors.Add("Mesh Filter has no Mesh assigned");
            }

            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (!mat)
                    {
                        errors.Add($"Null Material on {r.name}");
                    }
                    else if (mat.shader.name == "Hidden/InternalErrorShader" || mat.color == Color.magenta)
                    {
                        errors.Add($"Shader Error (Pink) on {r.name}");
                    }
                }
            }

            Bounds b = GetBounds(instance);
            if (b.size.magnitude < 0.001f)
            {
                errors.Add("Object is suspiciously small (Zero Size?)");
            }

            return errors.Count > 0;
        }

        public static void AttachErrorVisuals(GameObject target, List<string> errors)
        {
            Transform existing = target.transform.Find("Debug_Error_Flag");
            if (existing) Object.DestroyImmediate(existing.gameObject);

            GameObject flag = new GameObject("Debug_Error_Flag");
            flag.transform.SetParent(target.transform);
            flag.transform.localPosition = new Vector3(0, GetBounds(target).max.y + 1.0f, 0);

            GameObject icon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            icon.name = "Icon";
            icon.transform.SetParent(flag.transform);
            icon.transform.localPosition = Vector3.zero;
            icon.transform.localScale = Vector3.one * 0.3f;
            
            Material redMat = new Material(Shader.Find("GUI/Text Shader"))
            {
                color = Color.red
            };
            icon.GetComponent<Renderer>().material = redMat;
            Object.DestroyImmediate(icon.GetComponent<Collider>());

            GameObject textObj = new GameObject("Error_Text");
            textObj.transform.SetParent(flag.transform);
            textObj.transform.localPosition = new Vector3(0, 0.4f, 0);
            
            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.text = string.Join("\n", errors);
            tm.characterSize = 0.1f;
            tm.fontSize = 40;
            tm.color = Color.red;
            tm.anchor = TextAnchor.LowerCenter;
            tm.alignment = TextAlignment.Center;
            
            textObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private static Bounds GetBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.zero);
            Bounds b = renderers[0].bounds;
            foreach (Renderer r in renderers) b.Encapsulate(r.bounds);
            return b;
        }
    }
}