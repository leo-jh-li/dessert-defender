using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject titleScreen;
    [SerializeField]
    private GameObject aboutScreen;

    void Awake() {
        aboutScreen.SetActive(false);
    }

    public void BeginGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToTitle() {
        aboutScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void GoToAbout() {
        titleScreen.SetActive(false);
        aboutScreen.SetActive(true);
    }
}
