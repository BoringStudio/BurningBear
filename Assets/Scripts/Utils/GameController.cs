using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : Singleton<GameController>
{
    public TextMeshProUGUI counter;

    public float maxGameTime = 600.0f;
    private float _curTimer = 0.0f;

    void Start()
    {

    }

    void Update()
    {
        _curTimer += Time.deltaTime;
        counter.text = ((int)_curTimer).ToString();
    }

    public void DoGameOver()
    {
        CursorController.Instance.SetNormalCursor();
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }

    public void DoGameWin()
    {
        SceneManager.LoadScene("GameWinScene", LoadSceneMode.Single);
    }
}
