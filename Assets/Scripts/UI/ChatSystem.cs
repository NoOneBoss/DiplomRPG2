using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Configs;
using Netcode.LogModule;
using Other;
using Player;
using ScriptableObjects;
using UI.Chat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;


public class ChatSystem : NetworkBehaviour
{
    public ServerConfig serverConfig;
    public NetworkList<ChatMessage> chat;
    public UIDocument document;

    private ListView globalChatView;
    private ListView tradeChatView;

    private TextField globalChatInput;
    private TextField tradeChatInput;
    
    public void Awake()
    {
        chat = new NetworkList<ChatMessage>();
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Chat system initialized!");
            chat.OnListChanged += @event =>
            {
                Debug.Log($"({@event.Value.chatType}) {@event.Value.playerName}: {@event.Value.message}");
            };
        }
        else
        {
            globalChatView = document.rootVisualElement.Q<ListView>("globalChat");
            tradeChatView = document.rootVisualElement.Q<ListView>("tradeChat");
            globalChatInput = document.rootVisualElement.Q<TextField>("globalChatInput");
            tradeChatInput = document.rootVisualElement.Q<TextField>("tradeChatInput");
            
            globalChatInput.RegisterCallback<KeyDownEvent>(SendChatEvent);
            tradeChatInput.RegisterCallback<KeyDownEvent>(SendChatEvent);
            
            UpdateGlobalChatView();
            UpdateTradeChatView();

            chat.OnListChanged += @event =>
            {
                Debug.Log($"({@event.Value.chatType}) {@event.Value.playerName}: {@event.Value.message}");
                UpdateGlobalChatView();
                UpdateTradeChatView();
            };
        }
    }

    private void UpdateGlobalChatView()
    {
        var globalChatMessages = chat.GetEnumerator().ToIEnumerable().Where(m => m.chatType.Value == "global").ToList();
        globalChatView.itemsSource = globalChatMessages;
        globalChatView.makeItem = () => 
        {
            var label = new Label();
            label.AddToClassList("label-wrap");
            return label;
        };
        globalChatView.bindItem = (element, index) =>
        {
            if (index >= 0 && index < globalChatMessages.Count)
            {
                ((Label)element).text = $"{globalChatMessages[index].playerName.Value}: {globalChatMessages[index].message.Value}";
            }
        };
        globalChatView.ScrollToItem(-1);
    }

    private void UpdateTradeChatView()
    {
        var tradeChatMessages = chat.GetEnumerator().ToIEnumerable().Where(m => m.chatType.Value == "trade").ToList();
        tradeChatView.itemsSource = tradeChatMessages;
        tradeChatView.makeItem = () => 
        {
            var label = new Label();
            label.AddToClassList("label-wrap");
            return label;
        };
        tradeChatView.bindItem = (element, index) =>
        {
            if (index >= 0 && index < tradeChatMessages.Count)
            {
                ((Label)element).text = $"{tradeChatMessages[index].playerName.Value}: {tradeChatMessages[index].message.Value}";
            }
        };
        tradeChatView.ScrollToItem(-1);
    }

    private void SendChatEvent(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Return) return;
        evt.StopPropagation();

        var chatType = globalChatInput.focusController.focusedElement == globalChatInput ? "global" : "trade";
        var message = chatType == "global" ? globalChatInput.value : tradeChatInput.value;

        if (string.IsNullOrEmpty(message)) return;

        PlayerData _playerData = PlayerManager.Singleton.getLocalPlayerData();
        ChatMessage chatMessage = new ChatMessage(
            NetworkManager.LocalClientId,
            _playerData.PlayerName,
            _playerData.uuid,
            DateTime.Now.ToString(CultureInfo.CurrentCulture), message, chatType
        );
        SendChatRpc(chatMessage);
        StartCoroutine(postMessage(chatMessage));
        LogService.Singlenton.AddLog(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), _playerData.uuid, "send_message", $"{chatType}");

        if (chatType == "global") globalChatInput.value = "";
        else if (chatType == "trade") tradeChatInput.value = "";
    }

    [Rpc(SendTo.Server)]
    private void SendChatRpc(ChatMessage chatMessage)
    {
        chat.Add(chatMessage);
    }
    
    [Rpc(SendTo.Server)]
    private void UpdateChatRpc()
    {
        var chatMessage = new ChatMessage();
        chat.Add(chatMessage);
        chat.Remove(chatMessage);
    }

    IEnumerator postMessage(ChatMessage message) {
        WWWForm form = new WWWForm();
        form.AddField("user_uuid", message.userUUID.Value);
        form.AddField("timestamp", message.timestamp.Value);
        form.AddField("message", message.message.Value);
        form.AddField("chat_type", message.chatType.Value);
     
        UnityWebRequest www = UnityWebRequest.Post($"http://{serverConfig.address}:{serverConfig.port}/chat", form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerManager.Singleton.getLocalPlayerData().authToken);
        yield return www.SendWebRequest();
    }
}
