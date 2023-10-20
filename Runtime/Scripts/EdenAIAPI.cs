using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.IO;

namespace EdenAI
{
    [Serializable]
	public class Key
	{
		public string api_key {get; set;}
	}

    public class EdenAIApi
    {
	    private string _apiKey;
		
		public EdenAIApi(string apiKey = default)
		{
			if (!string.IsNullOrEmpty(apiKey))
			{
				this._apiKey = apiKey;
			}
			else
			{
				var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var authPath = $"{userPath}/.edenai/auth.json";
				if (File.Exists(authPath))
                {
                    var json = File.ReadAllText(authPath);
                    Key auth = JsonConvert.DeserializeObject<Key>(json);
					this._apiKey = auth.api_key;
                }
                else
                {
                    throw new Exception("API Key is null and auth.json does not exist.");
                }
			}
		}
		private static async Task<AudioClip> AudioToAudioClip(string audioFormat, TextToSpeechResponseJson response)
		{
			audioFormat = audioFormat.ToLower();
			AudioType audioType;
			switch (audioFormat)
			{
				case "mp3":
					audioType = AudioType.MPEG;
					break;
				case "wav":
					audioType = AudioType.WAV;
					break;
				default:
					audioType = AudioType.UNKNOWN;
					break;
			}
			using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(response.audio_resource_url, audioType);
			var asyncOp = request.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Yield();
			}
			if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				throw new Exception(request.error);
			}
			AudioClip audio = DownloadHandlerAudioClip.GetContent(request);
			if (audio)
				return audio;
			throw new Exception("Unable to convert audio to audioClip");
		}

        public async Task<TextToSpeechResponse> SendTextToSpeechRequest(string provider, string text, string audioFormat, 
	        TextToSpeechOption option, string language, int? rate = null, int? pitch = null, int? volume = null, string voiceModel = null
	        )
        {
            string url = "https://api.edenai.run/v2/audio/text_to_speech";
            Dictionary<string, string> settings = voiceModel != null ? new Dictionary<string, string> { { provider, voiceModel } } : null;
			TextToSpeechRequest payload = new TextToSpeechRequest(provider, text, audioFormat, option, language, rate, pitch,
				volume,settings);
			using UnityWebRequest request = new UnityWebRequest(url, "POST");
			try
			{
				AddHeaders(request);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
			byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			}));
			request.uploadHandler = new UploadHandlerRaw(postData);
			request.downloadHandler = new DownloadHandlerBuffer();
			var asyncOp = request.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Yield();
			}
			if (request.result == UnityWebRequest.Result.Success)
			{
				string responseText = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
				TextToSpeechResponseJson[] response = JsonConvert.DeserializeObject<TextToSpeechResponseJson[]>(responseText);
				TextToSpeechResponse result = new TextToSpeechResponse
				{
					status = response[0].status,
					provider = response[0].provider,
					cost = response[0].cost,
					voice_type = response[0].voice_type,
					audio = await AudioToAudioClip(audioFormat, response[0]),
					audio_base64 = response[0].audio
				};
				return result;
			}
			Debug.LogError(request.downloadHandler.text);
			throw new Exception(request.error);
        }

		public async Task<ChatResponse> SendChatRequest(string provider, string text, string chatBotGlobalAction = null,
			List<ChatMessage> previousHistory = null, string model = null)
        {
            string url = "https://api.edenai.run/v2/text/chat";
            Dictionary<string, string> settings = model != null ? new Dictionary<string, string> { { provider, model } } : null;
			ChatRequest payload = new ChatRequest(provider, text, chatBotGlobalAction, previousHistory, settings);
			using UnityWebRequest request = new UnityWebRequest(url, "POST");
			try
			{
				AddHeaders(request);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
			byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			}));
			request.uploadHandler = new UploadHandlerRaw(postData);
			request.downloadHandler = new DownloadHandlerBuffer();
			var asyncOp = request.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Yield();
			}
			if (request.result == UnityWebRequest.Result.Success)
			{
				string responseText = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
				ChatResponse[] obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
				return obj[0];
			}
			//Debug.LogError(request.error);
			Debug.LogError(request.downloadHandler.text);
			throw new Exception(request.downloadHandler.text);
        }
		public async Task<YodaResponse> SendYodaRequest(string projectID, string query, List<Dictionary<string,string>> history = null,
			int? k = null, string llmModel = null, string llmProvider = null)
		{
			string url = "https://api.edenai.run/v2/aiproducts/askyoda/" + projectID + "/ask_llm";
			YodaRequest payload = new YodaRequest(query, history, k, llmModel, llmProvider);
			using UnityWebRequest request = new UnityWebRequest(url, "POST");
			try
			{
				AddHeaders(request);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				throw new Exception(ex.Message);
			}
			byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			}));
			request.uploadHandler = new UploadHandlerRaw(postData);
			request.downloadHandler = new DownloadHandlerBuffer();
			var asyncOp = request.SendWebRequest();
			while (!asyncOp.isDone)
			{
				await Task.Yield();
			}
			if (request.result == UnityWebRequest.Result.Success)
			{
				string responseText = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
				YodaResponse obj = JsonConvert.DeserializeObject<YodaResponse>(responseText);
				return obj;
			}
			Debug.LogError(request.error);
			Debug.LogError(request.downloadHandler.text);
			throw new Exception(request.error);
		}

        private void AddHeaders(UnityWebRequest request)
        {
 			if (string.IsNullOrEmpty(this._apiKey))
			{
				throw new Exception("Missing Api Key");
			}
            request.SetRequestHeader("Authorization", "Bearer " + this._apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
        }
    }
}