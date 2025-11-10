using UnityEngine.AddressableAssets;

namespace CheesyUtils.PackagedAssets
{
    public static class AddressablesExtensions
    {
        /// <summary>
        /// Retrieves a new AssetReferenceSprite based on the provided sprite reference.
        /// If the input sprite reference contains a valid AssetGUID, a new AssetReferenceSprite
        /// is created using the GUID and its SubObjectName is copied if specified.
        /// </summary>
        /// <param name="spriteReference">The source AssetReferenceSprite to retrieve the asset reference from.</param>
        /// <returns>
        /// A new AssetReferenceSprite instance if the AssetGUID is valid. Returns null if no valid AssetGUID is found.
        /// </returns>
        public static AssetReferenceSprite GetAssetReferenceSpriteFromObject(AssetReferenceSprite spriteReference)
        {
            if (spriteReference != null && !string.IsNullOrEmpty(spriteReference.AssetGUID))
            {
                // Create or update the AssetReference with the GUID from the current page's icon
                AssetReferenceSprite asset = new AssetReferenceSprite(spriteReference.AssetGUID);
                
                // If you need to handle the SubObject name (for sprite sheets)
                if (!string.IsNullOrEmpty(spriteReference.SubObjectName))
                {
                    asset.SubObjectName = spriteReference.SubObjectName;
                    return asset;
                }
            }
            else
            {
                Debug.LogWarning("No icon data found for current page.");
            }
            return null;
        }
    }
}