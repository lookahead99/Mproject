/*
    导入文件时，预处理一下 --
 */
using UnityEditor;
using UnityEngine;

public  class AssetSetting : AssetPostprocessor
{
    // 预处理 图片
    void OnPreprocessTexture()
    {
        string texturePath = assetPath.Replace("\\", "/");
       // Debug.LogError("texturePath "+ texturePath);
        if (texturePath.Contains("/UI/Sprite/")) // ui
        {
            int index = texturePath.LastIndexOf("/");
            string folderPath = texturePath.Substring(0, index);
          //  Debug.LogError("folderPath " + folderPath);
            index = folderPath.LastIndexOf("/");
            string lastFolderName = folderPath.Substring(index+1);
            //Debug.LogError("lastFolderName " + lastFolderName);


            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePackingTag = lastFolderName;
            textureImporter.mipmapEnabled = false;

            // 如果需要特许的 sprite 再在这里加设置吧
        }
    }

}
