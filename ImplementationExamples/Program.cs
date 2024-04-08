using System.Security.Cryptography;
using System.Text;

namespace basic
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "Hello World!";

            // MD5
            string hash = MD5Hash(input);
            Console.WriteLine("The MD5 hash of " + input + " is: " + hash);

            Console.WriteLine("----------------------------------");
            // DES
            string key = "12345678";
            string iv = "00000000";
            string encrypted = DESEncrypt(input, key, iv);
            Console.WriteLine("The DES encryption of " + input + " is: " + encrypted);
            Console.WriteLine("The DES decryption is: " + DESDecrypt(encrypted, key, iv));

            Console.WriteLine("----------------------------------");
            // RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = rsa.ExportParameters(true);
            Console.WriteLine("The RSA encryption of " + input + " is: " + BitConverter.ToString(RSAEncrypt(input, parameters, false)));
            Console.WriteLine("The RSA decryption is: " + RSADecrypt(RSAEncrypt(input, parameters, false), parameters, false));


        }

        static string MD5Hash(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = MD5.Create().ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes);
        }

        static string DESEncrypt(string input, string key, string iv)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv);

            DES des = DES.Create();
            ICryptoTransform encryptor = des.CreateEncryptor(keyBytes, ivBytes);

            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return BitConverter.ToString(encryptedBytes);
        }

        static string DESDecrypt(string input, string key, string iv)
        {
            // TODO: Implement DES decryption

            return "";
        }

        public static byte[] RSAEncrypt(string DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {

            byte[] data = Encoding.UTF8.GetBytes(DataToEncrypt);

            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    encryptedData = RSA.Encrypt(data, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public static string RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return Encoding.ASCII.GetString(decryptedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
    }
}
