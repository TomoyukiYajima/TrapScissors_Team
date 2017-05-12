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
    private int _score;
    // Use this for initialization
    void Start()
    {
        _rstate = ResultScoreState.FIRST;
        _score = 0;

    }

    // Update is called once per frame
    void Update()
    {
       // _score = GameManager.gameManager.GetScore();
    }
    void Result()
    {
        if (_score >= 100&&_score<200)  //適当
        {
            ChengeScoreState(ResultScoreState.SECOND);
        }
        else if (_score >= 200)         //適当
        {
            ChengeScoreState(ResultScoreState.THIRD);
        }
    }
    public void ChengeScoreState(ResultScoreState rstate)
    {
        _rstate = rstate;
    }
}