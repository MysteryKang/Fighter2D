using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioClip simpleAttack;
    public static AudioClip shoryukken;
    public static AudioClip hadoken;
    public static AudioSource audiosrc;


    private void Start()
    {
        simpleAttack = Resources.Load<AudioClip>("baseAttack");
        shoryukken = Resources.Load<AudioClip>("shoryuken");
        hadoken = Resources.Load<AudioClip>("hadoken");
        audiosrc = GetComponent<AudioSource>();
    }

    public static void Attack()
    {
       // audiosrc.PlayOneShot(simpleAttack);
    }

    public static void PlayHadoken()
    {
        audiosrc.PlayOneShot(hadoken);
    }

    public static void Shoryuken()
    {
        audiosrc.PlayOneShot(shoryukken);
    }
         
    //public static void PlayCollisionSound()
    //{
    //    audiosrc.PlayOneShot(hitting);
    //}

    //public static void GameOver()
    //{
    //    audiosrc.PlayOneShot(death);
    //}

}
