using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioClip simpleAttack;
    public static AudioClip hit;
    public static AudioClip shoryukken;
    public static AudioClip hadoken;
    public static AudioSource audiosrc;


    private void Start()
    {
        simpleAttack = Resources.Load<AudioClip>("baseAttack");
        shoryukken = Resources.Load<AudioClip>("shoryuken");
        hadoken = Resources.Load<AudioClip>("hadoken");
        hit = Resources.Load<AudioClip>("Hit");
        audiosrc = GetComponent<AudioSource>();
    }

    public static void Attack()
    {
       // audiosrc.PlayOneShot(simpleAttack);
    }

    public static void PlayHitSound()
    {
        audiosrc.PlayOneShot(hit);
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
