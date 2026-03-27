using UnityEngine;

public class KeepAudio : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, 1.0f);
    }

}