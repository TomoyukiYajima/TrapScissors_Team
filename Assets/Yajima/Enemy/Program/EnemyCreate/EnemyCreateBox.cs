using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyCreateBox : MonoBehaviour {

    //[SerializeField]
    //private int m_CreateCount = 3;              // 生成数
    //[SerializeField]
    //private float m_CreateTime = 2.0f;          // 生成間隔
    //[SerializeField]
    //private float m_CreateLength = 10.0f;       // 生成する距離
    [SerializeField]
    private Transform[] m_MovePoints = null;    // 移動用ポイント配列
    [SerializeField]
    private GameObject m_CreateEnemy;           // 生成用オブジェクト
    [SerializeField]
    private GameObject m_MainCamera;            // メインカメラ
    [SerializeField]
    private bool m_IsStartCreate = false;       // ゲーム開始時に生成するか

    private int m_CreateCount = 1;              // 生成数
    private float m_Timer = 0.0f;               // 経過時間
    private bool m_IsRendered = true;           // カメラ内か
    private AnimalManager m_AnimalManager;      // 動物マネージャー
    //private GameObject m_ChildObject;           // 生成した子供オブジェクト

    // Use this for initialization
    void Start () {
        // メインカメラがなかった場合は、探す
        if (m_MainCamera == null)
        {
            var camera = GameObject.Find("Main Camera");
            if (camera == null) return;
            m_MainCamera = camera;
        }

        // 動物マネージャーの取得
        var obj = this.transform.parent.gameObject;
        //if(obj != null)
        //{
        //    SetAnimalManager(obj);
        //}
        //else 
        if (obj == null)
        {
            var enemiesObject = GameObject.Find("Enemies");
            if (enemiesObject == null) return;
            obj = enemiesObject;
            //var 
        }
        SetAnimalManager(obj);

        // 開始時に生成しない場合は返す
        if (!m_IsStartCreate) return;
        // 敵の生成
        CreateEnemy();

        //if (m_CreateEnemy == null) return;
        //var enemy = m_CreateEnemy.GetComponent<Enemy3D>();
        //if (enemy == null) return;
        ////m_Enemy = enemy;
        //if (m_MovePoints.Length == 0) return;
        //for(int i = 0; i != m_MovePoints.Length; i++)
        //{
        //    enemy.AddMovePoint(m_MovePoints[i]);
        //}
        //m_Enemy.AddMovePoint();
        //enemy.
    }
	
	// Update is called once per frame
	void Update () {
        m_Timer += Time.deltaTime;

        if (!m_IsRendered)
        {

            // 条件を満たしたら、生成
            //if (m_Timer >= m_CreateTime &&
            //    this.transform.childCount < m_CreateCount && 
            //    !m_IsRendered)
            // 生成数に達していなかったら、動物を生成
            if (this.transform.childCount < m_CreateCount)
            {
                // 敵の生成
                CreateEnemy();
                m_Timer = 0.0f;
            }

            //if(m_ChildObject == null)
            //{
            //    // 子供オブジェクトに追加
            //    var child = this.transform.FindChild(m_CreateEnemy.name);
            //    if (child == null) return;
            //    m_ChildObject = child.gameObject;
            //}

            // 子オブジェクトが非アクティブ状態なら、アクティブ状態に変更
            //if (m_ChildObject.activeSelf) return;
            //m_ChildObject.SetActive(true);
            if (m_CreateEnemy.activeSelf) return;
            m_CreateEnemy.SetActive(true);
        }

        m_IsRendered = false;
    }

    // 敵の生成
    private void CreateEnemy()
    {
        //Enemy3D enemy = m_CreateEnemy.GetComponentInChildren<Enemy3D>();
        //if (enemy == null) return;
        ////if (m_MovePoints.Length == 0) return;
        //enemy.ResizeMovePoints(m_MovePoints.Length);
        //for (int i = 0; i != m_MovePoints.Length; i++)
        //{
        //    enemy.AddMovePoint(i, m_MovePoints[i]);
        //}

        // 生成して、子オブジェクトにする
        //Instantiate(
        //    m_CreateEnemy, this.transform.position,
        //    this.transform.rotation, this.transform
        //    );
        // 生成カウントに加算
        m_AnimalManager.AddAnimalCount(1);

        //// 子供オブジェクトに追加
        //var child = this.transform.FindChild(m_CreateEnemy.name); // + "(Clone)");
        //if (child == null) return;
        //m_ChildObject = child.gameObject;
    }

    private void SetAnimalManager(GameObject obj)
    {
        var manager = obj.GetComponent<AnimalManager>();
        if (manager == null) return;
        m_AnimalManager = manager;
    }

    public void OnWillRenderObject()
    {
        if (m_MainCamera.tag != "MainCamera") return;
        // 見えている
        m_IsRendered = true;
    }

    // 移動ポイント配列のポイントを取得します
    public Transform GetMovePoint(int num) { return m_MovePoints[num]; }

    // 移動ポイント配列のサイズを取得します
    public int GetMovePointsSize() { return m_MovePoints.Length; }

    #region エディターのシリアライズ変更
    // 変数名を日本語に変換する機能
    // CustomEditor(typeof(Enemy), true)
    // 継承したいクラス, trueにすることで、子オブジェクトにも反映される
#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyCreateBox), true)]
    [CanEditMultipleObjects]
    public class EnemyCreateBoxEditor : Editor
    {
        //SerializedProperty CreateCount;
        //SerializedProperty CreateTime;
        //SerializedProperty CreateLength;
        SerializedProperty IsStartCreate;
        SerializedProperty CreateEnemy;
        SerializedProperty MainCamera;
        SerializedProperty MovePoints;

        public void OnEnable()
        {
            //CreateCount = serializedObject.FindProperty("m_CreateCount");
            //CreateTime = serializedObject.FindProperty("m_CreateTime");
            //CreateLength = serializedObject.FindProperty("m_CreateLength");
            IsStartCreate = serializedObject.FindProperty("m_IsStartCreate");
            CreateEnemy = serializedObject.FindProperty("m_CreateEnemy");
            MainCamera = serializedObject.FindProperty("m_MainCamera");
            MovePoints = serializedObject.FindProperty("m_MovePoints");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            // 必ず書く
            serializedObject.Update();

            EnemyCreateBox createBox = target as EnemyCreateBox;

            //// int
            //CreateCount.intValue = EditorGUILayout.IntField("敵の最大生成数", createBox.m_CreateCount);

            //// float
            //CreateTime.floatValue = EditorGUILayout.FloatField("敵の生成間隔(秒)", createBox.m_CreateTime);
            //CreateLength.floatValue = EditorGUILayout.FloatField("敵の生成距離(m)", createBox.m_CreateLength);
            // bool
            IsStartCreate.boolValue = EditorGUILayout.Toggle("ゲーム開始時に生成", createBox.m_IsStartCreate);

            EditorGUILayout.Space();
            // Transform
            CreateEnemy.objectReferenceValue = EditorGUILayout.ObjectField("生成する敵の種類", createBox.m_CreateEnemy, typeof(GameObject), true);
            MainCamera.objectReferenceValue = EditorGUILayout.ObjectField("メインカメラ", createBox.m_MainCamera, typeof(GameObject), true);
            // 配列
            EditorGUILayout.PropertyField(MovePoints, new GUIContent("動物の徘徊ポイント"), true);
            //EditorGUILayout.Space();
            //// WallChackPoint
            //WChackPoint.objectReferenceValue = EditorGUILayout.ObjectField("壁捜索ポイント", enemy.m_WChackPoint, typeof(WallChackPoint), true);

            // Unity画面での変更を更新する(これがないとUnity画面で変更できなくなる)
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    #endregion
}
