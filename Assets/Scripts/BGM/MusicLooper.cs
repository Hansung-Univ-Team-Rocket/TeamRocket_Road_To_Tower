using System.Collections;
using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    public AudioClip song1;
    public AudioClip song2;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(PlayLoop());
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            // 노래 1 재생
            audioSource.clip = song1;
            audioSource.Play();
            yield return new WaitForSeconds(song1.length);

            // 노래 2 재생
            audioSource.clip = song2;
            audioSource.Play();
            yield return new WaitForSeconds(song2.length);
        }
    }
}
