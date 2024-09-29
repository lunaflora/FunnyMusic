using UnityEditor;
using UnityEngine;
using System;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class CustomToolbarExtend
    {
        private static readonly Type kToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject sCurrentToolbar;
        
        static CustomToolbarExtend()
        {
            // EditorApplication.update += OnUpdate;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }
        
        static void OnToolbarGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                GUILayout.FlexibleSpace();

                //自定义按钮加在此处
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("QuickPlay", EditorGUIUtility.FindTexture("PlayButton"), "从启动页启动游戏")))
                {
                    // 这里开启游戏
                    UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/GameScenes/Init.unity");
                    EditorApplication.EnterPlaymode();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    
}