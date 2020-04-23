using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;

    private int _currentAudioClip = 0;
    private string _mainScene;

    void Awake()
    {
        var siblings = GameObject.FindGameObjectsWithTag(tag);
        if (siblings.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        _mainScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _mainScene)
        {
            NextMusic();
        } else
        {
            StopMusic();
        }
    }

    public void NextMusic()
    {
        var source = FindObjectOfType<AudioSource>();

        source.clip = audioClips[_currentAudioClip];
        _currentAudioClip = (_currentAudioClip + 1) % audioClips.Length;

        source.Play();
    }

    public void StopMusic()
    {
        var source = FindObjectOfType<AudioSource>();
        source.Stop();
    }
}
