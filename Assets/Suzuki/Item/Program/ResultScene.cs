using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultScene : MonoBehaviour
{
    private SpriteRenderer _sp;
    private SceneManagerScript _scene;


    
    // Use this for initialization
    void Start()
    {
        _scene = SceneManagerScript.FindObjectOfType<SceneManagerScript>();
        _sp = SpriteRenderer.FindObjectOfType<SpriteRenderer>();
        _sp.color = new Color(0, 0, 0, 0);
        Result();
    }

    // Update is called once per frame
    void Update()
    {
        Result();
    }
    void Result()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _scene.FadeBlack();
            _sp.color = new Color(0, 0, 0, 1);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            _scene.FadeWhite();
            _sp.color = new Color(0, 0, 0, 0);
        }
    }
}