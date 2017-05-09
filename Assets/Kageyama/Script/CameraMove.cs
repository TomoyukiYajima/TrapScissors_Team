using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    private GameObject _player = null;
    private Vector3 _offset = Vector3.zero;
    [SerializeField, TooltipAttribute("スムーズに追いかけるかどうか")]
    private bool _lerpFrag;
    [SerializeField, TooltipAttribute("追尾させるかどうか")]
    private bool _moveFrag;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _offset = transform.position - _player.transform.position;
    }

    void LateUpdate()
    {
        if (_moveFrag == false) return;

        Vector3 newPosition = transform.position;
        newPosition.x = _player.transform.position.x + _offset.x;
        newPosition.y = _player.transform.position.y + _offset.y;
        newPosition.z = _player.transform.position.z + _offset.z;
        //ピッタリと追いかける
        if (_lerpFrag == false) transform.position = newPosition;
        //スムーズに追いかける
        else transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);
    }

    /// <summary>
    /// スムーズに追いかけるときはtrue、ピッタリくっつくときはfalseにする
    /// </summary>
    /// <param name="frag">スムーズにするかどうか</param>
    public void LerpFragChenge(bool frag)
    {
        _lerpFrag = frag;
    }

    /// <summary>
    /// カメラを追尾させるかどうか(trueなら追尾させる、falseなら追尾させない)
    /// </summary>
    /// <param name="frag">追尾させるかどうか</param>
    public void MoveChenge(bool frag)
    {
        _moveFrag = frag;
    }
}
