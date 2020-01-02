using System;
using System.Collections.Generic;
using System.Text;

namespace TextEncryptionPet.Interfaces
{
    interface ITextProcessor
    {
        string ProcessorName { get; }
        void SetKey(string key);
        string DecryptText(string text);
        string EncryptText(string text);
    }
}
