#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlexiSprite
{
    public static class AnimSpriteDemoPackageImporter
    {
        const string ImportKeyPrefix = "FlexiSprite.DemoPackageImported.v1";

        public readonly struct ImportResult
        {
            public readonly int Found;
            public readonly int Imported;
            public readonly int Skipped;
            public readonly string Message;

            public ImportResult(int found, int imported, int skipped, string message)
            {
                Found = found;
                Imported = imported;
                Skipped = skipped;
                Message = message;
            }
        }

        public static ImportResult ImportForCurrentPipeline(bool forceReimport = false)
        {
            return ImportForPipeline(AnimSpriteProjectSettings.DetectPipeline(), forceReimport);
        }

        public static ImportResult ImportForPipeline(RenderPipelineTarget pipeline, bool forceReimport = false)
        {
            var packages = FindMatchingDemoPackages(pipeline).ToList();

            if (packages.Count == 0)
            {
                return new ImportResult(
                    0,
                    0,
                    0,
                    $"No {pipeline} demo .unitypackage installers were found. " +
                    "Expected them somewhere under an Assets/**/Scenes folder, for example Assets/AnimatedSprite/Scenes/.");
            }

            int imported = 0;
            int skipped = 0;
            var importedNames = new List<string>();
            var skippedNames = new List<string>();

            try
            {
                for (int i = 0; i < packages.Count; i++)
                {
                    string packagePath = packages[i];
                    string packageName = Path.GetFileName(packagePath);
                    string key = GetImportKey(pipeline, packagePath);

                    if (!forceReimport && EditorPrefs.GetBool(key, false))
                    {
                        skipped++;
                        skippedNames.Add(packageName);
                        continue;
                    }

                    EditorUtility.DisplayProgressBar(
                        "Importing FlexiSprite demo scenes",
                        $"Importing {packageName}",
                        packages.Count == 0 ? 1f : (float)i / packages.Count);

                    AssetDatabase.ImportPackage(packagePath, false);
                    EditorPrefs.SetBool(key, true);
                    imported++;
                    importedNames.Add(packageName);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }

            string message = BuildMessage(pipeline, packages.Count, imported, skipped, importedNames, skippedNames);
            Debug.Log("[FlexiSprite] " + message);
            return new ImportResult(packages.Count, imported, skipped, message);
        }

        static IEnumerable<string> FindMatchingDemoPackages(RenderPipelineTarget pipeline)
        {
            string assetsPath = Application.dataPath.Replace('\\', '/');

            if (!Directory.Exists(assetsPath))
                yield break;

            var packages = Directory
                .GetFiles(assetsPath, "*.unitypackage", SearchOption.AllDirectories)
                .Select(path => Path.GetFullPath(path).Replace('\\', '/'))
                .Where(path => IsInLikelyScenesFolder(path) && IsPackageForPipeline(path, pipeline))
                .OrderBy(path => GetPackageSortScore(path, pipeline))
                .ThenBy(path => path, StringComparer.OrdinalIgnoreCase);

            foreach (string package in packages)
                yield return package;
        }

        static bool IsInLikelyScenesFolder(string packagePath)
        {
            string normalized = packagePath.Replace('\\', '/').ToLowerInvariant();
            return normalized.Contains("/scenes/") || normalized.EndsWith("/scenes.unitypackage");
        }

        static bool IsPackageForPipeline(string path, RenderPipelineTarget pipeline)
        {
            string name = Normalize(Path.GetFileNameWithoutExtension(path));

            bool mentionsURP = name.Contains("urp") || name.Contains("universalrenderpipeline");
            bool mentionsHDRP = name.Contains("hdrp") || name.Contains("highdefinitionrenderpipeline");
            bool mentionsBuiltIn = name.Contains("buildin") || name.Contains("builtin") || name.Contains("builtinrp") || name.Contains("birp");

            return pipeline switch
            {
                RenderPipelineTarget.URP => mentionsURP,
                RenderPipelineTarget.HDRP => mentionsHDRP,
                _ => mentionsBuiltIn && !mentionsURP && !mentionsHDRP
            };
        }

        static int GetPackageSortScore(string path, RenderPipelineTarget pipeline)
        {
            string name = Normalize(Path.GetFileNameWithoutExtension(path));

            if (pipeline == RenderPipelineTarget.URP)
            {
                if (name.Contains("2d") || name.Contains("twod") || name.Contains("light"))
                    return 1;

                return 0;
            }

            return 0;
        }

        static string Normalize(string value)
        {
            var chars = value
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray();

            return new string(chars);
        }

        static string BuildMessage(
            RenderPipelineTarget pipeline,
            int found,
            int imported,
            int skipped,
            List<string> importedNames,
            List<string> skippedNames)
        {
            string message = $"Demo package import for {pipeline}: found {found}, imported {imported}, skipped {skipped}.";

            if (importedNames.Count > 0)
                message += "\nImported:\n- " + string.Join("\n- ", importedNames);

            if (skippedNames.Count > 0)
                message += "\nAlready imported:\n- " + string.Join("\n- ", skippedNames);

            return message;
        }

        static string GetImportKey(RenderPipelineTarget pipeline, string packagePath)
        {
            var info = new FileInfo(packagePath);
            string stamp = info.Exists ? $"{info.Length}_{info.LastWriteTimeUtc.Ticks}" : "missing";
            string raw = Application.dataPath + "|" + pipeline + "|" + packagePath + "|" + stamp;
            return ImportKeyPrefix + "." + StableHash(raw);
        }

        static int StableHash(string value)
        {
            unchecked
            {
                int hash = 23;

                for (int i = 0; i < value.Length; i++)
                    hash = hash * 31 + value[i];

                return hash;
            }
        }
    }
}
#endif
