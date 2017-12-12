using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class oulAudio
{
    //===============================================
    //	メンバ変数
    //===============================================
    static GameObject gameObject = null;
    // サウンドリソース
    static AudioSource sourceBGM = null;         // BGM
    static AudioSource sourceSEDefault = null;   // SE (デフォルト)
    static AudioSource[] sourcesSE = new AudioSource[NumSEChannel];              // SE (チャンネル)

    // BGMにアクセスするためのテーブル
    static Dictionary<string, SoundData> BGMTable = new Dictionary<string, SoundData>();
    // SEにアクセスするためのテーブル 
    static Dictionary<string, SoundData> SETable = new Dictionary<string, SoundData>();

    // 定数
    const int NumSEChannel = 4;
    enum TYPE { BGM, SE }

    public enum seID
    {
        //Cursor0,
        //FullCombo,
        //FullComboVoice,
        //Cool,
        //Nice,
        //Bad
    }

    // 定数
    //static Constant constant;


    //===============================================
    //	サウンドデータ
    //===============================================
    class SoundData
    {
        // アクセス用のキー
        public string m_sKey;
        // ファイル名
        public string m_sFileName;
        // AudioClip
        public AudioClip m_Clip;

        // コンストラクタ
        public SoundData(string sKey, string sFileName)
        {
            m_sKey = sKey;
            m_sFileName = sFileName;
            // AudioClipの取得
            m_Clip = Resources.Load(sFileName) as AudioClip;
        }
    }

    //===============================================
    //	コンストラクタ
    //===============================================
    public static void Initialize()
    {
        LoadSE("Ketudoramu", "SE/Ketudoramu");

        LoadSE("action_ability", "SE/shadowverse/action_ability");
        LoadSE("card_create", "SE/shadowverse/card_create");
        LoadSE("card_draw0", "SE/shadowverse/card_draw0");
        LoadSE("card_draw1", "SE/shadowverse/card_draw1");
        LoadSE("card_hold", "SE/shadowverse/card_hold");
        LoadSE("cursor0", "SE/shadowverse/cursor0");
        LoadSE("decide0", "SE/shadowverse/decide0");
        LoadSE("decide1", "SE/shadowverse/decide1");
        LoadSE("decide2", "SE/shadowverse/decide2");
        LoadSE("decide3", "SE/shadowverse/decide3");
        LoadSE("edit_end", "SE/shadowverse/edit_end");
        LoadSE("edit_out", "SE/shadowverse/edit_out");
        LoadSE("edit_set", "SE/shadowverse/edit_set");
        LoadSE("ether", "SE/shadowverse/ether");
        LoadSE("fire0", "SE/shadowverse/fire0");
        LoadSE("gacha_comon", "SE/shadowverse/gacha_comon");
        LoadSE("gacha_legend", "SE/shadowverse/gacha_legend");
        LoadSE("edit_set", "SE/shadowverse/edit_set");
        LoadSE("gacha_rare", "SE/shadowverse/gacha_rare");
        LoadSE("gacha_s_rare", "SE/shadowverse/gacha_s_rare");
        LoadSE("phase", "SE/shadowverse/phase");
        LoadSE("select0", "SE/shadowverse/select0");
        LoadSE("up", "SE/shadowverse/up");
        LoadSE("Win", "SE/shadowverse/Win");

        LoadSE("Attack_Extra", "SE/Main/Attack_Extra");
        LoadSE("Attack_Middle", "SE/Main/Attack_Middle");
        LoadSE("Attack_Mini", "SE/Main/Attack_Mini");
        LoadSE("Attack_Strong", "SE/Main/Attack_Strong");


        //LoadSE("Trigger0", "Customizes/SE/Trigger0");
        //LoadSE("Wolf", "Sound/SE/wolf");
        //LoadSE("UnlimitedWolf", "Sound/SE/unlimited_wolf");
        //LoadSE("Sacrifice", "Sound/SE/sacrifice");
        //LoadSE("Sacrifice", "Sound/SE/sacrifice");
        //LoadSE("Fire", "Sound/SE/fire");
        //LoadSE("BadMeat", "Sound/SE/bad");
        //LoadSE("GreatMeat", "Sound/SE/great");
        //LoadSE("PerfectMeat", "Sound/SE/perfect");
        //LoadSE("PerfectMoment", "Sound/SE/imadesu");
        //LoadSE("CatchSheep", "Sound/SE/catch");
        //LoadSE("CatchRealSheep", "Sound/SE/catch");
        //LoadSE("FatIn", "Sound/SE/fat_in");
        //LoadSE("BeFat", "Sound/SE/be_fat");
        //foreach(string str in System.Enum.GetNames(typeof(seID)))
        //{
        //    LoadSE(str, "Sound/SE/" + str);
        //}
        LoadBGM("CollectionBGM", "BGM/Collect");
        LoadBGM("tori", "BGM/tori");
        LoadBGM("RisingWinter", "BGM/Rising Winter");
        // 定数
        //constant = GameObject.Find("Util").GetComponent<Constant>();
    }


    //===============================================
    //	ロード関数
    //===============================================
    static public void LoadBGM(string key, string fileName)
    {
        // すでに登録済みだったら、登録しているのを消す
        if (BGMTable.ContainsKey(key)) BGMTable.Remove(key);
        // リスト追加
        BGMTable.Add(key, new SoundData(key, fileName));
    }
    static public void LoadSE(string key, string fileName)
    {
        // すでに登録済みだったら、登録しているのを消す
        if (SETable.ContainsKey(key)) SETable.Remove(key);
        // リスト追加
        SETable.Add(key, new SoundData(key, fileName));
    }
    static public void SetAudioClip(seID id, AudioClip clip)
    {
        var key = id.ToString();

        // 対応するキーがないかチェック
        if (!SETable.ContainsKey(key)) return;

        // オーディオクリップ変更
        SETable[key].m_Clip = clip;
    }

    //===============================================
    //	BGM制御関数
    //===============================================
    // 再生
    static public bool PlayBGM(string key, bool isLoop)
    {
        // 対応するキーがないかチェック
        if (!BGMTable.ContainsKey(key)) return false;

        // いったん止める
        StopBGM();

        // リソースの取得
        var l_Data = BGMTable[key];

        // 再生
        var l_Source = GetAudioSource(TYPE.BGM);
        l_Source.loop = isLoop;
        l_Source.clip = l_Data.m_Clip;
        l_Source.Play();

        return true;
    }
    // 停止
    static public void StopBGM()
    {
        GetAudioSource(TYPE.BGM).Stop();
    }
    static public bool isPlayBGM()
    {
        var l_Source = GetAudioSource(TYPE.BGM);
        if (!l_Source) return false;

        return l_Source.isPlaying;
    }
    // 再生時間の取得(ミリ秒単位)
    static public int GetMSecondBGM()
    {
        var time = GetAudioSource(TYPE.BGM).time;
        return (int)(time * 1000);
    }


    //===============================================
    //	SE制御関数
    //===============================================
    // 再生
    static public void PlaySE(string id, int channel = -1)
    {
        var key = id.ToString();

        // 対応するキーがないかチェック
        if (!SETable.ContainsKey(key)) return;

        // リソースの取得
        var data = SETable[key];

        AudioSource source;
        if (0 <= channel && channel < NumSEChannel)
        {
            // チャンネル指定
            source = GetAudioSource(TYPE.SE, channel);
            source.clip = data.m_Clip;
            source.Play();
        }
        else
        {
            // デフォルト
            source = GetAudioSource(TYPE.SE);

            // SEのボリューム設定
            //source.volume = constant.seVolume;

            // 再生
            source.PlayOneShot(data.m_Clip);
        }
        //return source;
    }

    static public void PlaySE(seID id, int channel = -1)
    {
        PlaySE(id.ToString());
    }




    //===============================================
    //	オーディオソース取得
    //===============================================
    static AudioSource GetAudioSource(TYPE eType, int channel = -1)
    {
        if (!gameObject)
        {
            // GameObjectがなければ作る
            gameObject = new GameObject("Audio");
            // 破棄しないようにする
            GameObject.DontDestroyOnLoad(gameObject);
            // AudioSourceを作成
            sourceBGM = gameObject.AddComponent<AudioSource>();
            sourceSEDefault = gameObject.AddComponent<AudioSource>();
            for (int i = 0; i < NumSEChannel; i++)
            {
                sourcesSE[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        //// チャンネル確保
        //sourcesSE = new AudioSource[NumSEChannel];

        //// AudioSourceを作成
        //sourceBGM = gameObject.AddComponent<AudioSource>();
        //sourceSEDefault = gameObject.AddComponent<AudioSource>();
        //for (int i = 0; i < NumSEChannel; i++)
        //{
        //    sourcesSE[i] = gameObject.AddComponent<AudioSource>();
        //}

        switch (eType)
        {
            case TYPE.BGM:
                // BGM
                return sourceBGM;

            case TYPE.SE:
                if (0 <= channel && channel < NumSEChannel)
                {
                    // チャンネル指定
                    return sourcesSE[channel];
                }
                else
                {
                    // デフォルト
                    return sourceSEDefault;
                }

            default:
                return null;
        }
    }
}
//public class SoundManager
//{
//    GameObject m_SoundPlayerObj;
//    AudioSource m_AudioSource;
//    Dictionary<string, AudioClipInfo> m_AudioClips = new Dictionary<string, AudioClipInfo>();

//    // AudioClip information
//    class AudioClipInfo
//    {
//        public string m_sResourceName;
//        public string m_sID;
//        public AudioClip m_Clip;

//        public AudioClipInfo(string sResourceName, string sID)
//        {
//            m_sResourceName = sResourceName;
//            m_sID = sID;
//        }
//    }

//    public SoundManager()
//    {
//        m_AudioClips.Add("SE01_SUPER", new AudioClipInfo("Sound/SE/Customize/SE01/super", "SE01_SUPER"));
//    }

//    public bool PlaySE(string sID)
//    {
//        if (m_AudioClips.ContainsKey(sID) == false)
//            return false; // not register

//        AudioClipInfo info = m_AudioClips[sID];

//        // Load
//        if (!info.m_Clip)
//            info.m_Clip = (AudioClip)Resources.Load(info.m_sResourceName);

//        if (!m_SoundPlayerObj)
//        {
//            m_SoundPlayerObj = new GameObject("SoundPlayer");
//            m_AudioSource = m_SoundPlayerObj.AddComponent<AudioSource>();
//        }

//        // Play SE
//        m_AudioSource.PlayOneShot(info.m_Clip);

//        return true;
//    }







//    class BGMPlayer
//    {
//        // State
//        class State
//        {
//            protected BGMPlayer m_Parent;
//            public State(BGMPlayer Parent)
//            {
//                m_Parent = Parent;
//            }
//            public virtual void Play() { }
//            public virtual void Pause() { }
//            public virtual void Stop() { }
//            public virtual void Update() { }
//        }

//        /*
//        ...ここにStateの派生クラスが並ぶ～...
//        */
//        class Wait : State
//        {

//            public Wait(BGMPlayer Parent) : base(Parent) { }

//            public override void Play()
//            {
//                if (m_Parent.m_fFadeInTime > 0.0f)
//                    m_Parent.m_State = new FadeIn(m_Parent);
//                else
//                    m_Parent.m_State = new Playing(m_Parent);
//            }
//        }

//        class FadeIn : State
//        {
//            float t = 0.0f;

//            public FadeIn(BGMPlayer Parent) : base(Parent)
//            {
//                m_Parent.m_AudioSource.Play();
//                m_Parent.m_AudioSource.volume = 0.0f;
//            }

//            public override void Pause()
//            {
//                m_Parent.m_State = new Pause(m_Parent, this);
//            }

//            public override void Stop()
//            {
//                m_Parent.m_State = new FadeOut(m_Parent);
//            }

//            public override void Update()
//            {
//                t += Time.deltaTime;
//                m_Parent.m_AudioSource.volume = t / m_Parent.m_fFadeInTime;
//                if (t >= m_Parent.m_fFadeInTime)
//                {
//                    m_Parent.m_AudioSource.volume = 1.0f;
//                    m_Parent.m_State = new Playing(m_Parent);
//                }
//            }
//        }

//        class Playing : State
//        {

//            public Playing(BGMPlayer m_Parent) : base(m_Parent)
//            {
//                if (m_Parent.m_AudioSource.isPlaying == false)
//                {
//                    m_Parent.m_AudioSource.volume = 1.0f;
//                    m_Parent.m_AudioSource.Play();
//                }
//            }

//            public override void Pause()
//            {
//                m_Parent.m_State = new Pause(m_Parent, this);
//            }

//            public override void Stop()
//            {
//                m_Parent.m_State = new FadeOut(m_Parent);
//            }
//        }

//        class Pause : State
//        {

//            State m_PrevState;

//            public Pause(BGMPlayer m_Parent, State PrevState) : base(m_Parent)
//            {
//                m_PrevState = PrevState;
//                m_Parent.m_AudioSource.Pause();
//            }

//            public override void Stop()
//            {
//                m_Parent.m_AudioSource.Stop();
//                m_Parent.m_State = new Wait(m_Parent);
//            }

//            public override void Play()
//            {
//                m_Parent.m_State = m_PrevState;
//                m_Parent.m_AudioSource.Play();
//            }
//        }

//        class FadeOut : State
//        {
//            float initVolume;
//            float t = 0.0f;

//            public FadeOut(BGMPlayer m_Parent) : base(m_Parent)
//            {
//                initVolume = m_Parent.m_AudioSource.volume;
//            }

//            public override void Pause()
//            {
//                m_Parent.m_State = new Pause(m_Parent, this);
//            }

//            public override void Update()
//            {
//                t += Time.deltaTime;
//                m_Parent.m_AudioSource.volume = initVolume * (1.0f - t / m_Parent.m_fFadeOutTime);
//                if (t >= m_Parent.m_fFadeOutTime)
//                {
//                    m_Parent.m_AudioSource.volume = 0.0f;
//                    m_Parent.m_AudioSource.Stop();
//                    m_Parent.m_State = new Wait(m_Parent);
//                }
//            }
//        }

//        GameObject m_Obj;
//        AudioSource m_AudioSource;
//        State m_State;
//        float m_fFadeInTime = 0.0f;
//        float m_fFadeOutTime = 0.0f;

//        public BGMPlayer() { }

//        public BGMPlayer(string bgmFileName)
//        {
//            AudioClip clip = (AudioClip)Resources.Load(bgmFileName);
//            if (clip != null)
//            {
//                m_Obj = new GameObject("BGMPlayer");
//                m_AudioSource = m_Obj.AddComponent<AudioSource>();
//                m_AudioSource.clip = clip;
//                m_State = new Wait(this);
//            }
//            else
//                Debug.LogWarning("BGM " + bgmFileName + " is not found.");
//        }

//        public void destory()
//        {
//            if (m_AudioSource)
//                GameObject.Destroy(m_Obj);
//        }

//        public void PlayBGM()
//        {
//            if (m_AudioSource)
//                m_State.Play();
//        }

//        public void PlayBGM(float fFadeTime)
//        {
//            if (m_AudioSource)
//            {
//                m_fFadeInTime = fFadeTime;
//                m_State.Play();
//            }
//        }

//        public void PauseBGM()
//        {
//            if (m_AudioSource)
//                m_State.Pause();
//        }

//        public void StopBGM(float fFadeTime)
//        {
//            if (m_AudioSource)
//            {
//                m_fFadeOutTime = fFadeTime;
//                m_State.Stop();
//            }
//        }

//        public void UpdateBGM()
//        {
//            if (m_AudioSource)
//                m_State.Update();
//        }

//        public float GetBGMPosition()
//        {
//            return m_AudioSource.time;
//        }
//    }
//}