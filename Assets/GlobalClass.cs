using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.Assets
{
    public static class GlobalClass
    {
        public static string DefaultCurrentStatus(int type)
        {
            var table = new Dictionary<int, string>();
            table[0] = "00";
            table[1] = "00000";
            table[2] = "000";
            table[3] = "00000";
            table[4] = "0000";
            table[5] = "0000000";
            return table[type];
        }
    }
}