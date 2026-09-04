using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public class Status
    {
        public int Code { get; set; }

        public Status(int code)
        {
            Code = code;
        }

        protected Status()
        {
        }
    }
}
