using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject menu;
    private void Start()
    {
        menu = gameObject;
        menu.SetActive(true);
    }
    public void StartButton()
    {
        PlayerManager.instance.gameStarted = true;
        menu.SetActive(false);
    }
}
