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
            table[0] = "000";
            table[1] = "000000";
            table[2] = "0000";
            table[3] = "000000";
            table[4] = "00000";
            table[5] = "00000000";
            return table[type];
        }
    }
}