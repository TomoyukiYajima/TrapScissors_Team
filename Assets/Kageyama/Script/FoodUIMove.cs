using UnityEngine;
using System.Collections;

public class FoodUIMove : MonoBehaviour
{
    public GameObject[] _foodUI;
    [SerializeField]
    private int _selectFood;

	// Use this for initialization
	void Start ()
    {
        _selectFood = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            LeftRotation();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            RightRotation();
        }
	}

    /// <summary>
    /// 右回りに回転する
    /// </summary>
    public void LeftRotation()
    {
        _selectFood++;
        if (_selectFood >= _foodUI.Length) _selectFood = 0;
        _foodUI[_selectFood].transform.SetAsLastSibling();
        for (int i = 0; i < _foodUI.Length; i++)
        {
            _foodUI[i].GetComponent<FoodUI>().RightMoveRotation(SelectFood());
        }
    }

    /// <summary>
    /// 左回りに回転する
    /// </summary>
    public void RightRotation()
    {
        _selectFood--;
        if (_selectFood < 0) _selectFood = _foodUI.Length - 1;
        _foodUI[_selectFood].transform.SetAsLastSibling();
        for (int i = 0; i < _foodUI.Length; i++)
        {
            _foodUI[i].GetComponent<FoodUI>().LeftMoveRotation(SelectFood());
        }
    }

    /// <summary>
    /// どのの餌を選んでいるか値を返す
    /// </summary>
    /// <returns></returns>
    public GameObject SelectFood()
    {
        return _foodUI[_selectFood];
    }
}
