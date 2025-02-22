using UnityEngine;

namespace CheesyUtils
{
    public static class MeshExtensions
    {
        public static void SetMaterials(this MeshRenderer renderer, int index, Material material)
        {
            var materials = renderer.materials;
            materials[index] = material;
            renderer.materials = materials;
        }
    }
}