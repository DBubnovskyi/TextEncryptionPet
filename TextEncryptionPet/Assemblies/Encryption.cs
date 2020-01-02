using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace TextEncryptionPet.Assemblies
{
    /// <summary>Provides methods to encrypt data</summary>
    public static class Encryption
    {
        /// <summary>
        /// Constant key that changing with new encryption version, 
        /// string length should be 16 characters.
        /// </summary>
        private const string LIBRARY_KEY = "ErT87%RtQ6SE$PO=";
        /// <summary>
        /// Define length of the key for <see cref="SymmetricAlgorithm.CreateDecryptor"/>
        /// This number should not be changed for correct work of the algorithm
        /// </summary>
        private const int KEY_LENGTH = 16;

        /// <summary>Used to select the encryption method.</summary>
        public enum EncryptionType
        {
            /// <summary>
            /// Encryption always works on the same way, 
            /// if input text and <see cref="EncryptionKey"/> the same, 
            /// encrypted output data has always the same value.
            /// </summary>
            Static,
            /// <summary>Encryption works with random parrameters and output data always different.</summary>
            Random
        }

        /// <summary>Variable that stores encryption key</summary>
        private static string _initVector = null;
        /// <summary>
        /// Key that used for creating <see cref="SymmetricAlgorithm.CreateEncryptor"/>,
        /// this value should be set before start <see cref="Encrypt"/> or <see cref="Decrypt"/> methods,
        /// value should contain more than 16 characters.
        /// </summary>
        public static string EncryptionKey
        {
            get
            {
                if (_initVector == null)
                {
                    throw new Exception("EncryptionKey not set, set the encryption key before run Encrypt or Decrypt methods");
                }
                return _initVector;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("EncryptionKey can not be null or empty");
                }
                else if (value.Length < KEY_LENGTH)
                {
                    throw new Exception("EncryptionKey should be equal or longer than 16 characters");
                }
                _initVector = value;
            }
        }

        /// <summary>
        /// Value used for creation random params for <see cref="EncryptionType.Random"/> algorithm of encryption.
        /// This value shows number of digits that would be used for random key generarion <see cref="GetRandomizerCode"/>.
        /// </summary>
        private static int _numCount = 4;
        /// <summary>
        /// Set number of digits for generation random key in method <see cref="GetRandomizerCode"/> 
        /// from <see cref="EncryptionKey"/>. Value should be in the range of 1 to 9 because of restriction 
        /// of the int value type: <see cref="Int32.MaxValue"/>
        /// </summary>
        public static int RandNumCount
        {
            get => _numCount;
            set
            {
                int min = 1;
                int max = 9;
                if (value < min)
                {
                    _numCount = min;
                }
                else if (value > max)
                {
                    _numCount = max;
                }
                else
                {
                    _numCount = value;
                }
            }
        }

        /// <summary>Serialize object in JSON format and encrypt it</summary>
        /// <typeparam name="T">Represents object with public constructor</typeparam>
        /// <param name="plainObject">Input object for the encription</param>
        /// <param name="encryptionType">Type of encription</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt<T>(T plainObject, EncryptionType encryptionType = EncryptionType.Static) where T : new()
        {
            string json = JsonConvert.SerializeObject(plainObject);
            return Encrypt(json, encryptionType);
        }
        /// <summary>Encrypt input string</summary>
        /// <param name="inputText">Input string for the encription</param>
        /// <param name="encryptionType">Type of encription</param>
        /// <returns>Encrypted string</returns>
        public static string Encrypt(string inputText, EncryptionType encryptionType = EncryptionType.Static)
        {

            byte[] initVectorbytes;
            if (encryptionType == EncryptionType.Random)
            {
                int randomizerCode = GetRandomizerCode(RandNumCount);
                string encryptionKey = GenerateKey(EncryptionKey, randomizerCode);
                initVectorbytes = Encoding.UTF8.GetBytes(encryptionKey);
                byte[] encryptedArrey = Encrypt(initVectorbytes, inputText);
                return $"{RandNumCount}{randomizerCode}{Convert.ToBase64String(encryptedArrey)}";
            }
            else
            {
                initVectorbytes = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, KEY_LENGTH));
                byte[] encryptedArrey = Encrypt(initVectorbytes, inputText);
                return Convert.ToBase64String(encryptedArrey);
            }
        }
        /// <summary>Encrypt input string</summary>
        /// <param name="initVectorbytes">Array with key bytes</param>
        /// <param name="plainText">Input string for the encription</param>
        /// <returns>Encrypted bytes</returns>
        private static byte[] Encrypt(byte[] initVectorbytes, string plainText)
        {
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(LIBRARY_KEY);
                aes.IV = initVectorbytes;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return array;
        }

        /// <summary>Decrypt object to JSON format and deserialize object</summary>
        /// <typeparam name="T">Represents object with public constructor</typeparam>
        /// <param name="encryptText">Input encrypted object in JSON format</param>
        /// <param name="encryptionType">Type of encription that was used</param>
        /// <returns>New instance of decrypted object</returns>
        public static T Decrypt<T>(string encryptText, EncryptionType encryptionType = EncryptionType.Static) where T : new()
        {
            string json = Decrypt(encryptText, encryptionType);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>Decrypt input string</summary>
        /// <param name="inputText">Input string for the decryption</param>
        /// <param name="encryptionType">Type of encription that was used</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string encryptedText, EncryptionType encryptionType = EncryptionType.Static)
        {
            byte[] initVectorbytes;
            if (encryptionType == EncryptionType.Random)
            {
                int rendomazerLength;
                if (int.TryParse(encryptedText.Substring(0, 1), out rendomazerLength))
                {
                    int randomizerCode;
                    if (int.TryParse(encryptedText.Substring(1, rendomazerLength), out randomizerCode))
                    {
                        string encryptionKey = GenerateKey(EncryptionKey, randomizerCode);
                        initVectorbytes = Encoding.UTF8.GetBytes(encryptionKey);
                        return Decrypt(initVectorbytes, encryptedText.Substring(rendomazerLength + 1));
                    }
                    else
                    {
                        throw new Exception("Can't parse 'randomizerCode' parameter for EncryptionType.Random");
                    }
                }
                else
                {
                    throw new Exception("Can't parse 'rendomazerLength' parameter for EncryptionType.Random");
                }
            }
            else
            {
                initVectorbytes = Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, KEY_LENGTH));
                return Decrypt(initVectorbytes, encryptedText);
            }


        }
        /// <summary>Encrypt input string</summary>
        /// <param name="initVectorbytes">Array with key bytes</param>
        /// <param name="cipherText">Input string for the dencription</param>
        /// <returns>Decrypted bytes</returns>
        private static string Decrypt(byte[] initVectorbytes, string cipherText)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(LIBRARY_KEY);
                aes.IV = initVectorbytes;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates random key in runtime with set parameters.
        /// Can be used for generating key for <see cref="EncryptionKey"/> 
        /// in runtime using seed property of <see cref="Random"/>.
        /// </summary>
        /// <param name="randomizerParam">Seed property of <see cref="Random"/></param>
        /// <param name="length">Set length of generated string</param>
        /// <returns>String of random characters</returns>
        public static string GenerateKey(int randomizerParam = 0, int length = 256)
        {
            string generatedKey = "";
            var rand = new Random(randomizerParam);
            for (var i = 0; i < length; i++)
            {
                generatedKey += (char)rand.Next(33, 126);
            }
            return generatedKey;
        }

        /// <summary>Generates random key from input parameters</summary>
        /// <param name="encryptionKey">Represent string from which method randomly pics chars for generation</param>
        /// <param name="randomizerParam">Seed parameter for random generation of string indexes</param>
        /// <returns>Generated key with length of <see cref="KEY_LENGTH"/></returns>
        public static string GenerateKey(string encryptionKey, int randomizerParam = 0)
        {
            string generatedCode = "";
            var rand = new Random(randomizerParam);
            for (var i = 0; i < KEY_LENGTH; i++)
            {
                generatedCode += encryptionKey[rand.Next(encryptionKey.Length - 1)];
            }
            return generatedCode;
        }

        /// <summary>Generate int nuber with set count of digits</summary>
        /// <param name="length">Set number of digits</param>
        private static int GetRandomizerCode(int length)
        {
            int min = 1, max = 9;
            for (int i = 0; i < length - 1; i++)
            {
                min = min * 10;
                max = (max * 10) + 9;
            }
            return new Random().Next(min, max);
        }
    }
}
