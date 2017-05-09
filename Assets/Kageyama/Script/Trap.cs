using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public enum TrapState
    {
        WAIT,       //動物が掛かるのを待っている状態
        CAPTURE,    //動物を捕まえている状態
    }

    public TrapState _state;

    #region 鋏むときに必要な変数
    //鋏むんでいるオブジェクトを入れる
    [SerializeField]
    private GameObject _targetAnimal;
    #endregion

    //スタートよりも前に動く変数
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        _targetAnimal = null;
        _state = TrapState.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //当たっている最中も取得する当たり判定
    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Enemy")
        {
            ChengeState(TrapState.CAPTURE);
        }
    }

    public void ChengeState(TrapState state)
    {
        _state = state;
    }
}
