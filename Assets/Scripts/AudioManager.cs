using System.Threading;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip explosionClip, powerUp, powerDown, numberTriggerPositive,
     numberTriggerNegative, finishClip, gameOverClip;

    void Awake()
    {
        Instance = this;
    }
    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(explosionClip);
    }
    public void PlayPowerUp()
    {
        sfxSource.PlayOneShot(powerUp);
    }
    public void PlayPowerDown()
    {
        sfxSource.PlayOneShot(powerDown);
    }
    public void PlayNumberClipPositive()
    {
        sfxSource.PlayOneShot(numberTriggerPositive);
    }
    public void PlayNumberClipNegative()
    {
        sfxSource.PlayOneShot(numberTriggerNegative);
    }
    public void PlayFinish()
    {
        sfxSource.PlayOneShot(finishClip);
    }
    public void PlayGameOver()
    {
        sfxSource.PlayOneShot(gameOverClip);
    }
}
