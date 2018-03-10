using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Slidable.Presenter.Authentication
{
    public class ApiKeyCheck
    {
        private readonly HMACSHA256 _hash;

        public ApiKeyCheck(IOptionsMonitor<ApiKeyOptions> options)
        {
            using (var sha = SHA512.Create())
            {
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(options.CurrentValue.ApiKeyHashPhrase));
                _hash = new HMACSHA256(hashBytes);
            }
        }

        public bool Check(ReadOnlySpan<char> authorizationHeader, out string userName)
        {
            int colon = authorizationHeader.IndexOf(':');
            if (colon < 0)
            {
                userName = default;
                return false;
            }

            var user = GetUserName(authorizationHeader, colon);
            var key = GetKey(authorizationHeader, colon);

            if (Check(user, key))
            {
                userName = new string(user);
                return true;
            }

            userName = default;
            return false;
        }

        private bool Check(ReadOnlySpan<char> user, ReadOnlySpan<char> key)
        {
            var pool = ArrayPool<byte>.Shared;
            var userBytes = pool.Rent(16);
            var keyBytes = pool.Rent(512);
            var hashBytes = pool.Rent(512);

            try
            {
                var userByteSpan = GetUserBytes(userBytes, user);
                var keyByteSpan = TryGetKeyBytes(keyBytes, key, out bool keyOk);
                if (keyOk)
                {
                    var hashByteSpan = TryGetHashBytes(hashBytes, userByteSpan, out bool hashOk);
                    return hashOk && keyByteSpan.SequenceEqual(hashByteSpan);
                }
            }
            finally
            {
                pool.Return(userBytes);
                pool.Return(keyBytes);
                pool.Return(hashBytes);
            }

            return false;
        }

        private ReadOnlySpan<byte> TryGetHashBytes(byte[] buffer, ReadOnlySpan<byte> user, out bool success)
        {
            var span = buffer.AsSpan();
            success = _hash.TryComputeHash(user, span, out int count);
            return success ? span.Slice(0, count) : default;
        }

        private static ReadOnlySpan<byte> TryGetKeyBytes(byte[] buffer, ReadOnlySpan<char> key, out bool success)
        {
            var span = buffer.AsSpan();
            success = Convert.TryFromBase64Chars(key, span, out int count);
            return success ? span.Slice(0, count) : default;
        }

        private static ReadOnlySpan<byte> GetUserBytes(byte[] buffer, ReadOnlySpan<char> user)
        {
            var span = buffer.AsSpan();
            int count = Encoding.UTF8.GetBytes(user, span);
            return span.Slice(0, count);
        }

        public static ReadOnlySpan<char> GetUserName(ReadOnlySpan<char> header, int colon)
        {
            return header.Slice(8, colon - 8);
        }

        public static ReadOnlySpan<char> GetKey(ReadOnlySpan<char> header, int colon)
        {
            return header.Slice(colon + 1);
        }
    }
}