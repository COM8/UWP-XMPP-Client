﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using libsignal;
using libsignal.ecc;
using libsignal.state;
using libsignal.util;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using XMPP_API.Classes.Network;

namespace XMPP_API.Classes.Crypto
{
    public static class CryptoUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Reads the raw key from the given ECPublicKey.
        /// Based on: https://github.com/langboost/libsignal-protocol-pcl/blob/ef9dece1283948f89c7cc4c5825f944f77ed2c3e/signal-protocol-pcl/ecc/Curve.cs#L37
        /// </summary>
        /// <param name="key">The public key you want to retrieve the raw key from.</param>
        /// <returns>Returns the raw key from the given ECPublicKey.</returns>
        public static byte[] getRawFromECPublicKey(ECPublicKey key)
        {
            byte[] keySerialized = key.serialize();
            byte[] keyRaw = new byte[32];
            System.Buffer.BlockCopy(keySerialized, keySerialized.Length - keyRaw.Length, keyRaw, 0, keyRaw.Length);

            return keyRaw;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// RFC 2898 with Sha1
        /// </summary>
        public static byte[] Pbkdf2Sha1(string normalizedPassword, byte[] salt, int iterations)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(normalizedPassword, salt)
            {
                IterationCount = iterations
            };

            return deriveBytes.GetBytes(20);
        }

        public static byte[] HmacSha1(byte[] data, string key)
        {
            return HmacSha1(data, Encoding.UTF8.GetBytes(key));
        }

        public static byte[] HmacSha1(string data, string key)
        {
            return HmacSha1(Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(key));
        }

        public static byte[] HmacSha1(string data, byte[] key)
        {
            return HmacSha1(Encoding.UTF8.GetBytes(data), key);
        }

        public static byte[] HmacSha1(byte[] data, byte[] key)
        {
            HMACSHA1 hmac = new HMACSHA1(key);
            hmac.Initialize();
            return hmac.ComputeHash(data);
        }

        public static byte[] SHA_1(byte[] data)
        {
            return hash(data, HashAlgorithmNames.Sha1);
        }

        public static byte[] xor(byte[] a, byte[] b)
        {
            byte[] output = new byte[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                output[i] = (byte)(b[i] ^ a[i % a.Length]);
            }

            return output;
        }

        public static SignedPreKeyRecord generateOmemoSignedPreKey(IdentityKeyPair identityKeyPair)
        {
            return KeyHelper.generateSignedPreKey(identityKeyPair, 5);
        }

        public static IdentityKeyPair generateOmemoIdentityKeyPair()
        {
            return KeyHelper.generateIdentityKeyPair();
        }

        public static IList<PreKeyRecord> generateOmemoPreKeys()
        {
            return KeyHelper.generatePreKeys(0, 100);
        }

        public static uint generateOmemoDeviceId()
        {
            return KeyHelper.generateRegistrationId(false);
        }

        public static uint generateOmemoDeviceIds(IList<uint> usedDeviceIds)
        {
            // Try 10 times to get a random, unique device id:
            uint id;
            for (int i = 0; i < 10; i++)
            {
                id = generateOmemoDeviceId();
                if (!usedDeviceIds.Contains(id))
                {
                    return id;
                }
            }
            throw new InvalidOperationException("Failed to generate unique device id! " + nameof(usedDeviceIds) + ". Count = " + usedDeviceIds.Count);
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string byteArrayToHexString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Generates the display fingerprint for the given OMEMO identity public key.
        /// </summary>
        /// <param name="keyRaw">The OMEMO identity public key as a byte[].</param>
        /// <returns>A hex string representing the given OMEMO identity public key. Split up in blocks of 8 characters separated by a whitespace.</returns>
        public static string generateOmemoFingerprint(byte[] keyRaw)
        {
            string fingerprint = byteArrayToHexString(keyRaw);
            return Regex.Replace(fingerprint, ".{8}", "$0 ").TrimEnd();
        }

        /// <summary>
        /// Generates the OMEMO fingerprint for displaying as a QR code.
        /// </summary>
        /// <param name="key">The identity public key.</param>
        /// <param name="account">The XMPP account the fingerprint belongs to.</param>
        /// <returns>A string representation of the fingerprint concatenated with JID and device id.</returns>
        public static string generateOmemoQrCodeFingerprint(IdentityKey key, XMPPAccount account)
        {
            return generateOmemoQrCodeFingerprint(key, account.getBareJid(), account.omemoDeviceId);
        }

        /// <summary>
        /// Generates the OMEMO fingerprint for displaying as a QR code.
        /// </summary>
        /// <param name="key">The identity public key.</param>
        /// <param name="bareJid">The bare JID the fingerprint belongs to.</param>
        /// <param name="deviceId">The OMEMO device id the fingerprint belongs to.</param>
        /// <returns>A string representation of the fingerprint concatenated with JID and device id.</returns>
        public static string generateOmemoQrCodeFingerprint(IdentityKey key, string bareJid, uint deviceId)
        {
            StringBuilder sb = new StringBuilder("xmpp:");
            sb.Append(bareJid);
            sb.Append("?omemo-sid-");
            sb.Append(deviceId);
            sb.Append('=');
            sb.Append(byteArrayToHexString(key.serialize()));
            return sb.ToString();
        }

        public static void nextBytesSecureRandom(out byte[] b, in uint length)
        {
            IBuffer buf = CryptographicBuffer.GenerateRandom(length);
            CryptographicBuffer.CopyToByteArray(buf, out b);
        }

        /// <summary>
        /// Loads a new <see cref="IdentityKeyPair"/> instance from the given key and returns it.
        /// </summary>
        /// <param name="key">The key to load the <see cref="IdentityKeyPair"/> from.</param>
        public static IdentityKeyPair loadIdentityKeyPair(byte[] key)
        {
            return new IdentityKeyPair(key);
        }

        /// <summary>
        /// Loads a new <see cref="SignedPreKeyRecord"/> instance from the given key and returns it.
        /// </summary>
        /// <param name="key">The key to load the <see cref="SignedPreKeyRecord"/> from.</param>
        public static SignedPreKeyRecord loadSignedPreKeyRecord(byte[] key)
        {
            return new SignedPreKeyRecord(key);
        }

        /// <summary>
        /// Loads a new <see cref="PreKeyRecord"/> instance from the given key and returns it.
        /// </summary>
        /// <param name="key">The key to load the <see cref="PreKeyRecord"/> from.</param>
        public static PreKeyRecord loadPreKeyRecord(byte[] key)
        {
            return new PreKeyRecord(key);
        }

        #endregion

        #region --Misc Methods (Private)--
        // Source: https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.hashalgorithmprovider
        private static byte[] hash(byte[] data, string algName)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.CreateFromByteArray(data);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algName);

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new InvalidOperationException("There was an error creating the hash");
            }

            // Return the encoded string
            return buffHash.ToArray();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
