using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    class MenuController : MonoBehaviour
    {
        public string nextSceneName;

        public void LoadIntro()
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }

        public void LoadMain()
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
