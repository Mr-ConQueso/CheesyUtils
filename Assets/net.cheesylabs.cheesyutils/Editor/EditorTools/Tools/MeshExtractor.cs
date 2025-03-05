using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CheesyUtils.EditorTools
{
    public class MeshExtractor : MonoBehaviour
    {
        [MenuItem("Assets/Extract Textures & Materials", false, 30)]
        private static void ExtractMaterialsAndTextures()
        {
            var selected = Selection.activeObject;
            if (selected == null) return;

            string assetPath = AssetDatabase.GetAssetPath(selected);
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null) return;

            ProcessFBX(assetPath, selected.name);
        }

        private static void ProcessFBX(string originalPath, string originalName)
        {
            // Handle existing SM_ prefix
            string cleanName = originalName.StartsWith("SM_") 
                ? originalName.Substring(3) 
                : originalName;

            // Create root folder
            string parentDir = Path.GetDirectoryName(originalPath);
            string meshFolder = Path.Combine(parentDir, cleanName);
            AssetDatabase.CreateFolder(parentDir, cleanName);

            // Create materials folder
            CreateSubfolder(meshFolder, "Materials");

            // Move FBX (only rename if needed)
            string newFBXName = originalName.StartsWith("SM_") 
                ? originalName + ".fbx" 
                : $"SM_{cleanName}.fbx";
            string newFBXPath = MoveAsset(originalPath, meshFolder, newFBXName);

            // Process assets
            bool hasEmbeddedTextures = ExtractSubAssets(newFBXPath, meshFolder);
            ProcessExternalDependencies(newFBXPath, meshFolder, hasEmbeddedTextures);

            // Configure FBX import settings
            ConfigureModelImporter(newFBXPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ConfigureModelImporter(string fbxPath)
        {
            ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
            if (importer == null) return;

            // Set material settings
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.materialSearch = ModelImporterMaterialSearch.Local;
            
            // Set texture settings
            // importer.textureNameToMaterialSearch = ModelImporterTextureNameToMaterialSearch.FromModel;
            // importer.extractTextures = false;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        private static void CreateSubfolder(string parent, string folderName)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parent, folderName)))
                AssetDatabase.CreateFolder(parent, folderName);
        }

        private static string MoveAsset(string originalPath, string newFolder, string newName)
        {
            string newPath = Path.Combine(newFolder, newName);
            try
            {
                AssetDatabase.MoveAsset(originalPath, newPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to move {originalPath} to {newPath}");
                Console.WriteLine(e);
                throw;
            }
            return newPath;
        }

        private static bool ExtractSubAssets(string fbxPath, string meshFolder)
        {
            bool hasTextures = false;
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxPath);
            foreach (var asset in assets)
            {
                if (asset is Material mat)
                {
                    CreateMaterialAsset(mat, meshFolder);
                }
                else if (asset is Texture2D tex)
                {
                    CreateTextureAsset(tex, meshFolder);
                    hasTextures = true;
                }
            }
            return hasTextures;
        }

        private static void CreateMaterialAsset(Material original, string meshFolder)
        {
            string baseName = original.name.StartsWith("M_") 
                ? original.name 
                : $"M_{original.name}";
            
            string materialsFolder = Path.Combine(meshFolder, "Materials");
            string newPath = Path.Combine(materialsFolder, $"{baseName}.mat");
            
            if (!File.Exists(newPath))
            {
                Material newMat = new Material(original);
                AssetDatabase.CreateAsset(newMat, newPath);
            }
        }

        private static void CreateTextureAsset(Texture2D original, string meshFolder)
        {
            string texturesFolder = Path.Combine(meshFolder, "Textures");
            if (!AssetDatabase.IsValidFolder(texturesFolder))
                AssetDatabase.CreateFolder(meshFolder, "Textures");

            string baseName = original.name.StartsWith("T_") 
                ? original.name 
                : $"T_{original.name}";
            
            string newPath = Path.Combine(texturesFolder, $"{baseName}.png");
            
            if (!File.Exists(newPath))
            {
                byte[] pngData = original.EncodeToPNG();
                File.WriteAllBytes(newPath, pngData);
                AssetDatabase.ImportAsset(newPath);
            }
        }

        private static void ProcessExternalDependencies(string fbxPath, string meshFolder, bool hasEmbeddedTextures)
        {
            string[] dependencies = AssetDatabase.GetDependencies(fbxPath);
            foreach (string dependency in dependencies)
            {
                if (dependency == fbxPath) continue;

                string fileName = Path.GetFileNameWithoutExtension(dependency);
                string ext = Path.GetExtension(dependency).ToLower();
                
                // Skip .fbm folder contents
                if (dependency.Contains(".fbm")) continue;

                string newName = GetPrefixedName(fileName, ext);
                string targetFolder = GetTargetFolder(ext, meshFolder, hasEmbeddedTextures);

                if (targetFolder != null && AssetDatabase.IsValidFolder(targetFolder))
                {
                    string newPath = Path.Combine(targetFolder, newName);
                    if (!File.Exists(newPath))
                        AssetDatabase.MoveAsset(dependency, newPath);
                }
            }
        }

        private static string GetPrefixedName(string original, string extension)
        {
            return extension switch
            {
                ".mat" => original.StartsWith("M_") ? $"{original}.mat" : $"M_{original}.mat",
                ".png" or ".jpg" or ".jpeg" or ".tga" => original.StartsWith("T_") 
                    ? $"{original}{extension}" 
                    : $"T_{original}{extension}",
                _ => original + extension
            };
        }

        private static string GetTargetFolder(string extension, string meshFolder, bool hasEmbeddedTextures)
        {
            return extension switch
            {
                ".mat" => Path.Combine(meshFolder, "Materials"),
                ".png" or ".jpg" or ".jpeg" or ".tga" => hasEmbeddedTextures 
                    ? Path.Combine(meshFolder, "Textures") 
                    : null,
                _ => null
            };
        }
    }
}