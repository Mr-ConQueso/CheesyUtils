using System.Collections.Generic;
using UnityEngine;

namespace CheesyUtils
{
    public static class MeshExtensions
    {
        private static readonly int DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");
        private static readonly int DetailAlbedoMapScale = Shader.PropertyToID("_DetailAlbedoMapScale");
        
        public static void SetMaterialAtIndex(this MeshRenderer renderer, int index, Material material)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            materials[index] = material;
            renderer.SetSharedMaterials(materials);
        }
        
        public static void SetAllMaterials(this MeshRenderer renderer, Material material)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i] = material;
            }
            renderer.SetSharedMaterials(materials);
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
            renderer.SetSharedMaterials(materials);
        }
        
        public static List<Color> GetColorsToList(this Mesh mesh)
        {
            List<Color> colors = new List<Color>();
            mesh.GetColors(colors);
            return colors;
        }
        
        public static Color[] GetColorsToArray(this Mesh mesh)
        {
            List<Color> colors = new List<Color>();
            mesh.GetColors(colors);
            return colors.ToArray();
        }
        
        public static Color32[] GetColors32ToArray(this Mesh mesh)
        {
            List<Color32> colors = new List<Color32>();
            mesh.GetColors(colors);
            return colors.ToArray();
        }
        
        public static List<Color32> GetColors32ToList(this Mesh mesh)
        {
            List<Color32> colors = new List<Color32>();
            mesh.GetColors(colors);
            return colors;
        }
        
        public static Vector3[] GetNormalsToArray(this Mesh mesh)
        {
            List<Vector3> normals = new List<Vector3>();
            mesh.GetNormals(normals);
            return normals.ToArray();
        }
        
        public static List<Vector3> GetNormalsToList(this Mesh mesh)
        {
            List<Vector3> normals = new List<Vector3>();
            mesh.GetNormals(normals);
            return normals;
        }
        
        public static Vector2[] GetUVsToArray(this Mesh mesh, int index)
        {
            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(index, uvs);
            return uvs.ToArray();
        }
        
        public static List<Vector2> GetUVsToList(this Mesh mesh, int index)
        {
            List<Vector2> uvs = new List<Vector2>();
            mesh.GetUVs(index, uvs);
            return uvs;
        }
        
        public static Vector3[] GetVerticesToArray(this Mesh mesh)
        {
            List<Vector3> vertices = new List<Vector3>();
            mesh.GetVertices(vertices);
            return vertices.ToArray();
        }
        
        public static List<Vector3> GetVerticesToList(this Mesh mesh)
        {
            List<Vector3> vertices = new List<Vector3>();
            mesh.GetVertices(vertices);
            return vertices;
        }
        
        public static int[] GetTrianglesToArray(this Mesh mesh)
        {
            List<int> triangles = new List<int>();
            mesh.GetTriangles(triangles, 0);
            return triangles.ToArray();
        }
        
        public static List<int> GetTrianglesToList(this Mesh mesh)
        {
            List<int> triangles = new List<int>();
            mesh.GetTriangles(triangles, 0);
            return triangles;
        }
        
        public static Vector4[] GetTangentsToArray(this Mesh mesh)
        {
            List<Vector4> tangents = new List<Vector4>();
            mesh.GetTangents(tangents);
            return tangents.ToArray();
        }
        
        public static List<Vector4> GetTangentsToList(this Mesh mesh)
        {
            List<Vector4> tangents = new List<Vector4>();
            mesh.GetTangents(tangents);
            return tangents;
        }
    }
}