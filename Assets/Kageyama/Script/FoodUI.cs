using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    [SerializeField]
    private int _myNumberll;
    private float _toPosition;
    private FoodUIMove _foodUIMove;
    private RectTransform _myRect;
    private float _blackColor = 0.5f;
    private float _smallScale = 0.6f;
    private float[] _positions = {-35, 0, 35};
    [SerializeField]
    private int _posNumber;
    // Use this for initialization
    void Start()
    {
        _toPosition = _positions[_posNumber];
        _foodUIMove = this.transform.parent.GetComponent<FoodUIMove>();
        _myRect = this.GetComponent<RectTransform>();
        if(_foodUIMove.SelectFood() != this.gameObject)
        {
            _myRect.GetComponent<Image>().color = new Color(_blackColor, _blackColor, _blackColor, 1.0f);
            _myRect.localScale = new Vector2(_smallScale, _smallScale);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 自分が何番の餌なのか教えてもらう
    /// </summary>
    /// <param name="num"></param>
    public void SetNumber(int num)
    {
        _myNumberll = num;
    }

    public void RightMoveRotation(GameObject obje)
    {
        _posNumber++;
        if (_posNumber > 2) _posNumber = 0;
        LeanTween.cancel(this.gameObject);
        BlackFade(obje);
        _toPosition = _positions[_posNumber];
        LeanTween.move(_myRect, new Vector2(_toPosition, this.transform.localPosition.y), 0.1f);
    }

    public void LeftMoveRotation(GameObject obje)
    {
        _posNumber--;
        if (_posNumber < 0) _posNumber = 2;
        LeanTween.cancel(this.gameObject);
        BlackFade(obje);
        _toPosition = _positions[_posNumber];
        LeanTween.move(_myRect, new Vector2(_toPosition, this.transform.localPosition.y), 0.1f);
    }

    public void BlackFade(GameObject obje)
    {
        RectTransform _myRect = this.GetComponent<RectTransform>();
        if (obje != this.gameObject)
        {
            LeanTween.color(_myRect, new Color(_blackColor, _blackColor, _blackColor), 0.1f);
            LeanTween.scale(_myRect, new Vector2(_smallScale, _smallScale), 0.1f);
        }
        else
        {
            LeanTween.color(_myRect, new Color(1, 1, 1), 0.1f);
            LeanTween.scale(_myRect, new Vector2(1, 1), 0.1f);
        }
    }
}
