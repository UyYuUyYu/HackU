using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Input");
    }
    public void Setumei()
    {
        SceneManager.LoadScene("IntoroScene");
    }
}
