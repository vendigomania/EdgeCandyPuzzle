using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource click;
    [SerializeField] private AudioSource win;
    [SerializeField] private AudioSource lose;

    public static SoundController Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void Click()
    {
        if (enabled) click.Play();
    }

    public void Win()
    {
        if (enabled) win.Play();
    }

    public void Lose()
    {
        if (enabled) lose.Play();
    }
}
