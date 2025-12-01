using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sonido[] Musica;

    private string currentSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        foreach (Sonido s in Musica)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string nombre)
    {
        foreach (Sonido s in Musica)
        {
            if (nombre == s.name)
            {
                s.source.Play();
                if (!s.soundefect)
                    currentSong = s.name;
                return;
            }
        }
    }

    public void Stop(string nombre)
    {
        foreach (Sonido s in Musica)
        {
            if (nombre == s.name)
            {
                s.source.Stop();
                return;
            }
        }
        Debug.Log("Esa no la tenemos pa!");
    }

    internal void Play(object destruirCajas)
    {
        throw new NotImplementedException();
    }
}
