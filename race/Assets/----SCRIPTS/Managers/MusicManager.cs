using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicTrack
{
    public string TrackName;
    public string TrackArtist;
    public float TrackLength;

    public float TrackVolume;

    public AudioClip clip;
}

public class MusicManager : MonoBehaviour
{
    [SerializeField] float timeToPlay;
    [SerializeField] int LastSongPlayed=-1;

    public List<MusicTrack> musicTracks;

    GameManager gm;
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }



    private void Update()
    {
        timeToPlay -= Time.deltaTime;
        if (timeToPlay < 0)
        {
            ChangeSong();
        }

        if (musicPlayerSource.volume != musicTracks[LastSongPlayed].TrackVolume * gm.MasterVolume * gm.MusicVolume)
            musicPlayerSource.volume = musicTracks[LastSongPlayed].TrackVolume * gm.MasterVolume * gm.MusicVolume;
    }

    public AudioSource musicPlayerSource;
    void ChangeSong()
    {
        int newSong = Random.Range(0, musicTracks.Count);
        if (musicTracks.Count > 1)
        {
            while (newSong == LastSongPlayed)
            {
                newSong = Random.Range(0, musicTracks.Count);
            }
        }
        else
        {
            newSong = 0;
        }
        LastSongPlayed = newSong;

        MusicTrack track = musicTracks[newSong];

        float newLength = track.TrackLength + Random.Range(5f, 20f);
        timeToPlay = newLength;

        musicPlayerSource.clip = track.clip;
        musicPlayerSource.Play();
    }
}
