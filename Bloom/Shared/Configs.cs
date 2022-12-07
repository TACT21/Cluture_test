using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Shared
{
    public static class Configs
    {
        //生徒判別用
        /// <summary>
        /// 生徒確認時の返却値
        /// </summary>
        public static string judgementStudent = "masuofes";
        /// <summary>
        /// 生徒確認用のGasURL
        /// </summary>
        public static string judgemantUrl = "https://script.google.com/a/macros/sit-kashiwa.com/s/AKfycbzevGdW9UVerFOP7KCeAIm1Gtu61dW568a3yBkLrDNZlz2_BVIE9Bu3Tnl1-stJNCcY5Q/exec";

        /// <summary>
        /// 増尾大賞用。もし、NULLではなければ、そこに遷移。
        /// </summary>
        public static string? URL = null;
    }

    
}
