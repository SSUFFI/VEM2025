
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Fade")]
    [Range(0f, 3f)]
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Range(0f, 3f)]
    [SerializeField] private float fadeInDuration = 0.5f;

    [Space(15)]
    [Header("========== Scene BGM ==========")]
    [SerializeField] private AudioClip titleBGM;
    [Range(0f, 1f)][SerializeField] private float titleVolume = 1f;

    [SerializeField] private AudioClip townBGM;
    [Range(0f, 1f)][SerializeField] private float townVolume = 1f;

    [SerializeField] private AudioClip mapBGM;
    [Range(0f, 1f)][SerializeField] private float mapVolume = 1f;

    [SerializeField] private AudioClip battleBGM;
    [Range(0f, 1f)][SerializeField] private float battleVolume = 1f;

    [Space(15)]
    [Header("========== TownPanel ==========")]
    [SerializeField] private AudioClip templeBGM;
    [Range(0f, 1f)][SerializeField] private float templeVolume = 1f;

    [SerializeField] private AudioClip royalBGM;
    [Range(0f, 1f)][SerializeField] private float royalVolume = 1f;

    [SerializeField] private AudioClip shopBGM;
    [Range(0f, 1f)][SerializeField] private float shopVolume = 1f;

    [Space(15)]
    [Header("========== EventPanel ==========")]
    [SerializeField] private AudioClip healBGM;
    [Range(0f, 1f)][SerializeField] private float healVolume = 1f;

    [SerializeField] private AudioClip exploreBGM;
    [Range(0f, 1f)][SerializeField] private float exploreVolume = 1f;

    [SerializeField] private AudioClip npcBGM;
    [Range(0f, 1f)][SerializeField] private float npcVolume = 1f;

    [SerializeField] private AudioClip slateBGM;
    [Range(0f, 1f)][SerializeField] private float slateVolume = 1f;

    [Space(15)]
    [Header("========== Mouse SFX ==========")]
    [SerializeField] private AudioClip mouseClickSFX;
    [Range(0f, 1f)]
    [SerializeField] private float mouseClickVolume = 1f;

    [Space(15)]
    [Header("========== Card SFX ==========")]
    [SerializeField] private AudioClip cardDrawSFX;
    [Range(0f, 1f)]
    [SerializeField] private float cardDrawVolume = 1f;

    [SerializeField] private AudioClip cardHoverSFX;
    [Range(0f, 1f)]
    [SerializeField] private float cardHoverVolume = 1f;

    [Space(15)]
    [Header("========== Damage SFX ==========")]
    [SerializeField] private AudioClip damageSFX;

    [Range(0f, 1f)]
    [SerializeField] private float damageVolume = 1f;

    private struct BGMState
    {
        public AudioClip clip;
        public float time;
    }

    private Stack<BGMState> bgmStack = new Stack<BGMState>();

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM(titleBGM);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1))
        {
            PlayMouseClickSFX();
        }
    }

    public void PlayMouseClickSFX()
    {
        if (sfxSource == null || mouseClickSFX == null)
            return;

        sfxSource.PlayOneShot(mouseClickSFX, mouseClickVolume);
    }

    public void PlayCardDrawSFX()
    {
        if (sfxSource == null || cardDrawSFX == null)
            return;

        sfxSource.PlayOneShot(cardDrawSFX, cardDrawVolume);
    }

    public void PlayDamageSFX()
    {
        if (sfxSource == null || damageSFX == null)
            return;

        sfxSource.PlayOneShot(damageSFX, damageVolume);
    }

    public void PlayCardHoverSFX()
    {
        if (sfxSource == null || cardHoverSFX == null)
            return;

        sfxSource.PlayOneShot(cardHoverSFX, cardHoverVolume);
    }

    private float GetBGMVolume(AudioClip clip)
    {
        if (clip == titleBGM) return titleVolume;
        if (clip == townBGM) return townVolume;
        if (clip == mapBGM) return mapVolume;
        if (clip == battleBGM) return battleVolume;

        if (clip == templeBGM) return templeVolume;
        if (clip == royalBGM) return royalVolume;
        if (clip == shopBGM) return shopVolume;

        if (clip == healBGM) return healVolume;
        if (clip == exploreBGM) return exploreVolume;
        if (clip == npcBGM) return npcVolume;
        if (clip == slateBGM) return slateVolume;

        return 1f;
    }

    private void SetBGM(AudioClip clip, float startTime = 0f)
    {
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;

        if (clip == null)
            return;

        bgmSource.time = startTime;
        bgmSource.Play();
    }

    private void ChangeBGM(AudioClip clip, float startTime = 0f)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine = StartCoroutine(
            FadeBGM(clip, startTime)
        );
    }

    private IEnumerator FadeBGM(AudioClip clip, float startTime)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = fadeOutDuration > 0f
                ? Mathf.Clamp01(elapsed / fadeOutDuration)
                : 1f;

            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        bgmSource.volume = 0f;

        SetBGM(clip, startTime);

        if (clip == null)
        {
            fadeCoroutine = null;
            yield break;
        }

        float targetVolume = GetBGMVolume(clip);
        elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = fadeInDuration > 0f
                ? Mathf.Clamp01(elapsed / fadeInDuration)
                : 1f;

            bgmSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        bgmSource.volume = targetVolume;
        fadeCoroutine = null;
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmStack.Clear();
        ChangeBGM(clip);
    }

    public void PlayTownBGM() => PlayBGM(townBGM);
    public void PlayMapBGM() => PlayBGM(mapBGM);
    public void PlayBattleBGM() => PlayBGM(battleBGM);

    private void OpenPanelBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmStack.Push(new BGMState
        {
            clip = bgmSource.clip,
            time = bgmSource.time
        });

        ChangeBGM(clip);
    }

    public void ClosePanelBGM()
    {
        if (bgmStack.Count == 0) return;

        BGMState previous = bgmStack.Pop();

        ChangeBGM(previous.clip, previous.time);
    }

    public void OpenTempleBGM() => OpenPanelBGM(templeBGM);
    public void OpenRoyalBGM() => OpenPanelBGM(royalBGM);
    public void OpenShopBGM() => OpenPanelBGM(shopBGM);

    public void OpenHealBGM() => OpenPanelBGM(healBGM);
    public void OpenExploreBGM() => OpenPanelBGM(exploreBGM);
    public void OpenNPCBGM() => OpenPanelBGM(npcBGM);
    public void OpenSlateBGM() => OpenPanelBGM(slateBGM);
}
