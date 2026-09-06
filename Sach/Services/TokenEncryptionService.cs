using System;
using System.Security.Cryptography;
using System.Text;

namespace Sach.Services;

public interface ITokenEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
}

public class TokenEncryptionService : ITokenEncryptionService
{
    private const string EncryptionPrefix = "DPAPI:";

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = System.Security.Cryptography.ProtectedData.Protect(
                plaintextBytes,
                null,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);

            return EncryptionPrefix + Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to encrypt token", ex);
        }
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        try
        {
            // Support legacy plaintext tokens for backward compatibility
            if (!encryptedText.StartsWith(EncryptionPrefix))
                return encryptedText;

            string base64String = encryptedText[EncryptionPrefix.Length..];
            byte[] encryptedBytes = Convert.FromBase64String(base64String);
            byte[] plaintextBytes = System.Security.Cryptography.ProtectedData.Unprotect(
                encryptedBytes,
                null,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(plaintextBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to decrypt token", ex);
        }
    }
}
