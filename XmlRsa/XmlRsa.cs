using System.Security.Cryptography;
using System.Text;

namespace xmlrsa
{
    public class XmlRsa
    {
        public static void Main(string[] args)
        {
            Console.WriteLine();

            string publicKey = "<RSAKeyValue><Modulus>yXA14h2M/bLlMsERDyy9ErTh6VyFklaN+8GfMxQkVNcx9IS0AESWgVX49w802EcrYZib1oGJuPW1HTX1A1eyb75JXkolq2mSHr+GCjMqV7xlTljOf7u42av0kZKNOvWUQEDo/aOtYpOZH7CSmA3f4dhBGdLoh7TqhB/fGdMwnEk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            string privateKey = "<RSAKeyValue><Modulus>yXA14h2M/bLlMsERDyy9ErTh6VyFklaN+8GfMxQkVNcx9IS0AESWgVX49w802EcrYZib1oGJuPW1HTX1A1eyb75JXkolq2mSHr+GCjMqV7xlTljOf7u42av0kZKNOvWUQEDo/aOtYpOZH7CSmA3f4dhBGdLoh7TqhB/fGdMwnEk=</Modulus><Exponent>AQAB</Exponent><P>904FD9XlvXN2bF4lOrEID/Coo64M5yJBDJd2XAHEzG/THadrSHag0Wt5VfCEOqJG1T0gVm+iEZI8uxN/Z8FM8w==</P><Q>0IVZRNVK+6BHQJoaTL19Nv8k1I/wEj9F57xRBxy/YcwD9LrGiFGEpqkvoEq80SNogjcrxoAnY6ktw4aIdXUQ0w==</Q><DP>FaIwlSsL9t+z21T9Ar5byzEtP2xJWqrHb/eL9g59jbi2iiCMJQGjnc4+BgONPafWdG7tdkI7tjfJsj/JZGUnew==</DP><DQ>zoDxCikNbCqrxb+Xgh46jieZyuSNRRTiXv/xYtDGe8y2sjvyd3f7na15nA3H9npRenu234t09s7JopRuOZxovw==</DQ><InverseQ>0BmUXX/99QckT3mII2mXnVbDQdWF1EE1N90mhAOZI4gIBMqliNmU5p5JekybmIreosgxpPb+uHasxAcCkHJJpg==</InverseQ><D>XMjV2Pg5mCYN0ooBfJbj5DjzEhVu5Q//SGZQGSv+7CFPPkzdyH3PQXXDPrF/atulTMgJiA2UzWBH81OGLur3IhVzFtoQd3DSVY5BR4hA9FjsLdfCWKiBpnU0l8yvV3uM6q3mov3aX4JMSo/caRRr0rVnIJ3gHGmjEtAoy3Qcty0=</D></RSAKeyValue>";

            string message = "Hello World!";
            Console.WriteLine("Original Message: " + message);
            byte[] encryptedMessage = RSAEncrypt(message, privateKey);
            Console.WriteLine("Encrypted Message: " + BitConverter.ToString(encryptedMessage));
            Console.WriteLine("Decrypted Message: " + RSADecrypt(encryptedMessage, privateKey));
        }

        public static void generarClaus()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            FileStream fsPrivate = new FileStream("C:\\Users\\Sergio\\Desktop\\priv.xml", FileMode.Create);
            StreamWriter swPriv = new StreamWriter(fsPrivate);

            FileStream fsPublic = new FileStream("C:\\Users\\Sergio\\Desktop\\pub.xml", FileMode.Create);
            StreamWriter swPub = new StreamWriter(fsPublic);

            string xmlPrivateKey = rsa.ToXmlString(true);
            string xmlPublicKey = rsa.ToXmlString(false);

            swPriv.Write(xmlPrivateKey);
            swPub.Write(xmlPublicKey);

            swPriv.Close();
            swPub.Close();

            fsPrivate.Close();
            fsPublic.Close();

            Console.WriteLine("Keys generated and saved to files.");
            Console.WriteLine("----------PUBLIC KEY----------");
            Console.WriteLine(xmlPublicKey);
            Console.WriteLine("----------PRIVATE KEY----------");
            Console.WriteLine(xmlPrivateKey);
        }

        public static byte[] RSAEncrypt(string missatge, string clauXml)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(clauXml);

            byte[] missatgeBytes = Encoding.ASCII.GetBytes(missatge);
            byte[] missatgeEncriptat = rsa.Encrypt(missatgeBytes, false);

            return missatgeEncriptat;
        }

        public static string RSADecrypt(byte[] missatge, string clauXml)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(clauXml);

            byte[] missatgeDesencriptat = rsa.Decrypt(missatge, false);
            string missatgeDesencriptatString = Encoding.ASCII.GetString(missatgeDesencriptat);

            return missatgeDesencriptatString;
        }
    }
}