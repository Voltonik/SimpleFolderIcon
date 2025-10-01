using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    [InitializeOnLoad]
    public class CustomFolderLoader
    {
        static CustomFolderLoader()
        {
            EditorApplication.delayCall += Initialize;
        }

        static void Initialize()
        {
            EditorApplication.delayCall -= Initialize;
            IconDictionaryCreator.BuildDictionary();
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
        }

        static string FindDeepestMatchingParentKey(string folderPath, Dictionary<string, Texture> iconDictionary)
        {
            var normalizedPath = folderPath.Replace("\\", "/");
            var pathParts = normalizedPath.Split('/');

            for (int i = pathParts.Length - 1; i >= 0; i--)
            {
                var folderName = pathParts[i].ToLowerInvariant();
                if (!string.IsNullOrEmpty(folderName) && iconDictionary.ContainsKey(folderName))
                {
                    return folderName;
                }
            }

            return null;
        }

        static void DrawFolderIcon(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var iconDictionary = IconDictionaryCreator.IconDictionary;

            if (path == "" ||
                Event.current.type != EventType.Repaint ||
                !File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                return;
            }

            var matchingKey = FindDeepestMatchingParentKey(path, iconDictionary);
            if (string.IsNullOrEmpty(matchingKey))
            {
                return;
            }

            Rect imageRect;

            if (rect.height > 20)
            {
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.width + 2);
            }
            else if (rect.x > 20)
            {
                imageRect = new Rect(rect.x - 1, rect.y - 1, rect.height + 2, rect.height + 2);
            }
            else
            {
                imageRect = new Rect(rect.x + 2, rect.y - 1, rect.height + 2, rect.height + 2);
            }

            var texture = IconDictionaryCreator.IconDictionary[matchingKey];
            if (texture == null)
            {
                return;
            }

            GUI.DrawTexture(imageRect, texture);
        }
    }
}
