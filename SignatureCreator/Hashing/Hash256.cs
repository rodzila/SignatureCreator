using System;
using System.Security.Cryptography;
using System.Text;


namespace SignatureCreator.Hashing
{
    class Hash256
    {
        SHA256 hash256 = SHA256Managed.Create();

        public string ToString(byte[] hash)
        {
            StringBuilder hashString = new StringBuilder();
            int j;
            for(j=0; j < hash.Length; j++)
            {
                hashString.Append(hash[j].ToString("x2"));
                if ((j % 4) == 3) hashString.Append(" ");
            }
            return hashString.ToString();
        }

        public byte[] GetHash(byte[] block)
        {
            return hash256.ComputeHash(block);
        }

    }
}
