using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreboard;

    public Text timer;

    float timerSeconds = 0;

    public GameObject completeScreen;
    BugSpawner spawner;

    IEnumerator timerRoutine;

    void Start() {
        spawner = FindObjectOfType<BugSpawner>();
        spawner.OnUpdateScore += updateScore;
        spawner.OnAllBugsDead += gameComplete;

        NewGame();
    }

    public void NewGame() {
        completeScreen.SetActive(false);
        timerRoutine = fixedTimer();
        StartCoroutine(timerRoutine);
    }

    IEnumerator fixedTimer() {
        while (true) {
            updateTimer();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void gameComplete() {
        StopCoroutine(timerRoutine);
        completeScreen.SetActive(true);
    }

    void updateTimer() {
        timerSeconds += 0.1f;
        timer.text = timerSeconds.ToString();
    }

    void updateScore(string score) {
        scoreboard.text = score + " bugs";
    }
}
