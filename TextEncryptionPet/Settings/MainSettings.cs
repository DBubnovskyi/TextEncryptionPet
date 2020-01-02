using System.Collections.Generic;
using TextEncryptionPet.Interfaces;
using TextEncryptionPet.Processors;

namespace TextEncryptionPet.Settings
{
    static class MainSettings
    {
        public static List<ITextProcessor> TextProcessors = new List<ITextProcessor>
        {
            new StaticEncryption(), new RandomEncryption(), new SumEncryption("Â¿¬æé"){}, 
            new SumEncryption("ΜʆǦΆΦ"){ RandomMin = 161, RandomMax = 887},
            new SumEncryption("ևӣѼ֊"){ RandomMin = 900, RandomMax = 1366},
            new SumEncryption("߰߆ޟࠌࠓ"){ RandomMin = 1785, RandomMax = 1969},
            new SumEncryption("ᘏ᎓ሸᖯᗴ"){ RandomMin = 4096, RandomMax = 5651},
            new SumEncryption("ᛃᗈᔴᚴᛑ"){ RandomMin = 5121, RandomMax = 5788},
            new SumEncryption("⠮❵✃⠬⡂"){ RandomMin = 9725, RandomMax = 10239},
            new SumEncryption("⤻⣲⢻⥐⥛"){ RandomMin = 10241, RandomMax = 10495},
            new SumEncryption("⮀⨉⬷⭘⯤"){ RandomMin = 10496, RandomMax = 11123},
            new SumEncryption("⹖⳾⸕⸴⺵"){ RandomMin = 11264, RandomMax = 11844},
            new SumEncryption("ㆷ㄁ㆢㆶ㈀"){ RandomMin = 12353, RandomMax = 12686},
            new SumEncryption("㌸㋵㍂㍎㍱"){ RandomMin = 12896, RandomMax = 13054},
            new SumEncryption("㐯㎬㐨㐹㑱"){ RandomMin = 13055, RandomMax = 13311},
            new SumEncryption("䮑㫒䜛䡇上"){ RandomMin = 13312, RandomMax = 19893},
            new SumEncryption("榢嗱摞斾沃"){ RandomMin = 19969, RandomMax = 27699},
        };
    }
}
