using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public float maxGameTime = 600.0f;
    private float _curTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _curTimer += Time.deltaTime;
        if (_curTimer >= maxGameTime)
        {
            DoGameOver();
        }
    }

    public void DoGameOver()
    {
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
    }
}
