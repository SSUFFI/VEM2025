using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        if (AudioManager.Inst == null)
            return;

        if (bgmSlider != null)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.wholeNumbers = false;

            bgmSlider.SetValueWithoutNotify(
                AudioManager.Inst.BGMVolume
            );

            bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;

            sfxSlider.SetValueWithoutNotify(
                AudioManager.Inst.SFXVolume
            );

            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    private void OnDisable()
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(OnBGMChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }

    private void OnBGMChanged(float value)
    {
        AudioManager.Inst?.SetBGMVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Inst?.SetSFXVolume(value);
    }
}
