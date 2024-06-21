using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.Components
{
    public class LifeTime
    {
        public LifeTime(long validAt, long expiresAt)
        {
            ValidFrom = validAt;
            ExpiresAt = expiresAt;
        }

        public long ValidFrom { get; }
        public long ExpiresAt { get; }
        public bool IsValid()
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return ((now > ValidFrom) && (now < ExpiresAt));
        }
        public bool IsExpired()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() > ExpiresAt;
        } 
        
    }
}
