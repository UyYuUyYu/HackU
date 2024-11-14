using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetumeiSceneScript : MonoBehaviour
{
    public void BackScene()
    {
        SceneManager.LoadScene("Start");
    }

    public void NextInfo()
    {
        print("NextInfo");
    }

}
