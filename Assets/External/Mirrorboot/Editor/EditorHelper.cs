﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AriaSDK.MirrorBoot.Editor
{

    /// <summary>
    ///
    /// </summary>
    internal static class EditorHelper
    {
#if UNITY_EDITOR_WIN
        const string FILE_EXTENSION = "bat";
#else // UNITY_EDITOR_OSX ||
        const string FILE_EXTENSION = "sh";
#endif
        const string FILE_NAME = "setupSynchronizableProject";

        const string MENU_PATH = "AriaSDK/Tools/GenerateSynchronizableProject";

        private static string GetScriptPath()
        {
            //var results = Directory.GetFiles("./", "*.sh", SearchOption.AllDirectories)
            var results = Directory.GetFiles(Directory.GetCurrentDirectory(), $"*.{FILE_EXTENSION}", SearchOption.AllDirectories)
                .Where(x => x.Contains($"{FILE_NAME}"))
                .ToList();

            //var results = AssetDatabase.FindAssets($"")
            //	.Select(x => AssetDatabase.GUIDToAssetPath(x))
            //	.Where(x => x.Contains($"{FILE_NAME}"))
            //	//.Select(x => FileUtil.GetProjectRelativePath(x))
            //	.ToList();

            return (results.Count > 0) ? results[0].Replace($"{FILE_NAME}.{FILE_EXTENSION}", ""): null;
        }

        struct UnityAppInfo
        {
            public string path;
            public string exec;
        }

        private static UnityAppInfo CreateUnityAppInfo()
        {
            string unityAppPath = $"{EditorApplication.applicationPath}";

            return new UnityAppInfo{
                path = Path.GetDirectoryName(unityAppPath),
                exec = Path.GetFileName(unityAppPath)
            #if UNITY_EDITOR_OSX
                    + "/Contents/MacOS/Unity"
            #endif
            };
        }

        [UnityEditor.MenuItem(MENU_PATH)]
        private static void GenerateSynchronizableProject()
        {
            var path = GetScriptPath();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var shellRunner = new BatchRunner($"{path}", $"{FILE_NAME}.{FILE_EXTENSION}");
            {
                string log = shellRunner.Run();
                Debug.Log(log);
            }

            var unityAppInfo = CreateUnityAppInfo();

            var unityCmdParam = new System.Diagnostics.ProcessStartInfo{
                FileName = $"{unityAppInfo.path}/{unityAppInfo.exec}",
                Arguments = $"-projectPath ../Synchronizable{Application.productName}"
            };
            System.Diagnostics.Process.Start(unityCmdParam);

            // [todo] wait for exit.
            //var unityCommandline = new BatchRunner(unityAppInfo.path, unityAppInfo.exec);
            //{
            //	string log = unityCommandline.Run($"-projectPath ../Synchronizable{Application.productName}");
            //	Debug.Log(log);
            //}
        }
    }
}
