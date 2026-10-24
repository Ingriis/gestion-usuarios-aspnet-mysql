using System;
using System.Security.Cryptography;
using System.Text;

namespace apiRestCtrlUsuariosFull.Models
{
    internal static class PasswordHasher
    {
        private const int Iterations = 600000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string Prefix = "pbkdf2-sha256";

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña es obligatoria.", "password");

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash = DeriveKey(password, salt, Iterations, HashSize);
            return Prefix + "$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string storedValue, out bool needsUpgrade)
        {
            needsUpgrade = false;
            if (password == null || string.IsNullOrEmpty(storedValue))
                return false;

            string[] parts = storedValue.Split('$');
            if (parts.Length != 4 || parts[0] != Prefix)
            {
                if (storedValue.StartsWith("pbkdf2-", StringComparison.Ordinal))
                    return false;

                // Compatibility for existing databases: migrate a legacy plaintext
                // password to PBKDF2 after the first successful sign-in.
                needsUpgrade = true;
                return FixedTimeEquals(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(storedValue));
            }

            int rounds;
            byte[] salt;
            byte[] expected;
            if (!int.TryParse(parts[1], out rounds) || rounds < 100000 || rounds > 1000000)
                return false;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            if (salt.Length != SaltSize || expected.Length != HashSize)
                return false;

            byte[] actual = DeriveKey(password, salt, rounds, expected.Length);
            needsUpgrade = rounds < Iterations;
            return FixedTimeEquals(actual, expected);
        }

        private static byte[] DeriveKey(string password, byte[] salt, int rounds, int length)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(passwordBytes))
            {
                int blockCount = (length + hmac.HashSize / 8 - 1) / (hmac.HashSize / 8);
                byte[] result = new byte[length];
                byte[] blockInput = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, blockInput, 0, salt.Length);

                for (int block = 1; block <= blockCount; block++)
                {
                    blockInput[salt.Length] = (byte)(block >> 24);
                    blockInput[salt.Length + 1] = (byte)(block >> 16);
                    blockInput[salt.Length + 2] = (byte)(block >> 8);
                    blockInput[salt.Length + 3] = (byte)block;

                    byte[] u = hmac.ComputeHash(blockInput);
                    byte[] accumulator = (byte[])u.Clone();
                    for (int iteration = 1; iteration < rounds; iteration++)
                    {
                        u = hmac.ComputeHash(u);
                        for (int i = 0; i < accumulator.Length; i++)
                            accumulator[i] ^= u[i];
                    }

                    int offset = (block - 1) * accumulator.Length;
                    Buffer.BlockCopy(accumulator, 0, result, offset, Math.Min(accumulator.Length, length - offset));
                }

                return result;
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            int difference = left.Length ^ right.Length;
            int length = Math.Min(left.Length, right.Length);
            for (int i = 0; i < length; i++)
                difference |= left[i] ^ right[i];
            return difference == 0;
        }
    }
}
