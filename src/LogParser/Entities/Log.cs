using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Entities
{
    public class Log : BaseEntity
    {
        public RemoteAddress RemoteAddress { get; }

        public RemoteUser RemoteUser { get; }

        public Status Status { get; }

        public UserAgent UserAgent { get; }

        public string Request { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public int BytesCount { get; set; }

        public string HttpReferer { get; set; }

        public Log(
            int id, 
            RemoteAddress remoteAddress, 
            RemoteUser remoteUser, 
            Status status, 
            UserAgent userAgent, 
            string request, 
            DateTimeOffset dateTimeOffset, 
            int bytesCount, 
            string httpReferer) : base(id)
        {
            RemoteAddress = remoteAddress;
            RemoteUser = remoteUser;
            Status = status;
            UserAgent = userAgent;
            Request = request;
            DateTimeOffset = dateTimeOffset;
            BytesCount = bytesCount;
            HttpReferer = httpReferer;
        }

        protected Log() : base(0)
        {
            RemoteAddress = null!;
            RemoteUser = null!;
            Status = null!;
            UserAgent = null!;
            Request = string.Empty;
            HttpReferer = string.Empty;
        }
    }
}
