using System.Collections.Generic;

namespace Other
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class Helpers {
        public static Guid CreateGuidFromString(string input) {
            return new Guid(MD5.Create().ComputeHash(Encoding.Default.GetBytes(input)));
        }
    
        public static Vector2 ClampToScreen(VisualElement element, Vector2 targetPosition) {
            float x = Mathf.Clamp(targetPosition.x, 0, Screen.width - element.layout.width);
            float y = Mathf.Clamp(targetPosition.y, 0, Screen.height - element.layout.height);

            return new Vector2(x, y);
        }
        
        public static SerializableGuid ToSerializableGuid(this Guid systemGuid) {
            byte[] bytes = systemGuid.ToByteArray();
            return new SerializableGuid(
                BitConverter.ToUInt32(bytes, 0),
                BitConverter.ToUInt32(bytes, 4),
                BitConverter.ToUInt32(bytes, 8),
                BitConverter.ToUInt32(bytes, 12)
            );
        }

        public static Guid ToSystemGuid(this SerializableGuid serializableGuid) {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part1), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part2), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part3), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.Part4), 0, bytes, 12, 4);
            return new Guid(bytes);
        }
        
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator) {
            while ( enumerator.MoveNext() ) {
                yield return enumerator.Current;
            }
        }
    }
}