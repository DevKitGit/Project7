using UnityEngine;
using Object = UnityEngine.Object;

namespace Devkit.Modularis.Helpers
{
#if UNITY_EDITOR
    using UnityEditor;
 
    static class SOPlayModeResetter
    {
        [InitializeOnLoadMethod]
        static void RegisterResets()
        {
            EditorApplication.playModeStateChanged += ResetSOsWithIResetOnExitPlay; 
        }
        
        static void ResetSOsWithIResetOnExitPlay(PlayModeStateChange change)
        {
            ScriptableObject[] assets;
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    EditorApplication.playModeStateChanged -= ResetSOsWithIResetOnExitPlay;
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    return;
                case PlayModeStateChange.ExitingEditMode:
                    assets = FindAssets<ScriptableObject>();
                    foreach (var a in assets)
                    {
                        if (a is not IResetOnExitPlay play) continue;
                        play.StoreOnEnterPlay();
                    }
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    assets = FindAssets<ScriptableObject>();
                    foreach (var a in assets)
                    {
                        if (a is not IResetOnExitPlay play) continue;
                        play.ResetOnExitPlay(play.ShouldResetOnExitPlay());
                    }
                    break;
            }
        }
 
        static T[] FindAssets<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            var assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);            
            }
            return assets;
        }
    }
#endif
 
    public interface IResetOnExitPlay
    {
        public bool ShouldResetOnExitPlay();
        public void StoreOnEnterPlay();
        public void ResetOnExitPlay(bool reset);
    }
}