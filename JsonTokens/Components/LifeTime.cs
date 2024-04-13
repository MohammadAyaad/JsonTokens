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
            ValidAt = validAt;
            ExpiresAt = expiresAt;
        }

        public long ValidAt { get; }
        public long ExpiresAt { get; }
        public bool IsValid()
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return ((now > ValidAt) && (now < ExpiresAt));
        }
        public bool IsExpired()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() > ExpiresAt;
        } 
        
    }
}
