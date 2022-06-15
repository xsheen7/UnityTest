using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;

public class SoundMgr : UnitySingleton<SoundMgr>
{
    const int MAX_SOUNDS = 8;
    private AudioSource music;
    private AudioSource[] sounds = new AudioSource[MAX_SOUNDS];

    
    private int music_muted;
    private int sound_muted;

    private float music_volume;
    private float sound_volume;

    private int now_soundid = 0;

	public override void Awake () {
        base.Awake();

        this.music_muted = PlayerPrefs.GetInt("music_muted", 0);
        this.sound_muted = PlayerPrefs.GetInt("sound_muted", 0);
        this.music_volume = PlayerPrefs.GetFloat("music_volume", 1.0f);
        
        this.sound_volume = PlayerPrefs.GetFloat("sound_volume", 1.0f);


        this.music = this.gameObject.AddComponent<AudioSource>();
        this.music.mute = (this.music_muted == 1);
        this.music.volume = (this.music_volume);

        for (int i = 0; i < MAX_SOUNDS; i++) {
            this.sounds[i] = this.gameObject.AddComponent<AudioSource>();
            this.sounds[i].mute = (this.sound_muted == 1);
            this.sounds[i].volume = (this.sound_volume);
        }
	}

    public void play_music(string url, bool loop = true)
    {
        AudioClip clip =  AssetBundleManager.Instance.GetAssetCache(url) as AudioClip;
        // AudioClip clip = (AudioClip)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/AssetsPackage/Sounds/bgm_scene1.ogg", typeof(AudioClip));
        this.music.clip = clip;
        this.music.loop = loop;
        this.music.Play();
    }

    public int play_sound(string url, bool loop = false)
    {
        int soundid = this.now_soundid;
        AudioClip clip = AssetBundleManager.Instance.GetAssetCache(url) as AudioClip;

        this.sounds[this.now_soundid].clip = clip;
        this.sounds[this.now_soundid].loop = loop;
        this.sounds[this.now_soundid].Play();

        this.now_soundid ++;
        this.now_soundid = (this.now_soundid >= this.sounds.Length) ? 0 : this.now_soundid;
        return soundid;
    }

    public int play_one_shot(string url, bool loop = false)
    {
        int soundid = this.now_soundid;
        AudioClip clip = AssetBundleManager.Instance.GetAssetCache(url) as AudioClip;

        this.sounds[this.now_soundid].clip = clip;
        this.sounds[this.now_soundid].loop = loop;
        this.sounds[this.now_soundid].PlayOneShot(clip);

        this.now_soundid++;
        this.now_soundid = (this.now_soundid >= this.sounds.Length) ? 0 : this.now_soundid;
        return soundid;
    }

    public void stop_sound(int soundid)
    {
        if (soundid < 0 || soundid >= this.sounds.Length) {
            return;
        }

        this.sounds[soundid].Stop();
        this.sounds[this.now_soundid].clip = null;
    }

    public void stop_music()
    {
        this.music.Stop();
        this.music.clip = null;
    }

    public void stop_all_sounds()
    {
        for (int i = 0; i < this.sounds.Length; i++) {
            this.sounds[i].Stop();
            this.sounds[i].clip = null;
        }
    }

    public void set_music_mute(bool mute)
    {
        if (mute == (this.music_muted == 1)) {
            return;
        }

        this.music_muted = mute ? 1 : 0;
        this.music.mute = mute;

        PlayerPrefs.SetInt("music_muted", this.music_muted);
    }

    public void set_sound_mute(bool mute)
    {
        if (mute == (this.sound_muted == 1)) {
            return;
        }

        this.sound_muted = (mute) ? 1 : 0;
        for (int i = 0; i < MAX_SOUNDS; i++) {
            this.sounds[i].mute = mute;
        }

        PlayerPrefs.SetInt("sound_muted", this.sound_muted);
    }

    public void set_music_volume(float value)
    {
        if (value < 0.0f || value > 1.0f) {
            return;
        }

        this.music_volume = value;
        this.music.volume = this.music_volume;
        PlayerPrefs.SetFloat("music_volume", this.music_volume);
    }

    public void set_sound_volume(float value)
    {
        if (value < 0.0f || value > 1.0f) {
            return;
        }

        this.sound_volume = value;
        for (int i = 0; i < MAX_SOUNDS; i++) {
            this.sounds[i].volume = value;
        }

        PlayerPrefs.SetFloat("sound_volume", this.sound_volume);
    }

    public bool musicMuted {
        get {
            return (this.music_muted == 1);
        }
    }

    public bool soundMuted {
        get {
            return (this.sound_muted == 1);
        }
    }

    public float musicVolume {
        get {
            return this.music_volume;
        }
    }

    public float soundVolume {
        get {
            return this.sound_volume;
        }
    }
}
