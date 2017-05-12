using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultScene : MonoBehaviour
{
    public enum ResultScoreState
    {
        FIRST,       //Scoreが低い時
        SECOND,      //Scoreが中間の時
        THIRD,       //Scpreが高い時
    }

    public ResultScoreState _rstate;
    public int _addscore;   //出スコア
    private int _inscore;   //入スコア
    // Use this for initialization
    void Start()
    {
        _rstate = ResultScoreState.FIRST;
        _inscore = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // _inscore = GameManager.gameManager.GetScore();
        //AddScore(_addscore);
    }
    void StateChange()
    {
        if (_inscore >= 100&&_inscore<200)  //適当
        {
            ChangeScoreState(ResultScoreState.SECOND);
        }
        else if (_inscore >= 200)         //適当
        {
            ChangeScoreState(ResultScoreState.THIRD);
        }
    }
    public void ChangeScoreState(ResultScoreState rstate)
    {
        _rstate = rstate;
    }
    //public void AddScore(int _addscore)
    //{
    //    _addscore = _inscore;
    //}
}