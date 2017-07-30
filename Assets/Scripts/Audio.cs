using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace Neuromancers
{
	public class Audio : Singleton<Audio>
	{
		public enum SoundEffect
		{
			UIHoverOn,
			UIHoverOff,
			UISelect,
			ComfortRotation,
			Voiceover_ResettingOrientation,
			Voiceover_SelectTheScenes,
			Virus_Hit,
			Virus_Whoosh,
			Virus_WhooshFast,
			BloodVesselProjectileRelease,
			RERProjectileRelease,
			SwooshSlow,
			Spawn,
			DeskShow,
			Grip_Down,
			Grip_Up,
			Neuron_Fire1,
			Neuron_Fire2,
			Neuron_Fire3,
			Neuron_Fire4,
			Neuron_Fire5,
			Neuron_Fire6,
			Neuron_Fire7,
			Neuron_Fire8,
			Neuron_Fire9,
			Neuron_Fire10,
			Neuron_Fire11,
			Neuron_Fire12,
			Neuron_Fire13,
			Neuron_Click

		}


		protected Dictionary<SoundEffect,string> soundEffectToResourcePath = new Dictionary<SoundEffect, string>()
		{
			{SoundEffect.UIHoverOn,"UI_Hover_On"},
			{SoundEffect.UIHoverOff,"UI_Hover_Off"},
			{SoundEffect.UISelect,"UI_Select"},
			{SoundEffect.ComfortRotation,"ComfortRotation"},
			{SoundEffect.Voiceover_ResettingOrientation,"VO_ResettingOrientation"},
			{SoundEffect.Voiceover_SelectTheScenes,"VO_SelectTheScenes"},
			{SoundEffect.Virus_Hit,"C9_Hit_Audio"},
			{SoundEffect.Virus_Whoosh,"C9_Whoosh_Audio"},
			{SoundEffect.Virus_WhooshFast,"C9_WhooshFast_Audio"},
			{SoundEffect.BloodVesselProjectileRelease,"swoosh_05"},
			{SoundEffect.RERProjectileRelease,"swoosh_04"},
			{SoundEffect.SwooshSlow,"swoosh_05"},
			{SoundEffect.Spawn,"ui_casual_open"},
			{SoundEffect.DeskShow,"DeskShow"},
			{SoundEffect.Grip_Down,"Grip_Down"},
			{SoundEffect.Grip_Up,"Grip_Up"},

			{SoundEffect.Neuron_Fire1, "snap1"},
			{SoundEffect.Neuron_Fire2, "snap2"},
			{SoundEffect.Neuron_Fire3, "snap3"},
			{SoundEffect.Neuron_Fire4, "snap4"},
			{SoundEffect.Neuron_Fire5, "snap5"},
			{SoundEffect.Neuron_Fire6, "snap6"},
			{SoundEffect.Neuron_Fire7, "snap7"},
			{SoundEffect.Neuron_Fire8, "snap8"},
			{SoundEffect.Neuron_Fire9, "snap9"},
			{SoundEffect.Neuron_Fire10, "snap10"},
			{SoundEffect.Neuron_Fire11, "snap11"},
			{SoundEffect.Neuron_Fire12, "snap12"},
			{SoundEffect.Neuron_Fire13, "snap13"},


			{SoundEffect.Neuron_Click, "trigger"},

		};

		protected Dictionary<SoundEffect,AudioClip> soundEffectToAudioClip = new  Dictionary<SoundEffect, AudioClip>();
		//readonly

		//Serialized
		
		/////Protected/////
		//References
		protected AudioSource audioSource;
		protected AudioMixer audioMixer;
		//Primitives
//		protected Application.Language currentLanguage;
		protected static float musicVolume = 1f;
		protected static float narrationVolume = 1f;
		

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected virtual void Awake()
		{
			base.SaveInstance(this);

			this.audioSource = GetComponent<AudioSource>();
			if(this.audioSource==null)
				this.audioSource = this.gameObject.AddComponent<AudioSource>();

//			this.audioMixer = Application.Application.Instance.audioMixer;
		}
		
		protected virtual void Start ()
		{
			
		}
		
		protected virtual void Update ()
		{
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Audio Functions
		//

		public void PlaySoundEffect(SoundEffect soundEffect, float volume = 1f, Vector3 position = default(Vector3)) 
		{
			if(this.soundEffectToAudioClip.ContainsKey(soundEffect))
			{
				PlayClip(this.soundEffectToAudioClip[soundEffect],volume,position);
			}
			else
			{
				string soundEffectResourcePath = this.soundEffectToResourcePath[soundEffect];
//				if(soundEffect.ToString().StartsWith("Voiceover")&& currentLanguage!=ITHB.Application.Language.English){
//					soundEffectResourcePath += "-" + currentLanguage.ToString();
//				}
				AudioClip audioClip = Resources.Load (soundEffectResourcePath) as AudioClip;
				if(!soundEffect.ToString().StartsWith("Voiceover"))
					this.soundEffectToAudioClip[soundEffect] = audioClip;
				PlayClip(audioClip,volume,position);
			}
		}

		protected void PlayClip(AudioClip audioClip, float volume = 1f,  Vector3 position = default(Vector3))
		{
			if(audioClip.name.StartsWith("VO"))
			{
//				Voiceover.Instance.PlayClip(audioClip);
				return;
			}

			if(this.audioSource.isPlaying)
			{
				AudioSource.PlayClipAtPoint(audioClip,position,volume);
			}
			else
			{
				if(position==Vector3.zero)
					this.audioSource.spatialBlend = 0;
				else
					this.audioSource.spatialBlend = 1f;
				this.transform.position = position;
				this.audioSource.volume = volume;
				this.audioSource.clip = audioClip;
				this.audioSource.Play();
			}
		}

//		public void SetLanguage(Application.Language language) {

//			this.currentLanguage = language;
//		}

		////////////////////////////////////////
		//
		// Volume Functions

		public void SetNarrationVolume(float volume) {

			narrationVolume = volume;
			audioMixer.SetFloat("NarrationVolume",PercentToDecibel(volume));
		}

		public void SetMusicVolume(float volume) {

			musicVolume = volume;
			audioMixer.SetFloat("AmbienceVolume",PercentToDecibel(volume));
		}

		public float GetNarrationVolume() {
			
			return narrationVolume;
		}

		public float GetMusicVolume() {

			return musicVolume;
		}

		protected float PercentToDecibel(float percent) {

			return 80f*((1f-Mathf.Pow(1f-percent,2f))-1f);
		}
	}
}