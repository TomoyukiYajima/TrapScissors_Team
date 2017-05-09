using UnityEngine;
using System.Collections;

public class AnimalManager : MonoBehaviour {

    private static int ANIMAL_COUNT = 0;

	//// Use this for initialization
	//void Start () {
	
	//}
	
	//// Update is called once per frame
	//void Update () {
	
	//}

    // 動物カウントの追加
    public void AddAnimalCount(int count) { ANIMAL_COUNT += count; }

    // 動物カウントの値を取得します
    public int GetAnimalCount() { return ANIMAL_COUNT; }
}
