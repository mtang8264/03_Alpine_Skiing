using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public Part[] parts;
    [Range(0,1)]
    public float masterVolume;

    public bool playing;
    private bool played;
    public bool[] partsIn;

    public bool test;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        partsIn = new bool[parts.Length];
    }

    // Update is called once per frame
    void Update()
    {
        //If the player should be playing but hasn't started
        if(!played && playing)
        {
            //It has now started and play each part from the beginning at its goal volume
            played = true;
            for (int i = 0; i < parts.Length; i ++)
            {
                parts[i].Play(masterVolume);
            }
        }

        //If the player shouldn't be playing
        if (!playing)
        {
            //Set each bool to not be playing
            played = false;
            for (int i = 0; i < partsIn.Length; i++)
            {
                partsIn[i] = false;
            }
        }

        //Test every part
        for (int i = 0; i < parts.Length; i ++)
        {
            //Join if it is active or leave otherwise
            if(partsIn[i])
            {
                parts[i].Join(masterVolume);
            }
            else
            {
                parts[i].Leave();
            }
        }
    }

    public void Begin()
    {
        playing = true;
        for (int i = 0; i < parts.Length; i++)
        {
            if(parts[i].layer == 0)
            {
                partsIn[i] = true;
            }
            else
            {
                partsIn[i] = false;
            }
        }
    }
    public void PlayLayer(int layer)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if(parts[i].layer == layer)
            {
                partsIn[i] = true;
            }
        }
    }
    public void LeaveLayer(int layer)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if(parts[i].layer == layer)
            {
                partsIn[i] = false;
            }
        }
    }
}

[System.Serializable]
public class Part
{
    public string name;
    public AudioSource source;
    public int layer;
    [Range(0f,1f)]
    public float volume;

    public void Play()
    {
        source.volume = volume;
        source.Play();
    }
    public void Play(float v)
    {
        source.volume = volume * v;
        source.Play();
    }

    public void Leave()
    {
        source.volume = 0f;
    }
    public void Join()
    {
        source.volume = volume;
    }
    public void Join(float v)
    {
        source.volume = volume * v;
    }
}