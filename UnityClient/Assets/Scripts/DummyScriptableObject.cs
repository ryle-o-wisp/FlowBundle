using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Dummy Scriptable Object")]
    public class DummyScriptableObject : ScriptableObject
    {
        public string dummyText;
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(DummyScriptableObject))]
        public sealed class Inspector : Editor 
        {
            public override void OnInspectorGUI()
            {
                var obj = target as DummyScriptableObject;
                var rand = new System.Random(0);
                if (obj != null)
                {
                    var newLength = EditorGUILayout.IntField("Dummy Text Length", obj.dummyText?.Length ?? 0);
                    if (newLength != obj.dummyText?.Length)
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < newLength; i++)
                        {
                            sb.Append('A'+rand.Next('Z'-'A'));
                        }
                        obj.dummyText = sb.ToString();
                        EditorUtility.SetDirty(obj);
                    }
                }
            }
        }
        #endif
    }
}