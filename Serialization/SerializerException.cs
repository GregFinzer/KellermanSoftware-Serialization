using System;

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// Thrown when an object cannot be serialized or deserialized
    /// </summary>
    public class SerializerException : Exception
    {
        /// <summary>
        /// Serializer Exception With A Message
        /// </summary>
        /// <param name="message"></param>
        public SerializerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Serializer Exception with an Inner Exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SerializerException(string message, Exception innerException) : base(message,innerException)
        {            
        }
    }
}