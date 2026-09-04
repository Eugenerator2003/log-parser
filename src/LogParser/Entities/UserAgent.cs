using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public class UserAgent : BaseEntity
    {
        public string Name { get; set; }

        public UserAgent(int id, string name) : base(id)
        {
            Name = name;
        }

        protected UserAgent() : base(0)
        {
            Name = string.Empty;
        }
    }
}
