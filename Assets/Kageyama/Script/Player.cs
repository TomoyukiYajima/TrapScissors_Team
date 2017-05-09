/*******************************************
制　作　者：影山清晃
最終編集者：影山清晃

内　　　容：プレイヤーキャラクターの移動、画像切り替え
最終更新日：2017/04/21

********************************************/
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField, TooltipAttribute("メインカメラ")]
    private GameObject _mainCamera;
    
    #region 移動や重力に必要な変数
    //移動する方向ベクトル
    private Vector3 _moveDirection = Vector3.zero;
    //プレイヤーの移動速度
    public float _speed;
    //落ちていく速度を加速させる
    private float _gadd;
    [SerializeField , TooltipAttribute("重力の大きさ")]
    private float _gravity;
    //地面に衝突しているかどうか
    private bool _groundOn;
    [SerializeField, TooltipAttribute("ステージの大きさ")]
    private float _clampX, _clampZ;
    #endregion
    #region 画像切り替えに必要な変数
    [SerializeField]
    private GameManager _childSprite;
    private SpriteRenderer _myRenderer;
    public Sprite[] _sprite;

    #endregion
    
    [SerializeField]
    private GameObject _bigTrap;
    private BigTrap _trap;

    // Use this for initialization
    void Start ()
    {
        _myRenderer = this.transform.FindChild("PlayerSprite").GetComponent<SpriteRenderer>();
        _trap = _bigTrap.GetComponent<BigTrap>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_trap.FlgTarget() == true) return;
        //移動
        Move(_gravity);

        //ビルボード
        Vector3 p = _mainCamera.transform.localPosition;
        transform.LookAt(p);
    }

    //移動処理
    void Move(float gravity)
    {
        Vector3 newPosition = transform.position;

        var cameraRight = Vector3.Scale(_mainCamera.transform.right, new Vector3(1, 1, 1)).normalized;
        _moveDirection = (Vector3.forward - Vector3.right) * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");
        newPosition += _moveDirection * Time.deltaTime * _speed;
        transform.position = new Vector3(Mathf.Clamp(newPosition.x, _clampX, 4.5f),
                                         newPosition.y,
                                         Mathf.Clamp(newPosition.z, -4.5f, _clampZ));
    }
}
