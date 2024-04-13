using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens.Components
{
    public class Sequential
    {
        private byte[] sid;
        private byte[] ssi;
        private long sqi;
        private long mlen; //max length
        public byte[] SID { get { return sid; } private set { sid = value; } }
        public byte[] SSI { get { return ssi; } private set { ssi = value; } }
        public long SQI { get { return sqi; } private set { sqi = value; } }
        public long MLEN { get { return mlen; } private set { mlen = value; } }
        [JsonConstructor]
        private Sequential(byte[] sid, byte[] ssi, long sqi,long max_length)
        {
            this.sid = sid;
            this.ssi = ssi;
            this.sqi = sqi;
            mlen = max_length;
        }
        public void Advance()
        {
            byte[] shasid = SHA256.HashData(sid);
            ssi = GenerateSsi(sid, shasid, ssi);
        }
        public static Sequential GenerateSequence(long max_length)
        {
            byte[] sid = RandomNumberGenerator.GetBytes(64);
            byte[] shasid = SHA256.HashData(sid);

            byte[] ssi = new byte[32];
            for (int i = 0; i < ssi.Length; i++) ssi[i] = 0;
            ssi = GenerateSsi(sid, shasid, ssi);
            long fsqi = 0;
            return new Sequential(sid, ssi, fsqi, max_length);
        }
        public bool AuthenticateSequence()
        {
            byte[] shasid = SHA256.HashData(SID);
            byte[] essi = new byte[32];
            for (int i = 0; i < essi.Length; i++) essi[i] = 0;
            for (int i = 0; i <= SQI; i++) essi = GenerateSsi(SID, shasid, essi);
            bool result = true;
            for (int i = 0; i < essi.Length; i++) if (essi[i] != SSI[i]) { result = false; break; }
            return result;
        }
        
        private static byte[] GenerateSsi(byte[] sid, byte[] shasid, byte[] currentSqi)
        {
            byte[] a = SHA256.HashData(MergeByteArrays(sid, shasid, currentSqi));

            byte[] b0 = SHA256.HashData(ByteArrayXorLoop(sid, a));
            byte[] b1 = SHA256.HashData(ByteArrayXorLoop(shasid, sid));
            byte[] b2 = SHA256.HashData(ByteArrayXorLoop(currentSqi, sid));
            byte[] b3 = SHA256.HashData(ByteArrayXorLoop(currentSqi, shasid));

            byte[] c0 = SHA256.HashData(ByteArrayXorLoop(b0, b1));
            byte[] c1 = SHA256.HashData(ByteArrayXorLoop(b1, b2));
            byte[] c2 = SHA256.HashData(ByteArrayXorLoop(b2, b3));

            byte[] d0 = SHA256.HashData(ByteArrayXorLoop(c0, c1));
            byte[] d1 = SHA256.HashData(ByteArrayXorLoop(c1, c2));

            byte[] e = SHA256.HashData(ByteArrayXorLoop(d0, d1));

            return SHA256.HashData(e);
        }
        private static byte[] MergeByteArrays(params byte[][] arrays)
        {
            int lensum = 0;
            foreach (byte[] array in arrays) lensum += array.Length;
            byte[] bigarray = new byte[lensum];
            int index = 0;

            for (int y = 0; y < arrays.Length; y++)
                for (int x = 0; x < arrays[y].Length; x++, index++) bigarray[index] = arrays[y][x];
            return bigarray;
        }

        private static byte[] ByteArrayXorLoop(byte[] a, byte[] b)
        {
            byte[] r = new byte[a.Length];
            for (int x = 0; x < b.Length; x++) r[x % r.Length] = (byte)((int)a[x % r.Length] ^ (int)b[x]);
            return r;
        }
    }

}
