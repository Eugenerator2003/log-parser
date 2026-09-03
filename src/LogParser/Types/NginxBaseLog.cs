using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Types
{
    public class NginxBaseLog
    {
        public string RemoteIp;
        public string RemoteUser;
        public DateTime TimeLocal;
        public string Request;
        public int Status;
        public int BodyBytesSent;
        public string HttpReferer; 
        public string UserAgent;

        public NginxBaseLog(string remoteIp, string remoteUser, DateTime timeLocal, string request, int status, int bodyBytesSent, string httpReferer, string userAgent)
        {
            RemoteIp = remoteIp;
            RemoteUser = remoteUser;
            TimeLocal = timeLocal;
            Request = request;
            Status = status;
            BodyBytesSent = bodyBytesSent;
            HttpReferer = httpReferer;
            UserAgent = userAgent;
        }

        public override bool Equals(object? obj)
        {
            return obj is NginxBaseLog log &&
                   RemoteIp == log.RemoteIp &&
                   RemoteUser == log.RemoteUser &&
                   TimeLocal == log.TimeLocal &&
                   Request == log.Request &&
                   Status == log.Status &&
                   BodyBytesSent == log.BodyBytesSent &&
                   HttpReferer == log.HttpReferer &&
                   UserAgent == log.UserAgent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RemoteIp, RemoteUser, TimeLocal, Request, Status, BodyBytesSent, HttpReferer, UserAgent);
        }
    }
}
