using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// Interface for plugging in custom encryption
    /// </summary>
    public interface IEncryption
    {
        /// <summary>
        /// The block size for the encryption algorithm
        /// </summary>
        int BlockSize { get; set; }

        /// <summary>
        /// The key size for the encryption algorithm
        /// </summary>
        int KeySize { get; set; }

        /// <summary>
        /// The salt for the encryption algorithm
        /// </summary>
        string Salt { get; set;}

        /// <summary>
        /// Encrypt a series of bytes using the supplied password
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        byte[] EncryptBytes(byte[] inputBytes, string password);

        /// <summary>
        /// Decrypt a series of bytes using the supplied password
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        byte[] DecryptBytes(byte[] inputBytes, string password);

    }
}
