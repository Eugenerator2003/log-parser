using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public class RemoteUser : BaseEntity
    {
        public string Name { get; set; }

        public RemoteUser(int id, string name) : base(id)
        {
            Name = name;
        }

        protected RemoteUser() : base(0)
        {
            Name = string.Empty;
        }
    }
}
