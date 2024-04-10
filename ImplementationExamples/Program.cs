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
            byte[] encrypted = DESEncrypt(Encoding.ASCII.GetBytes(input), key, iv);
            Console.WriteLine("The DES encryption of " + input + " is: " + BitConverter.ToString(encrypted));
            Console.WriteLine("The DES decryption is: " + Encoding.ASCII.GetString(DESDecrypt(encrypted, key, iv)));

            Console.WriteLine("----------------------------------");
            // RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = rsa.ExportParameters(true);
            Console.WriteLine("The RSA encryption of " + input + " is: " + BitConverter.ToString(RSAEncrypt(input, parameters, false)));
            Console.WriteLine("The RSA decryption is: " + RSADecrypt(RSAEncrypt(input, parameters, false), parameters, false));

            // Custom
            key = "12345678";
            encrypted = CustomEncrypt(input, key);
            Console.WriteLine("The Custom encryption of " + input + " is: " + BitConverter.ToString(encrypted));
            Console.WriteLine("The Custom decryption is: " + CustomDecrypt(encrypted, key));


        }

        static string MD5Hash(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = MD5.Create().ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes);
        }

        static byte[] DESEncrypt(byte[] input, string key, string iv)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv);

            using (DES des = DES.Create())
            {
                ICryptoTransform encryptor = des.CreateEncryptor(keyBytes, ivBytes);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(input, 0, input.Length);
                return encryptedBytes;
            }
        }

        static byte[] DESDecrypt(byte[] input, string key, string iv)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv);

            using (DES des = DES.Create())
            {
                ICryptoTransform decryptor = des.CreateDecryptor(keyBytes, ivBytes);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(input, 0, input.Length);
                return decryptedBytes;
            }
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
        public static byte[] CustomEncrypt(string input, string key)
        {
            byte[] output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte)(input[i] ^ key[i % key.Length]);
            }
            return output;
        }

        public static string CustomDecrypt(byte[] input, string key)
        {
            char[] output = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (char)(input[i] ^ key[i % key.Length]);
            }
            return new string(output);
        }
    }

}
