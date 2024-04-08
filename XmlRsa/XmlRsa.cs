using System.Security.Cryptography;

namespace xmlrsa
{
    public class XmlRsa
    {
        public static void Main(string[] args)
        {
            string input = "Hello World!";
            string Key_Name = "xmlrsa";


            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                using (
                    var fs = new FileStream(
                        String.Concat("C:\\Users\\Sergio\\Desktop", "\\", Key_Name, ".xml"), FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        rsa.PersistKeyInCsp = true;
                        sw.WriteLine(rsa.ToXmlString(true));
                    }
                }
            }

        }
    }
}