using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy3D : MonoBehaviour
{
    #region シリアライズ変数
    [SerializeField]
    protected int m_RotateDegree = 180;                 // 振り向き時の角度
    [SerializeField]
    protected float m_Speed = 1.0f;                     // 移動速度
    [SerializeField]
    protected float m_TrapHitSpeed = 3.0f;              // 移動速度
    [SerializeField]
    protected float m_RageTime = 10.0f;                 // 暴れる時間
    [SerializeField]
    protected float m_ReRageTime = 5.0f;                // 再度暴れる時間
    [SerializeField]
    protected float m_ViewLength = 10.0f;               // プレイヤーが見える距離
    [SerializeField]
    protected float m_ViewAngle = 30.0f;                // プレイヤーが見える角度
    [SerializeField]
    protected Transform m_GroundPoint = null;           // 接地ポイント
    [SerializeField]
    protected Transform m_RayPoint = null;              // レイポイント
    [SerializeField]
    protected Transform[] m_MovePoints = null;          // 移動用ポイント配列
    [SerializeField]
    protected GameObject m_Sprite = null;               // スプライト
    [SerializeField]
    protected CameraMove m_MainCamera = null;           // メインカメラ
    [SerializeField]
    protected WallChackPoint m_WChackPoint = null;      // 壁捜索ポイント
    #endregion

    #region protected変数
    protected int m_Size = 1;                           // 動物の大きさ(内部数値)
    protected float m_StateTimer = 0.0f;                // 状態の時間
    protected string m_LineObjName = "Player";
    protected bool m_IsRendered = false;                // カメラに映っているか
    protected Vector3 m_TotalVelocity = Vector3.zero;   // 合計の移動量
    protected Vector3 m_Velocity = Vector3.right;       // 移動量
    protected Vector3 m_MovePointPosition;              // 移動ポイントの位置
    protected Transform m_DiscoverPlayer;               // プレイヤーを発見
    protected Player m_Player = null;                   // 当たったプレイヤー
    protected Rigidbody m_Rigidbody;

    // モーション番号
    protected int m_MotionNumber = (int)AnimationNumber.ANIME_IDEL_NUMBER;
    #endregion

    #region private変数
    //private bool m_IsPravGround;                    // 前回の接地判定
    //private int m_PointCount = 0;                   // 移動ポイント数
    private int m_CurrentMovePoint = -1;            // 現在の移動ポイント
    private float m_MoveStartTime = 0.5f;           // 移動開始時間
    private bool m_IsLift = false;                  // 持ち上げられたか
    private GameObject m_TrapObj = null;            // 挟まったトラバサミ
    private State m_State = State.Idel;             // 状態
    private TrapHitState m_THState =
        TrapHitState.TrapHit_Rage;                  // トラップヒット状態
    //private DSNumber m_DSNumber =
    //    DSNumber.DISCOVERED_CHASE_NUMBER;           // 追跡状態の番号   
    private NavMeshAgent m_Agent;                   // ナビメッシュエージェント  
    private EnemySprite m_EnemySprite;              // エネミースプライト                                          
    private List<State>
        m_DiscoveredStates = new List<State>();     // 発見後の行動

    private const int MultSpeed = 10;               // 速度の倍率(調整しやすくさせるため)
    private const string PlayerTag = "Player";      // プレイヤータグ
    #endregion

    #region 列挙クラス
    // 状態クラス 
    public enum State
    {
        Idel,       // 待機状態
        Chase,      // 追跡状態
        Discover,   // 発見状態
        TrapHit,    // トラバサミに挟まれている状態
        Runaway,    // 逃亡状態
        DeadIdel    // 死亡待機状態
    }
    // トラバサミに挟まれた時の状態クラス
    protected enum TrapHitState
    {
        TrapHit_Change, // トラップ化状態
        TrapHit_TakeIn, // トラバサミに飲み込まれた状態
        TrapHit_Rage,   // トラバサミに挟まれている(暴れている)状態
        TrapHit_Touch,  // トラバサミに挟まれ終わった状態
    }

    protected enum AnimationNumber
    {
        ANIME_IDEL_NUMBER = 0,
        ANIME_CHASE_NUMBER = 1,
        ANIME_DISCOVER_NUMBER = 2,
        ANIME_TRAP_HIT_NUMBER = 3,
        ANIME_TRAP_NUMBER = 4,
        ANIME_RUNAWAY_NUMBER = 5,
        ANIME_DEAD_NUMBER = 6
    };

    // DiscoveredStateNumber
    protected enum DSNumber
    {
        DISCOVERED_CHASE_NUMBER = 0,
        DISCOVERED_RUNAWAY_NUMBER = 1
    }

    #endregion

    #region 基盤関数
    // Use this for initialization
    protected virtual void Start()
    {
        // アニメーションリストにリソースを追加
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Agent = GetComponent<NavMeshAgent>();
        // オブジェクトの確認
        CheckObject();
        // ナビメッシュエージェントの設定
        SetAgentStatus();
        // 移動配列に何も入っていなかった場合は通常移動
        //if(m_MovePoints.Length == 0)

        m_DiscoveredStates.Add(State.Chase);
        m_DiscoveredStates.Add(State.Runaway);

        // スプライトカラーの変更
        ChangeSpriteColor(Color.red);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // 状態の更新
        UpdateState(Time.deltaTime);

        m_IsRendered = false;
        //ビルボード
        Vector3 p = m_MainCamera.transform.localPosition;
        //transform.LookAt(p);
        m_Sprite.transform.LookAt(p);
    }
    #endregion

    #region 状態関数
    // 状態の更新
    private void UpdateState(float deltaTime)
    {
        // 移動開始時間が 0 になったら移動
        m_MoveStartTime = Mathf.Max(m_MoveStartTime - deltaTime, 0.0f);
        if (m_MoveStartTime > 0.0f) return;

        // 状態の変更
        switch (m_State)
        {
            case State.Idel: Idel(deltaTime); break;
            case State.Discover: Discover(deltaTime); break;
            case State.Chase: Chase(deltaTime); break;
            case State.TrapHit: TrapHit(deltaTime); break;
            case State.Runaway: Runaway(deltaTime); break;
            case State.DeadIdel: DeadIdel(deltaTime); break;
        };

        // 状態の時間加算
        m_StateTimer += deltaTime;

        // 位置ベクトルを代入
        MoveVelocity();

        //Vector2 newVelocity = m_Rigidbody.velocity;
        //Vector2 gravity = Vector2.up * m_Rigidbody.velocity.y;
        //newVelocity = m_Velocity * m_Speed + gravity;
        //m_Rigidbody.velocity = newVelocity;

        //m_IsPravGround = IsGround();
    }

    // 状態の変更
    protected void ChangeState(State state, AnimationNumber motion)
    {
        if (m_State == state) return;
        m_State = state;
        m_MotionNumber = (int)motion;
        m_StateTimer = 0.0f;
        // ナビメッシュエージェントの移動停止
        m_Agent.Stop();
    }

    // トラップヒット状態の変更
    protected void ChangeTrapHitState(
        TrapHitState thState, AnimationNumber motion)
    {
        // 状態の変更
        ChangeState(State.TrapHit, motion);
        // 同じトラップヒット状態なら返す
        if (m_THState == thState) return;
        m_THState = thState;
        m_StateTimer = 0.0f;
        //m_MotionNumber = (int)motion;
        //m_StateTimer = 0.0f;
    }

    // 待機状態
    protected virtual void Idel(float deltaTime)
    {
        GameObject obj = null;
        if (InPlayer(out obj))
        {
            // 発見状態に遷移
            ChangeState(State.Discover, AnimationNumber.ANIME_IDEL_NUMBER);
            var player = obj.GetComponent<Player>();
            if(player != null)
            {
                m_Player = player;
            }
            m_DiscoverPlayer = obj.transform;
            //m_DSNumber = DSNumber.DISCOVERED_RUNAWAY_NUMBER;
            return;
        };

        // 移動
        if (m_MovePoints.Length > 0) PointMove(deltaTime);
        else ReturnMove(deltaTime, 1.0f);
        //else PointMove(deltaTime);
    }

    // 発見状態
    protected virtual void Discover(
        float deltaTime,
        DSNumber number =
        DSNumber.DISCOVERED_CHASE_NUMBER)
    {
        //// 接地したら、他の状態に遷移
        //if (!m_IsPravGround && IsGround())
        //{
        //    //ChangeState(State.Runaway, AnimationNumber.ANIME_RUNAWAY_NUMBER);
        //    ChangeState(
        //        m_DiscoveredStates[(int)m_DSNumber],
        //        AnimationNumber.ANIME_CHASE_NUMBER);
        //    return;
        //}
        ChangeState(State.Runaway, AnimationNumber.ANIME_RUNAWAY_NUMBER);
        m_Agent.Resume();
        SoundNotice(m_DiscoverPlayer);
        ChangeSpriteColor(Color.blue);
    }

    // 追跡状態
    protected void Chase(float deltaTime)
    {
        // 移動
        ChasePlayer();
        //Move(deltaTime, 2.0f);
    }

    // トラバサミに挟まれている状態
    protected void TrapHit(float deltaTime)
    {
        switch (m_THState)
        {
            case TrapHitState.TrapHit_Change: TrapHitChange(deltaTime); break;
            case TrapHitState.TrapHit_TakeIn: TrapHitTakeIn(deltaTime); break;
            case TrapHitState.TrapHit_Rage: TrapHitRage(deltaTime); break;
            case TrapHitState.TrapHit_Touch: TrapHitTouch(deltaTime); break;
        };
    }

    protected void TrapHitRage(float deltaTime)
    {
        //// プレイヤーが壁に当たった場合は、折り返す
        ////Collision2D.Equals

        ////// 移動(通常の移動速度の数倍)
        ////Move(deltaTime, m_TrapHitSpeed);

        //if (m_Player == null) return;
        //if (m_Player.GetState() == Player.State.WAIT)
        //{
        //    ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
        //    m_Player = null;
        //    // カメラがプレイヤーを追尾するようにする
        //    m_MainCamera.MoveChenge(true);
        //    ChangeSpriteColor(Color.red);
        //    return;
        //}
        //else if (m_Player.GetState() == Player.State.TOUCH)
        //{
        //    ChangeTrapHitState(
        //        TrapHitState.TrapHit_Touch,
        //        AnimationNumber.ANIME_TRAP_HIT_NUMBER
        //        );
        //    // カメラがプレイヤーを追尾するようにする
        //    m_MainCamera.MoveChenge(true);
        //    ChangeSpriteColor(Color.cyan);
        //    return;
        //}
    }

    // 飲み込まれ状態
    protected void TrapHitTakeIn(float deltaTime)
    {
        //this.transform.position = m_Player.transform.position;
    }

    // 暴れ治まり状態
    protected void TrapHitTouch(float deltaTime)
    {
        //Move(deltaTime, m_Speed / 20.0f);
        //// 再度暴れ状態になる場合、暴れ状態に遷移する
        //if (m_ReRageTime > m_StateTimer) return;
        //// プレイヤーの状態を変更
        //// 仮
        //m_Player._state = Player.State.ENDURE;
        //ChangeTrapHitState(
        //    TrapHitState.TrapHit_Rage,
        //    AnimationNumber.ANIME_TRAP_HIT_NUMBER
        //    );
        //ChangeSpriteColor(Color.yellow);
    }

    // 罠状態
    protected void TrapHitChange(float deltaTime)
    {
        if (m_TrapObj != null) return;
        // 死亡待機状態に遷移
        ChangeState(State.DeadIdel, AnimationNumber.ANIME_DEAD_NUMBER);
        // ステータスの初期化
        InitState();

        //// プレイヤーが罠状態なら返す

        //// 持ち上げられたら返す
        //if (m_IsLift) return;
        ////if (m_LiftObj != null) return;

        //// 変える
        //if (m_Player.GetState() != Player.State.WAIT) return;

        //// 待機状態に遷移
        //ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
        //// ナビメッシュエージェントの再度移動
        //m_Agent.Resume();
        //ChangeSpriteColor(Color.red);
    }

    // 逃げ状態
    protected void Runaway(float deltaTime)
    {
        // 移動(通常の移動速度の数倍)
        Move(deltaTime, 4.0f);
        //Camera.
        GameObject obj = null;
        if (!InPlayer(out obj))
        {
            // 発見状態に遷移
            ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
            m_Agent.Resume();
            m_Player = null;
            ChangeSpriteColor(Color.red);
            //m_DSNumber = DSNumber.DISCOVERED_RUNAWAY_NUMBER;
            return;
        };
        //find
    }

    // 死亡待機状態
    protected void DeadIdel(float deltaTime)
    {
        // 外部からアクティブ状態に変更されたら、待機状態に遷移
        if (!gameObject.activeSelf) return;
        // ReMove();
        ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
        gameObject.SetActive(true);
        m_Agent.Resume();
        ChangeSpriteColor(Color.red);
    }
    #endregion

    #region virtual関数
    // 移動ベクトルを代入します
    protected virtual void MoveVelocity()
    {
        // 移動量の加算
        m_Rigidbody.velocity = m_TotalVelocity;
        // 移動量の初期化
        m_TotalVelocity = Vector3.zero;
    }

    // 壁に衝突したときに、折り返します
    protected virtual void TurnWall()
    {
        // 壁に当たった、崖があった場合は折り返す
        //if (m_WChackPoint != null)
        //{
        //    if (m_WChackPoint.IsWallHit())
        //    {
        //    }
        //}

        // 壁に当たらなかった場合は折り返す
        if (m_WChackPoint == null || !m_WChackPoint.IsWallHit()) return;
        // 角度の設定
        SetDegree();
    }

    // 角度の設定
    protected void SetDegree()
    {
        // 移動量の変更
        m_Velocity *= -1;
        //var wall = m_WChackPoint.GetHitWallObj();
        // 角度の取得
        //var rotate = wall.transform.rotation.eulerAngles;
        // オブジェクトの回転
        //transform.Rotate(transform.up, m_RotateDegree);
        // スプライトの回転
        m_Sprite.transform.Rotate(Vector3.up, m_RotateDegree);
        // 壁捜索ポイントの方向変更
        m_WChackPoint.ChangeDirection();
    }
    #endregion

    #region 判定用関数
    // プレイヤーが見えているか
    protected bool InPlayer(out GameObject player)
    {
        return InObject("PlayerSprite", out player);
    }

    protected bool InObject(string name, out GameObject hitObj)
    {
        //var isPlayer = false;
        hitObj = null;
        var objName = name;
        var obj = GameObject.Find(objName);
        // オブジェクトがいない場合は返す
        if (obj == null) return false;
        // レイポイントからオブジェクトの位置までのレイを伸ばす
        var playerDir = obj.transform.position - m_RayPoint.position;
        Ray ray = new Ray(
            m_RayPoint.position, 
            playerDir
            );
        RaycastHit hitInfo;
        var hit = Physics.Raycast(ray, out hitInfo);
        // プレイヤーに当たらなかった場合、
        // プレイヤー以外に当たった場合は返す
        //print("見えているか調査");
        if (!hit || hitInfo.collider.name != objName) return false;
        // 当たったオブジェクト
        hitObj = hitInfo.collider.gameObject;
        // プレイヤーとの距離を求める
        var length = Vector3.Distance(
            m_RayPoint.position,
            obj.transform.position
            );
        // 可視距離から離れていれば返す
        //print("距離調査");
        if (length > m_ViewLength) return false;
        // 視野角の外ならば返す
        var dir = obj.transform.position - m_RayPoint.position;
        var angle = Vector3.Angle(m_RayPoint.forward, dir);
        //print("角度調査");
        if (Mathf.Abs(angle) > m_ViewAngle) return false;
        // プレイヤーを見つけた
        //print("見つけた");
        return true;
    }

    // 罠の動物が見えているか
    protected bool IsInTrapAnimal(out GameObject hitObj)
    {
        //hitObj = null;
        ////// プレイヤーがいない場合は返す
        ////if (m_Player == null) return false;
        //// プレイヤーが空の場合、プレイヤーをセットする
        //SetPlayer();
        //// プレイヤーがトラップ状態でなかったら、返す
        //// トラップ状態でも、ターゲットがいなければ返す
        //if (m_Player.GetState() != Player.State.TRAP ||
        //    m_Player._target == null) return false;

        //Ray ray = new Ray(
        //    m_RayPoint.transform.position,
        //    m_Player.transform.position
        //    );
        //RaycastHit hitInfo;
        //var hit = Physics.Raycast(ray, out hitInfo);
        //print("見えているか調査");
        //// 間にオブジェクトがある場合は返す
        //if (hitInfo.collider != null) return false;
        //// 罠になっている動物を入れる
        //hitObj = m_Player._target;
        //// 罠の動物との距離を求める
        //var length = Vector3.Distance(
        //    m_RayPoint.transform.position,
        //    hitObj.transform.position
        //    );
        //// 可視距離から離れていれば返す
        //print("距離調査");
        //if (length > m_ViewLength) return false;
        //// 視野角の外ならば返す
        //var dir = hitObj.transform.position - m_RayPoint.position;
        //var angle = Vector3.Angle(this.transform.forward, dir);
        //print("角度調査");
        //if (Mathf.Abs(angle) < m_ViewAngle) return false;
        //// プレイヤーを見つけた
        //print("見つけた");
        //return true;

        hitObj = null;
        return false;
    }

    // 接地しているか
    //protected bool IsGround()
    //{
    //    int layerMask = LayerMask.GetMask(new string[] { "Ground" });
    //    Collider2D hit =
    //        Physics2D.OverlapPoint(m_GroundPoint.position, layerMask);
    //    return hit != null;
    //}
    #endregion

    #region その他関数
    // ステータスの初期化を行います
    protected virtual void InitState()
    {
        // トラバサミを空っぽにする
        m_TrapObj = null;
        // 親オブジェクトを非アクティブに変更
        this.transform.parent.gameObject.SetActive(false);
        //this.transform.position = this.transform.parent.position;
        this.transform.localPosition = Vector3.zero;
        // ナビメッシュエージェント関連の初期化
        m_CurrentMovePoint = 0;
        m_MovePointPosition = m_MovePoints[0].position;
    }

    // 移動関数
    protected virtual void ReturnMove(float deltaTime, float subSpeed = 1.0f)
    {
        // 壁に衝突したときに、折り返す
        TurnWall();

        //this.transform.position += m_Velocity.normalized / 10;
        //return;

        //velocity = Vector3.forward;
        // 移動
        var cameraR = Vector3.Scale(m_MainCamera.transform.right, Vector3.one);
        //var v = (Vector3.forward - Vector3.right) * m_Velocity.z +
        //    cameraR * m_Velocity.x;

        var v = m_Velocity;
        v.y = 0;
        //var v = (Vector3.forward - Vector3.right) * Input.GetAxis("Vertical") + cameraR * Input.GetAxis("Horizontal");

        m_TotalVelocity = 
            (m_Speed * subSpeed) * MultSpeed * 
            v.normalized * deltaTime;
    }

    protected virtual void Move(float deltaTime, float subSpeed = 1.0f)
    {
        if (m_MovePoints.Length > 0) PointMove(deltaTime);
        else ReturnMove(deltaTime, subSpeed);
    }

    private void PointMove(float deltaTime)
    {
        //var pointPos = m_MovePoints[m_CurrentMovePoint % m_MovePoints.Length].position;
        //var v = pointPos - this.transform.position;
        //dir = dir.normalized;
        //m_Velocity = Vector3.Normalize(pointPos - this.transform.position);
        // ポイントとの距離が一定距離に達したら、移動ポイントを変える
        // Vector3.Distance(m_MovePointPosition, this.transform.position)

        //// エージェントの移動
        //m_Agent.destination = m_MovePoints[m_CurrentMovePoint % m_MovePoints.Length].position;
        // ベクトルとの角度によって、角度を変更します
        // 目的地に到着したら、目的地のポイントを変える
        var length = Vector3.Distance(
            m_Agent.destination, this.transform.position
            );
        if (length < 0.5f) ChangeMovePoint();
    }

    // 移動ポイントの変更を行います
    private void ChangeMovePoint()
    {
        // 次の移動ポイントに変更
        m_CurrentMovePoint++;
        var pos = 
            m_MovePoints[m_CurrentMovePoint % m_MovePoints.Length].position;
        ChangeMovePoint(pos);
        //// エージェントの移動
        //m_Agent.destination = m_MovePointPosition;
        //ChangeSpriteAngle();

        ////m_Velocity = m_MovePointPosition - this.transform.position;
        ////var changeAngle = 180 / 3;        
        ////print(Vector2.Angle(rightV, dir));
    }

    protected void ChangeMovePoint(Vector3 position)
    {
        // 次の移動ポイントに変更
        m_MovePointPosition = position;
        // エージェントの移動
        m_Agent.destination = m_MovePointPosition;
        ChangeSpriteAngle();
    }

    private void ChangeSpriteAngle()
    {
        // スプライトの角度変更
        var v = new Vector2(-1.0f, 1.0f).normalized;
        var pointV = m_MovePointPosition - this.transform.position;
        var dir = new Vector2(pointV.x, pointV.z).normalized;
        var angle = Vector2.Angle(v, dir);
        // 一定角度なら画像を変える
        var cAnagle = 30.0f;
        var num = 1;
        if (Mathf.Abs(angle) < cAnagle) num = 2;
        else if (Mathf.Abs(angle) < 180.0f - cAnagle) num = 0;
        //else m_EnemySprite.ChangeSprite(1);
        // 右ベクトルとの角度を求める
        // クォータービュー上に左右を分断する線を引いて、
        // 移動ベクトルとの角度を求めるイメージ
        var rightV = new Vector2(1.0f, 1.0f).normalized;
        var x = 1.0f;
        if (Vector2.Angle(rightV, dir) >= 90.0f) x = -1;
        m_EnemySprite.ChangeSprite(num, x);
    }

    // オブジェクトの確認を行います
    private void CheckObject()
    {
        // スプライトの確認
        CheckSprite();
        // 生成ボックスの確認
        CheckCreateBox();
        // メインカメラの確認
        CheckMainCamera();
    }

    // 生成ボックスの確認を行います
    private void CheckCreateBox()
    {
        // 生成ボックスの移動ポイント取得
        var box = this.transform.parent.GetComponentInParent<EnemyCreateBox>();
        if (box != null)
        {
            var count = 0;
            var size = box.GetMovePointsSize();
            // 移動ポイント配列のサイズ変更
            ResizeMovePoints(size);
            for (int i = 0; i != size; i++)
            {
                var point = box.GetMovePoint(i);
                // 移動ポイントが空でなかったら、追加する
                if (point != null)
                {
                    m_MovePoints[count] = point;
                    count++;
                }
            }
            // 移動ポイントの変更
            if (count != 0) ChangeMovePoint();
        }
    }

    // メインカメラの確認を行います
    private void CheckMainCamera()
    {
        // メインカメラが設定されていなかった場合
        if (m_MainCamera == null)
        {
            var camera = GameObject.Find("Main Camera");
            if (camera == null) return;
            var cMove = camera.GetComponent<CameraMove>();
            m_MainCamera = cMove;
            //ビルボード
            Vector3 p = m_MainCamera.transform.localPosition;
            //transform.LookAt(p);
            m_Sprite.transform.LookAt(p);
        }
    }

    // スプライトの確認を行います
    private void CheckSprite()
    {
        // スプライトがなかった場合
        if (m_Sprite == null)
        {
            var obj = this.transform.FindChild("EnemySprite");
            if (obj == null) return;
            m_Sprite = obj.gameObject;
        }
        // エネミースプライトの取得
        m_EnemySprite = m_Sprite.GetComponent<EnemySprite>();
        m_EnemySprite.ChackRender();
    }

    // エージェントの設定を行います
    protected void SetAgentStatus()
    {
        // ステータス
        m_Agent.speed = m_Speed;
        m_Agent.acceleration = m_Speed * 10;
        m_Agent.autoBraking = false;
    }

    // 敵のスプライトカラーの変更
    protected void ChangeSpriteColor(Color color)
    {
        var child = gameObject.transform.FindChild("EnemySprite");
        if (child == null) return;
        var child2 = child.gameObject.transform.FindChild("Sprite");
        var sprite = child2.GetComponent<SpriteRenderer>();
        if (sprite == null) return;
        sprite.color = color;
    }

    // プレイヤーとの向きを返します(単位ベクトル)
    protected Vector3 PlayerDirection()
    {
        var player = GameObject.Find("Player");
        // プレイヤーがいなければ、ゼロベクトルを返す
        if (player != null) return Vector3.zero;
        var direction = Vector3.one;
        var dir = player.transform.position - this.transform.position;
        if (dir.x < 0.0f) direction.x = -1.0f;
        if (dir.y < 0.0f) direction.y = -1.0f;
        if (dir.z < 0.0f) direction.z = -1.0f;
        return direction;
    }

    // 対象との向きを取得します
    protected Vector3 ObjectDirection(GameObject obj, Vector3 addPosition = new Vector3())
    {
        var direction = Vector3.zero;
        // オブジェクトが空だったら、ゼロベクトルを返す
        if (obj == null) return direction;
        direction = (obj.transform.position + addPosition) - this.transform.position;
        return direction.normalized;
    }

    // 対象との距離を取得します
    //protected float ObjectLength(GameObject obj)
    //{
    //    var length = 0.0f;
    //}

    // プレイヤーとの衝突判定処理
    protected void OnCollidePlayer(Collider collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == PlayerTag)
        {
            ////// 当たったプレイヤーを子供に追加
            ////collision.gameObject.transform.parent = gameObject.transform;
            //var player = collision.GetComponent<Player>();
            //if (player == null) return;
            //// プレイヤーがはさんだ状態なら、トラップヒット状態に遷移
            //if (player.GetState() == Player.State.ENDURE)
            //{
            //    // 暴れ状態に遷移
            //    ChangeTrapHitState(
            //        TrapHitState.TrapHit_Rage,
            //        AnimationNumber.ANIME_TRAP_HIT_NUMBER
            //        );
            //    // 暴れる時間を入れる
            //    player.SetFall(m_RageTime);
            //    m_Player = player;
            //    // カメラの追尾を解除
            //    //var camera = m_MainCamera.GetComponent<CameraMove>();
            //    //camera.MoveChenge(false);
            //    m_MainCamera.MoveChenge(false);
            //    // 敵を揺らす
            //    var speed = 0.5f;
            //    // .Add("easetype", "easeInOutBack")
            //    var moveHash = iTween.Hash(
            //        "x", speed, "y", speed / 10, "delay", 0.5f, "time", 0.5f
            //        );
            //    moveHash.Add("easetype", iTween.EaseType.easeInCubic);
            //    //moveHash.Add("loopType", "ioop");
            //    // iTween.Hash("x", speed, "y", speed, "time", m_RageTime)
            //    iTween.ShakePosition(
            //        gameObject,
            //        moveHash
            //        );
            //    ChangeSpriteColor(Color.yellow);
            //}
        }
    }

    // プレイヤーの設定します
    protected void SetPlayer()
    {
        //if (m_Player != null) return;
        //var obj = GameObject.Find("Player");
        //if (obj == null) return;
        //var player = obj.GetComponent<Player>();
        //if (player == null) return;
        //m_Player = player;
    }
    // 移動ポイント配列のサイズ変更
    private void ResizeMovePoints(int size)
    {
        for (int i = 0; i != m_MovePoints.Length; i++)
        {
            m_MovePoints[i] = null;
        }
        // サイズの変更
        m_MovePoints = new Transform[size];
    }
    #endregion

    #region public関数
    // プレイヤーの方向を向きます(単位ベクトル)
    public void ChasePlayer()
    {
        var player = GameObject.Find("Player");
        // プレイヤーがいなければ、返す
        //if (player != null) return;
        if (player != null)
        {
            ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
            return;
        }
        var direction = Vector2.right;
        var dir = player.transform.position - this.transform.position;
        var length = 2.0f;
        // 近づきすぎたら、移動しない
        if (Mathf.Abs(dir.x) < length) direction.x = 0.0f;
        // 方向転換
        if (dir.x < 0.0f) direction.x = -1.0f;
        // 移動量に代入
        m_TotalVelocity = m_Speed * direction * Time.deltaTime;
    }

    // 敵がはさまれた時の、暴れる時間を返します(秒数)
    public float RageTime() { return m_RageTime; }

    // 敵を待機状態にさせます
    public void ChangeWait()
    {
        ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
        ChangeSpriteColor(Color.red);
    }

    // 敵を飲み込まれ状態にさせます
    public void ChangeTakeIn()
    {
        // 暴れ状態に遷移
        ChangeTrapHitState(
            TrapHitState.TrapHit_TakeIn,
            AnimationNumber.ANIME_TRAP_HIT_NUMBER
            );
        ChangeSpriteColor(Color.black);
    }

    // 敵を暴れ状態にさせます
    public void ChengeRage()
    {
        //ChangeState(State)
        // 暴れ状態に遷移
        ChangeTrapHitState(
            TrapHitState.TrapHit_Rage, 
            AnimationNumber.ANIME_TRAP_HIT_NUMBER
            );
        ChangeSpriteColor(Color.yellow);
    }

    // 敵をトラップ化させます
    public void ChangeTrap(GameObject obj = null)
    {
        // トラップ化状態に遷移
        ChangeTrapHitState(
            TrapHitState.TrapHit_Change,
            AnimationNumber.ANIME_TRAP_NUMBER
            );
        ChangeSpriteColor(Color.green);
        // 挟まったトラバサミを入れる
        if (obj == null) return;
        m_TrapObj = obj;
        //// プレイヤーが空だったら、入れる
        //SetPlayer();
    }

    // 敵を持ち上げられたことにします
    public void EnemyLift() { m_IsLift = true; }

    //// 移動ポイント配列に追加を行います
    //public void AddMovePoint(int num, Transform point)
    //{
    //    //var num = m_MovePoints.Length;
    //    // 追加
    //    m_MovePoints[num] = point;
    //    //Array
    //}

    // 動物の状態を取得します
    public State GetState() { return m_State; }

    // カメラに映っているかを返します
    public bool IsRendered() { return m_IsRendered; }
    // 音に気付きます
    public virtual void SoundNotice(Transform point)
    {
        //　音の位置をポイントに変更します
        ChangeMovePoint(point.position);
        m_Agent.Resume();
    }
    #endregion

    #region 衝突判定関数
    // 衝突判定(トリガー用)
    public void OnTriggerEnter(Collider collision)
    {
        OnCollidePlayer(collision);
    }

    // 衝突中(トリガー用)
    public void OnTriggerStay(Collider collision)
    {
        OnCollidePlayer(collision);
    }

    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    var tag = collision.transform.tag;

    //    if(tag == m_PlayerTag)
    //    {
    //        ChangeState(State.Idel, AnimationNumber.ANIME_IDEL_NUMBER);
    //        return;
    //    }
    //}

    // 衝突判定
    public void OnCollisionEnter(Collision collision)
    {
        //var tag = collision.gameObject.tag;

        // プレイヤーに当たった場合
        //if (tag == m_PlayerTag)
        //{
        //    // 当たったプレイヤーを子供に追加
        //    //collision.gameObject.transform.parent = gameObject.transform;

        //    //if (player == null) return;
        //    //player
        //    //player._state = Player.State.Endure;
        //    //player._target = gameObject;
        //}

        // エネミー同士が当たった場合
        //if (tag == "Enemy") { this.transform.Rotate(Vector3.up, 180.0f); }
    }
    #endregion

    #region ギズモ関数
    // ギズモの描画
    public void OnDrawGizmos()
    {
        DrawObjLine("PlayerSprite");
        //// 視野角の右端の描画
        //Gizmos.DrawRay(m_RayPoint.position, Quaternion.Euler(0, m_ViewAngle, 0) * forward);
        //// 視野角の左端の描画
        //Gizmos.DrawRay(m_RayPoint.position, Quaternion.Euler(0, -m_ViewAngle, 0) * forward);

        //Gizmos.color = Color.green;
        //DrawObjLine(m_MovePoints[m_CurrentMovePoint].name);

        //// 移動の描画
        //for(int i = 0; i != m_MovePoints.Length; i++)
        //{
        //    Gizmos.DrawLine(
        //        m_MovePoints[i].transform.position, 
        //        m_MovePoints[(i + 1) % m_MovePoints.Length].transform.position
        //        );
        //}
    }

    // 対象との線分を描画します
    protected void DrawObjLine(string name)
    {
        var obj = GameObject.Find(name);
        if (obj == null) return;
        Gizmos.color = Color.red;
        // レイの描画
        Gizmos.DrawLine(m_RayPoint.position, obj.transform.position);
    }
    #endregion

    #region カメラ判定
    // カメラの描画範囲に入っている間に、実行される
    public void OnWillRenderObject()
    {
        if (m_MainCamera.tag != "MainCamera") return;
        // 見えている
        m_IsRendered = true;
    }
    #endregion

    #region エディターのシリアライズ変更
    // 変数名を日本語に変換する機能
    // CustomEditor(typeof(Enemy), true)
    // 継承したいクラス, trueにすることで、子オブジェクトにも反映される
#if UNITY_EDITOR
    [CustomEditor(typeof(Enemy3D), true)]
    [CanEditMultipleObjects]
    public class Enemy3DEditor : Editor
    {
        SerializedProperty Speed;
        SerializedProperty TrapHitSpeed;
        SerializedProperty RageTime;
        SerializedProperty ReRageTime;
        SerializedProperty ViewLength;
        SerializedProperty ViewAngle;
        SerializedProperty GroundPoint;
        SerializedProperty RayPoint;
        SerializedProperty MovePoints;
        SerializedProperty Sprite;
        SerializedProperty MainCamera;
        SerializedProperty WChackPoint;
        SerializedProperty RotateDegree;

        protected List<SerializedProperty> m_Serializes = new List<SerializedProperty>();
        protected List<string> m_SerializeNames = new List<string>();

        

        public void OnEnable()
        {
            //for(var i = 0; i != m_SerializeNames.Count; i++)
            //{
            //    SetSerialize(m_Serializes[i], m_SerializeNames[i]);
            //}

            Speed = serializedObject.FindProperty("m_Speed");
            TrapHitSpeed = serializedObject.FindProperty("m_TrapHitSpeed");
            RageTime = serializedObject.FindProperty("m_RageTime");
            ReRageTime = serializedObject.FindProperty("m_ReRageTime");
            ViewLength = serializedObject.FindProperty("m_ViewLength");
            ViewAngle = serializedObject.FindProperty("m_ViewAngle");
            GroundPoint = serializedObject.FindProperty("m_GroundPoint");
            RayPoint = serializedObject.FindProperty("m_RayPoint");
            MovePoints = serializedObject.FindProperty("m_MovePoints");
            Sprite = serializedObject.FindProperty("m_Sprite");
            MainCamera = serializedObject.FindProperty("m_MainCamera");
            WChackPoint = serializedObject.FindProperty("m_WChackPoint");
            RotateDegree = serializedObject.FindProperty("m_RotateDegree");
            OnChildEnable();
        }

        private void AddSerialize()
        {
            m_Serializes.Add(Speed);
        }

        public void SetSerialize(SerializedProperty serialize, string name)
        {
            serialize = serializedObject.FindProperty(name);
        }

        protected virtual void OnChildEnable() { }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            // 必ず書く
            serializedObject.Update();

            Enemy3D enemy = target as Enemy3D;

            EditorGUILayout.LabelField("〇動物共通のステータス");
            // int
            RotateDegree.intValue = EditorGUILayout.IntField("折り返し時の角度(度数法)", enemy.m_RotateDegree);

            // float
            Speed.floatValue = EditorGUILayout.FloatField("移動速度(m/s)", enemy.m_Speed);
            TrapHitSpeed.floatValue = EditorGUILayout.FloatField("はさまれた時の速度(m/s)", enemy.m_TrapHitSpeed);
            RageTime.floatValue = EditorGUILayout.FloatField("暴れる時間(秒)", enemy.m_RageTime);
            ReRageTime.floatValue = EditorGUILayout.FloatField("再度暴れる時間(秒)", enemy.m_ReRageTime);
            ViewLength.floatValue = EditorGUILayout.FloatField("視野距離(m)", enemy.m_ViewLength);
            ViewAngle.floatValue = EditorGUILayout.FloatField("視野角度(度数法)", enemy.m_ViewAngle);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("〇各種オブジェクトのTransfome");
            // Transform
            GroundPoint.objectReferenceValue = EditorGUILayout.ObjectField("接地ポイント", enemy.m_GroundPoint, typeof(Transform), true);
            RayPoint.objectReferenceValue = EditorGUILayout.ObjectField("レイポイント", enemy.m_RayPoint, typeof(Transform), true);
            // 配列
            EditorGUILayout.PropertyField(MovePoints, new GUIContent("徘徊ポイント"), true);
            // EditorGUILayout.PropertyField( prop , new GUIContent( “array1” ), true );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("〇各種オブジェクト");
            Sprite.objectReferenceValue = EditorGUILayout.ObjectField("敵の画像", enemy.m_Sprite, typeof(GameObject), true);
            MainCamera.objectReferenceValue = EditorGUILayout.ObjectField("メインカメラ", enemy.m_MainCamera, typeof(CameraMove), true);
            WChackPoint.objectReferenceValue = EditorGUILayout.ObjectField("壁捜索ポイント", enemy.m_WChackPoint, typeof(WallChackPoint), true);

            EditorGUILayout.Space();

            OnChildInspectorGUI();

            // Unity画面での変更を更新する(これがないとUnity画面で変更できなくなる)
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnChildInspectorGUI() { }
    }

    [MenuItem("Tools/SelectGameObject")]
    static void SelectGameObject()
    {

        //var obj = FindObjectOfType<GameObject>();
        //if (obj == null)
        //{
        //    Debug.Log("何も選択されていない");
        //    return;
        //}

        //Selection.activeGameObject = obj.gameObject;
        //Debug.Log(Selection.activeGameObject.name);
        var boxes = new List<GameObject>();

        //Debug.Log(Selection.gameObjects.Length);

        foreach (GameObject obj in Selection.gameObjects)
        {
            //boxes
            if (obj.name == "CreateBox")
            {
                boxes.Add(obj);
                Debug.Log(obj.name);
            }
        }
    }

#endif
    #endregion
}
