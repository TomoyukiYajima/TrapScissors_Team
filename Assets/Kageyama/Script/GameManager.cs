using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    protected static GameManager _gameManager;

    #region ステージの環境設定
    [SerializeField]
    private int _trapNumber;
    #endregion

    //どこでも参照可
    public static GameManager gameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
                if (_gameManager == null)
                {
                    Debug.LogError("SceneChange Instance Error");
                }
            }

            return _gameManager;
        }
    }

    public int TrapNumber()
    {
        return _trapNumber;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class SceneManagerEditor : Editor
    {
        SerializedProperty TrapNumber;

        public void OnEnable()
        {
            TrapNumber = serializedObject.FindProperty("_trapNumber");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GameManager manager = target as GameManager;
            
            TrapNumber.intValue = EditorGUILayout.IntField("仕掛けられる罠の最大数", manager._trapNumber);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
