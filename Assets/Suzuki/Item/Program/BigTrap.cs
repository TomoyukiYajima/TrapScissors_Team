using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class BigTrap : MonoBehaviour
{
    public enum TrapState
    {
        WAIT,       //動物が掛かるのを待っている状態
        CAPTURE,    //動物を捕まえている状態
    }

    public TrapState _state;
    [SerializeField, TooltipAttribute("Resultで表示されるオブジェクト")]
    private GameObject _result;
    private bool _flg;
    #region 挟むときに必要な変数
    //鋏んでいるオブジェクトを入れる
    [SerializeField] 
    private GameObject _targetAnimal;
    #endregion
    private SceneManagerScript _scene;

    //スタートよりも前に動く変数
    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        _targetAnimal = null;
        _state = TrapState.WAIT;
        _flg = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //当たっている最中も取得する当たり判定
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "LargeEnemy" || col.tag == "SmallEnemy")
        {

            ChengeState(TrapState.CAPTURE);

            _targetAnimal = col.gameObject;
            _flg = true;
            SceneManagerScript.sceneManager.FadeBlack();
            _result.SetActive(true);
        }
    }

    public void ChengeState(TrapState state)
    {
        _state = state;
    }
    public bool FlgTarget()
    {
        return _flg;
    }
}
