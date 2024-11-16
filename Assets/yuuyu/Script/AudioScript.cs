using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
   void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //DontDestroyOnLoad(this.gameObject);
    }

    public void PlayStart()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
