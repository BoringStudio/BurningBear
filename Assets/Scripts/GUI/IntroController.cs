using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GUI
{
    class IntroController : MonoBehaviour
    {
        public string nextSceneName;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                LoadMain();
            }
        }

        public void LoadMain()
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}
