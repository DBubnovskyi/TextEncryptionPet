using System;
using System.Collections.Generic;
using System.Text;
using TextEncryptionPet.Interfaces;

namespace TextEncryptionPet.Processors
{
    abstract class BaseProcessor : ITextProcessor
    {
        public virtual string ProcessorName => GetType().Name;
        public abstract void SetKey(string key);
        public abstract string DecryptText(string text);
        public abstract string EncryptText(string text);
    }
}
