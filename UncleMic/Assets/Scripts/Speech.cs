using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
              
public class Speech : MonoBehaviour 
{
    public static Speech instance;

    public Voice voice;
    public GCTextToSpeech speechManager;
    public AudioSource audioSource;

	private void Start()
	{
        instance = this;
        speechManager.SynthesizeSuccessEvent += SynthesizeSuccessEvent;
	}

    public void Speak (string text) 
    {
        string content = text;

        if (string.IsNullOrEmpty(content) || voice == null)
            return;

        VoiceConfig voiceCon = new VoiceConfig()
        {
            gender = voice.ssmlGender,
            languageCode = voice.languageCodes[0],
            name = voice.name
        };

        speechManager.Synthesize(content, voiceCon, false, 1, 1, voice.naturalSampleRateHertz);
    }

    private void SynthesizeSuccessEvent(PostSynthesizeResponse response)
    {
        audioSource.clip = speechManager.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
        audioSource.Play();
    }
}
