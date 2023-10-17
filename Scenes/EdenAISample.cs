using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EdenAI;
using TMPro;

namespace EdenAISample
{
    public class EdenAISample : MonoBehaviour
    {
        [SerializeField] private RectTransform _sent_message;
        [SerializeField] private RectTransform _receive_message;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _send_button;

        private string chatBotGlobalAction =
            "Act as an assistant in a video game. When asked who you are, say an assistant.";

        private List<ChatMessage> _messages = new List<ChatMessage>();
        private EdenAIApi _edenAIApi = new EdenAIApi();

        private float height;

        // Start is called before the first frame update
        void Start()
        {
            _send_button.onClick.AddListener(MessageResponse);
        }

        private void addChatMessage(ChatMessage message)
        {
            _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            if (message.Role == "assistant")
            {
                RectTransform receiveMessageRect = Instantiate(_receive_message, _scrollRect.content);
                receiveMessageRect.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = message.Message;
                receiveMessageRect.anchoredPosition = new Vector2(0, -height);
                LayoutRebuilder.ForceRebuildLayoutImmediate(receiveMessageRect);
                height += receiveMessageRect.sizeDelta.y;
                _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                _scrollRect.verticalNormalizedPosition = 0;
            }
            else
            {
                RectTransform sendMessageRect = Instantiate(_sent_message, _scrollRect.content);
                sendMessageRect.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = message.Message;
                sendMessageRect.anchoredPosition = new Vector2(0, -height);
                LayoutRebuilder.ForceRebuildLayoutImmediate(sendMessageRect);
                height += sendMessageRect.sizeDelta.y;
                _scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                _scrollRect.verticalNormalizedPosition = 0;
            }
        }

        private async void MessageResponse()
        {
            ChatMessage newChatMessage = new ChatMessage()
            {
                Role = "user",
                Message = _inputField.text
            };
            _send_button.enabled = false;

            _inputField.enabled = false;
            addChatMessage(newChatMessage);
            ChatResponse chat =
                await _edenAIApi.SendChatRequest("openai", _inputField.text, chatBotGlobalAction, _messages);
            _inputField.text = " ";
            _messages.AddRange(chat.message);
            ChatMessage newAssistantChatMessage = new ChatMessage()
            {
                Role = "assistant",
                Message = chat.generated_text
            };
            addChatMessage(newAssistantChatMessage);
            TextToSpeechResponse audio = await _edenAIApi.SendTextToSpeechRequest("amazon", chat.generated_text, "mp3",
                TextToSpeechOption.MALE, "en", voiceModel: "en-US_Justin_Standard");
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audio.audio;
            audioSource.Play(0);

            _send_button.enabled = true;
            _inputField.enabled = true;
        }
    }
}