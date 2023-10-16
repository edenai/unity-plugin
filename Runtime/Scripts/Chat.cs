using System.Collections;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdenAI
{
    [Serializable]
    public class ChatResponse
    {
        public string status { get; set; }
        public string provider { get; set; }
        public string generated_text { get; set; }
        public List<ChatMessage> message { get; set; }
        public double cost { get; set; }
    }

    [Serializable]
    public class ChatMessage
    {
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    [Serializable]
    public class ChatRequest
    {
        public string providers { get; set; }
        public bool response_as_dict = false;
        public bool show_original_response = false;
        public string text { get; set; }
        public List<ChatMessage> previous_history { get; set; }
        public string ChatBotGlobalAction { get; set; }
        public Dictionary<string, string> settings { get; set; }

        public ChatRequest(string provider, string text, string chatBotGlobalAction = null,
            List<ChatMessage> previousHistory = null, Dictionary<string,string> settings = null)
        {
            this.providers = provider;
            this.text = text;
            this.previous_history = previousHistory;
            this.ChatBotGlobalAction = chatBotGlobalAction;
            this.settings = settings;
        }
    }
}