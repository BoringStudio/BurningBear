using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    class MenuController : MonoBehaviour
    {
        public string nextSceneName;

        public void LoadIntro()
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Additive);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
