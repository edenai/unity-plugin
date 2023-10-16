using UnityEngine;
using System;
using System.Collections.Generic;


namespace EdenAI
{
	[Serializable]
    public class TextToSpeechResponseJson
    {
        public string status { get; set; }
        public string provider { get; set; }
        public double cost { get; set; }
        public string audio { get; set; }
        public int voice_type { get; set; }
        public string audio_resource_url { get; set; }
    }

    public class TextToSpeechResponse
    {
        public string status { get; set; }
        public string provider { get; set; }
        public double cost { get; set; }
        public AudioClip audio { get; set; }
        public int voice_type { get; set; }
		public string audio_base64 { get; set; }
    }

    public enum TextToSpeechOption
	{
		FEMALE,
		MALE,
	}

    public class TextToSpeechRequest
    {
        public string providers { get; set; }
        public bool response_as_dict = false;
        public bool show_original_response = false;
        public string text { get; set; }
		public string audio_format {get; set;}
		public int? rate {get; set;}
		public int? pitch {get; set;}
		public int? volume {get; set;}
		public string language {get; set;}
		public string option {get; set;}
		public Dictionary<string, string> settings { get; set; }

        public TextToSpeechRequest(string provider, string text, string audioFormat, TextToSpeechOption option,
	        string language, int? rate = null, int? pitch = null, int? volume = null, Dictionary<string, string> settings = null)
        {
            this.providers = provider;
            this.text = text;
			this.audio_format = audioFormat;
			this.rate = rate;
			this.pitch = pitch;
			this.volume = volume;
			this.language = language;
			this.option = option.ToString();
			this.settings = settings;
        }
    }
}