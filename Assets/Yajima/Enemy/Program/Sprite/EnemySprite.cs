using UnityEngine;
using System.Collections;

public class EnemySprite : MonoBehaviour {

    public Sprite[] m_Sprites;          // スプライト配列
    public GameObject m_Child;          // 子スプライト

    private SpriteRenderer m_Render;    // スプライトレンダラー
    private DirectionType m_DType = 
        DirectionType.DIRECTION_LEFT;   // ディレクションタイプ

    public enum SpriteNumber
    {
        MOVE_RIGHT_NUMBER = 0,
        MOVE_FORWARD_NUMBER = 1,
        MOVE_BACKWARD_NUMBER = 2
    }

    private enum DirectionType
    {
        DIRECTION_RIGHT,
        DIRECTION_LEFT
    }

	// Use this for initialization
	void Start () {
        ChackRender();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // スプライトの変更
    //public void ChangeSprite(SpriteNumber number)
    //{
    //    m_Render.sprite = m_Sprites[(int)number];
    //}
    public void ChangeSprite(int number, float directionX = 1.0f)
    {
        m_Render.sprite = m_Sprites[number];
        // 画像の方向を変える
        var type = DirectionType.DIRECTION_RIGHT;
        if (directionX < 0.3) type = DirectionType.DIRECTION_LEFT;

        if (m_DType == type) return;
        m_Child.transform.Rotate(Vector3.up, 180.0f);
        m_DType = type;
        //print("回転:" + directionX.ToString());
    }

    // レンダラーが設定されているかを確認します
    public void ChackRender()
    {
        if (m_Render != null) return;
        m_Render = m_Child.GetComponent<SpriteRenderer>();
    }
}
