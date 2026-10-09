using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    [SerializeField]private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    public void SetMusicVolume (float sliderValue)
    {
        audioMixer.SetFloat("music", sliderValue);  
    }

    public void SetSFXVolume(float sliderValue)
    {
        
        audioMixer.SetFloat("sfx", sliderValue);
    }
}
