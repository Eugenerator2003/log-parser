using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public class RemoteAddress : BaseEntity
    {
        public string Address { get; set; }

        public RemoteAddress(int id, string address) : base(id)
        {
            Address = address;
        }

        protected RemoteAddress() : base(0)
        {
            Address = string.Empty;
        }
    }
}
