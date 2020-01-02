using TextEncryptionPet.Assemblies;

namespace TextEncryptionPet.Processors
{
    class RandomEncryption : BaseProcessor
    {
        public override string EncryptText(string text)
        {
            return Encryption.Encrypt(text, Encryption.EncryptionType.Random);
        }

        public override string DecryptText(string text)
        {
            return Encryption.Decrypt(text, Encryption.EncryptionType.Random);
        }

        public override void SetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Encryption.EncryptionKey = Encryption.GenerateKey();
            }
            else
            {
                Encryption.EncryptionKey = Encryption.GenerateKey(key);
            }
        }
    }
}
