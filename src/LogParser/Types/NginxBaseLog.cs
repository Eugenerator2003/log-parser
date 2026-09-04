using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Types
{
    public readonly struct NginxBaseLog
    {
        public string RemoteIp { get; init; }
        public string RemoteUser { get; init; }
        public DateTimeOffset TimeLocal { get; init; }
        public string Request { get; init; }
        public int Status { get; init; }
        public int BodyBytesSent { get; init; }
        public string HttpReferer { get; init; }
        public string UserAgent { get; init; }

        public NginxBaseLog(string remoteIp, string remoteUser, DateTimeOffset timeLocal, string request, int status, int bodyBytesSent, string httpReferer, string userAgent)
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

        public static bool operator ==(NginxBaseLog left, NginxBaseLog right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NginxBaseLog left, NginxBaseLog right)
        {
            return !(left == right);
        }
    }
}
