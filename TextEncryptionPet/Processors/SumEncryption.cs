using TextEncryptionPet.Assemblies;
using System;

namespace TextEncryptionPet.Processors
{
    class SumEncryption : BaseProcessor
    {
        private string _key;
        public int RandomMin = 33;
        public int RandomMax = 126;
        public override string ProcessorName { get; }

        public SumEncryption(string text)
        {
            ProcessorName = $"{GetType().Name} {text}";
        }

        public override string EncryptText(string text)
        {
            Random vector = new Random(_key[0]);
            char[] textChars = text.ToCharArray();
            string outText = "";
            foreach (char c in textChars)
            {
                if (c == 32)
                {
                    outText += c;
                    continue;
                }
                outText += (char)(new Random(vector.Next(_key.Length - 1)).Next(RandomMin, RandomMax) + c);
            }
            return outText;
        }

        public override string DecryptText(string text)
        {
            Random vector = new Random(_key[0]);
            char[] textChars = text.ToCharArray();
            string outText = "";
            foreach (char c in textChars)
            {
                if (c == 32)
                {
                    outText += c;
                    continue;
                }
                outText += (char)(c - new Random(vector.Next(_key.Length - 1)).Next(RandomMin, RandomMax));
            }
            return outText;
        }

        public override void SetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _key = Encryption.GenerateKey();
            }
            else
            {
                _key = Encryption.GenerateKey(key);
            }
        }
    }
}
