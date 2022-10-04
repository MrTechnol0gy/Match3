using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    public AudioSource gemSound, explodeSound, stoneSound, roundOverSound;

    public void PlayGemBreak()
    {
        gemSound.Stop(); //stops the gemsound from playing to prevent the sound from looping instead of being new

        gemSound.pitch = Random.Range(.8f, 1.2f); //modifies the pitch to give the impression of slightly different sounds

        gemSound.Play(); //plays the sound
    }
    public void PlayExplode()
    {
        explodeSound.Stop(); //stops the gemsound from playing to prevent the sound from looping instead of being new

        explodeSound.pitch = Random.Range(.8f, 1.2f);

        explodeSound.Play(); //plays the sound
    }
    public void PlayStoneBreak()
    {
        stoneSound.Stop(); //stops the gemsound from playing to prevent the sound from looping instead of being new

        stoneSound.pitch = Random.Range(.8f, 1.2f);

        stoneSound.Play(); //plays the sound
    }
    public void PlayRoundOver()
    {        
        roundOverSound.Play(); //plays the sound
    }
}
