using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private string _gameOverScene = "GameOverScene";

    [SerializeField] private TextMeshProUGUI _counter;
    [SerializeField] private CursorController _cursorController;

    private float _curTimer = 0.0f;
    private int _currentSeconds = -1;

    void Awake()
    {
        Assert.IsNotNull(_cursorController, "[GameController]: Counter text is null");
        Assert.IsNotNull(_cursorController, "[GameController]: Cursor controller is null");
    }

    void Update()
    {
        _curTimer += Time.deltaTime;

        var seconds = (int)_curTimer;
        if (_currentSeconds != seconds)
        {
            _counter.text = $"{seconds / 60}:{seconds % 60}";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void DoGameOver()
    {
        _cursorController.SetNormalCursor();
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
    }
}
