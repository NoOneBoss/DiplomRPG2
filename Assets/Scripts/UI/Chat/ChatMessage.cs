using System;
using Unity.Collections;
using Unity.Netcode;

namespace UI.Chat
{
    public struct  ChatMessage : INetworkSerializable, IEquatable<ChatMessage>
    {
        public ulong senderId;
        public FixedString64Bytes playerName;
        public FixedString64Bytes userUUID;
        public FixedString32Bytes timestamp;
        public FixedString128Bytes message;
        public FixedString32Bytes chatType;

        public ChatMessage(ulong senderId, string playerName, string userUuid, string timestamp, string message, string chatType)
        {
            this.senderId = senderId;
            this.playerName = playerName;
            userUUID = userUuid;
            this.timestamp = timestamp;
            this.message = message;
            this.chatType = chatType;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out senderId);
                reader.ReadValueSafe(out playerName);
                reader.ReadValueSafe(out userUUID);
                reader.ReadValueSafe(out timestamp);
                reader.ReadValueSafe(out message);
                reader.ReadValueSafe(out chatType);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(senderId);
                writer.WriteValueSafe(playerName);
                writer.WriteValueSafe(userUUID);
                writer.WriteValueSafe(timestamp);
                writer.WriteValueSafe(message);
                writer.WriteValueSafe(chatType);
            }
        }

        public bool Equals(ChatMessage other)
        {
            return userUUID.Equals(other.userUUID) && timestamp.Equals(other.timestamp) && message.Equals(other.message) && chatType.Equals(other.chatType);
        }

        public override bool Equals(object obj)
        {
            return obj is ChatMessage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(userUUID, timestamp, message, chatType);
        }
    }
}