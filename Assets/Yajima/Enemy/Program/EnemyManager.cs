using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // 暴れ状態なし
    // トラバサミにはさまれた時の暴れ移動
    // 小さいハサミ->小さい敵 大きいハサミ->大きい敵
    public Vector3 TrapHitRageMove()
    {
        return Vector3.zero;
    }

    // これらは暴れ状態なし
    // 罠になった時の行動
    // 大きいハサミ->小さい敵

    // 通常行動
    // 小さいハサミ->大きい敵

    //public void OnWillRenderObject()
    //{
    //    if (m_MainCamera.tag != "MainCamera") return;
    //    // 見えている
    //    m_IsRendered = true;
    //}

}
