using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
 
 #if UNITY_IOS
public class CopyXcodeLaunchImageOnPostProcess
{
    const string XCODE_IMAGES_FOLDER = "Unity-iPhone/Images.xcassets";
    const string SOURCE_FOLDER_NAME = "AppLogo.imageset";
    const string SOURCE_FOLDER_ROOT = "Storyboard/ImageScanner/ImageScanner/Assets.xcassets";
 
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string sourcePath = $"{SOURCE_FOLDER_ROOT}/{SOURCE_FOLDER_NAME}";
            string targetPath = $"{path}/{XCODE_IMAGES_FOLDER}/{SOURCE_FOLDER_NAME}";
 
            FileUtil.DeleteFileOrDirectory(targetPath);
            FileUtil.CopyFileOrDirectory(sourcePath, targetPath);
        }
    }
}
#endif