using System.Collections.Generic;
using UnityEngine;

namespace CheesyUtils
{
    public static class MeshExtensions
    {
        private static readonly int DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");
        private static readonly int DetailAlbedoMapScale = Shader.PropertyToID("_DetailAlbedoMapScale");
        
        public static void SetMaterial(this MeshRenderer renderer, int index, Material material)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            materials[index] = material;
            renderer.sharedMaterials = materials.ToArray();
        }
        
        public static void SetAllMaterials(this MeshRenderer renderer, Material material)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i] = material;
            }
            renderer.sharedMaterials = materials.ToArray();
        }
        
        public static void SetDetailTexture(this MeshRenderer renderer, Texture detailTexture, float scale)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            foreach (Material material in materials)
            {
                material.SetTextureScale(DetailAlbedoMap, new Vector2(2.0f, 2.0f));
                material.SetTexture(DetailAlbedoMap, detailTexture);
                material.SetFloat(DetailAlbedoMapScale, scale);
            }
            renderer.sharedMaterials = materials.ToArray();
        }
    }
}