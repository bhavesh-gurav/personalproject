using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabourCommissioner.Abstraction
{
    public static class EnumLookup
    {
        public enum ResponseMsgType
        {
            success=1,
            error=2,
            warning=3,
            info=4
        }
        public enum ResourcesType
        {
            Education = 1,
            Degree = 2,
            Semester = 3
        }

        public enum EmailPurpose
        {
            V = 1,
            A = 2,
            R = 3
        }


    }
}
