using System.Security.Cryptography;
using System.Text;

namespace signverify
{
    public class SignVerify
    {
        public static void Main(string[] args)
        {
            string message = "Hello World!";

            Console.WriteLine("Original Message: " + message);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                string signedMessage = SignData(message, rsa.ExportParameters(true));
                Console.WriteLine("Signed Message: " + signedMessage);
                Console.WriteLine("Signature Verified: " + VerifyData(message, signedMessage, rsa.ExportParameters(false)));
            }

        }

        public static string SignData(string message, RSAParameters privateKey)
        {
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    rsa.ImportParameters(privateKey);
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, RSAParameters publicKey)
        {
            var encoder = new UTF8Encoding();
            byte[] bytesToVerify = encoder.GetBytes(originalMessage);
            byte[] signedBytes = Convert.FromBase64String(signedMessage);
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(publicKey);
                    return rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}