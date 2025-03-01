using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Animator transition;
    public float transitonTime = 1f;
    public static Scene activeScene;

    private void Awake()
    {
        activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex != 0)
            GameManager.Instance.sceneController = this;
    }

    public void LoadScene(string sceneName)
    {
        if(GameManager.Instance != null && GameManager.Instance.gameIsPaused)
            GameManager.Instance.TogglePauseGame();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitonTime);

        //Kind of jank way to delay this until scene fades
        if(activeScene.buildIndex == 2) //Gameplay
        {
            PersistInventoryHandler.Instance.ResetInventoryCache(); //Should fix inv cache randomly loading the previous floors inventory state
            PersistInventoryHandler.Instance.CachePlayerItems();
        }
        SceneManager.LoadScene(sceneName);
    }
}
