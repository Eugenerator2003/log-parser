using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        protected BaseEntity(int id)
        {
            Id = id;
        }
    }
}
