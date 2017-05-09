using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RabbitEnemy : Enemy3D
{

    [SerializeField]
    private float m_TurnLength = 1.0f;

    private float m_MoveLength = 0.0f;

    private List<Transform> m_BoxPoints = 
        new List<Transform>();              // 移動用ポイントコンテナ
    private Dictionary<Transform, int> m_ResultPoints =
        new Dictionary<Transform, int>(); // 移動ポイントの評価
    // Use this for initialization
    //protected override void Start()
    //{
    //    base.Start();
    //}

    //// Update is called once per frame
    //void Update () {

    //}

    #region override関数

    protected override void ReturnMove(float deltaTime, float subSpeed = 1.0f)
    {
        base.ReturnMove(deltaTime, subSpeed);
        // 移動距離の加算
        //m_MoveLength += Mathf.Abs(m_TotalVelocity.x) + Mathf.Abs(m_TotalVelocity.y) + Mathf.Abs(m_TotalVelocity.z);
    }

    protected override void TurnWall()
    {
        // 一定距離移動したら、折り返す
        if (m_MoveLength < m_TurnLength * 10) return;

        m_MoveLength = 0.0f;
        //base.TurnWall();
        // 角度の設定
        SetDegree();
    }

    public override void SoundNotice(Transform point)
    {
        var pointBox = GameObject.Find("MovePoints");
        // 移動ポイントコンテナがない場合は、
        // 自分の持っているポイントで移動する
        var length = 0.0f;
        var pointPos = point.position;
        var setPos = Vector3.zero;
        // 持っているポイントで、音の位置との最長距離を求める
        for (int i = 0; i != m_MovePoints.Length; i++)
        {
            var pos = m_MovePoints[i].position;
            var pointLength = Vector3.Distance(pointPos, pos);
            //var degree = Vector3.Angle(pointPos, pos);
            // 前回のポイントとの位置より長かったら,
            // 角度が一定角度より大きければ更新する
            //  && Mathf.Abs(degree) > 20.0f
            if (length < pointLength)
            {
                length = pointLength;
                setPos = pos;
            }
        }
        ChangeMovePoint(setPos);

        //if (pointBox == null)
        //{
        //    var length = 0.0f;
        //    var pointPos = point.position;
        //    // 持っているポイントで、音の位置との最長距離を求める
        //    for (int i = 0; i != m_MovePoints.Length; i++)
        //    {
        //        var pos = m_MovePoints[i].position;
        //        var pointLength = Vector3.Distance(pointPos, pos);
        //        var degree = Vector3.Angle(pointPos, pos);
        //        // 前回のポイントとの位置より長かったら,
        //        // 角度が一定角度より大きければ更新する
        //        if (length < pointLength && Mathf.Abs(degree) > 20.0f)
        //        {
        //            length = pointLength;
        //            m_MovePointPosition = pos;
        //        }
        //    }
        //}
        //else
        //{
        //    // 移動ポイントコンテナがある場合は、全ポイントを調べて
        //    // 移動ポイントを決める
        //    //pointBox.child
        //    int count = 0;
        //    foreach(Transform child in pointBox.transform)
        //    {
        //        m_BoxPoints.Add(child);
        //        m_ResultPoints[m_BoxPoints[count]] = count;
        //        count++;
        //    }
        //    // 取得したポイント全部との最長距離を取る
        //    //var length = 0.0f;
        //    for (int i = 0; i != m_BoxPoints.Count; i++)
        //    {
        //        var pos = m_BoxPoints[i].position;
        //        var pointLength = Vector3.Distance(point.position, pos);
        //        //// 前回のポイントとの位置より長かったら、更新する
        //        //if (length < pointLength)
        //        //{
        //        //    length = pointLength;
        //        //    m_MovePointPosition = pos;
        //        //}

        //        // 移動ポイントの評価

        //    }
        //}
        m_BoxPoints.Clear();
        m_ResultPoints.Clear();
    }
    #endregion

    #region シリアライズ変更
#if UNITY_EDITOR
    [CustomEditor(typeof(RabbitEnemy), true)]
    [CanEditMultipleObjects]
    public class RabbitEditor : Enemy3DEditor
    {
        SerializedProperty TurnLength;

        protected override void OnChildEnable()
        {
            TurnLength = serializedObject.FindProperty("m_TurnLength");
        }

        protected override void OnChildInspectorGUI()
        {
            RabbitEnemy enemy = target as RabbitEnemy;

            EditorGUILayout.LabelField("〇ウサギ固有のステータス");
            // int
            TurnLength.floatValue = EditorGUILayout.FloatField("折り返す距離", enemy.m_TurnLength);
        }
    }
#endif
    #endregion
}
