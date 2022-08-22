using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{

    public void StartGame() {
        
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync() {
        //UIManager ui = FindObjectOfType<UIManager>();
        Text loadText = FindObjectOfType<Button>().GetComponentInChildren<Text>();
        loadText.text = ".";

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        while (!asyncLoad.isDone)
        {
            if (loadText.text.Length > 5) {
                loadText.text = ".";
            } else {
                loadText.text += " .";
            }

            yield return null;
        }

    }

    public void NewGame() {
        BugSpawner spawner = FindObjectOfType<BugSpawner>();
        spawner.totalBugs++;
        spawner.NewGame();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        player.NewGame();

        UIManager ui = FindObjectOfType<UIManager>();
        ui.NewGame();
    }
}
