using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using System.IO;
using System.Security.Cryptography;

namespace socketServidor
{
    public class FileEncryption
    {
        public String AESKey { set; get; }
        ICryptoTransform encryptor;
        ICryptoTransform decryptor;


        public void makeKey()
        {
            //using (var rijndael = System.Security.Cryptography.Rijndael.Create())
            //{
            var aes = new AesCryptoServiceProvider();
            aes.GenerateKey();
            AESKey = Convert.ToBase64String(aes.Key);
            //}
        }

        public void saveKey(String fileout, String publicKeyFile) {
            byte[] plainTextBytes = Convert.FromBase64String(AESKey);

            PemReader pr = new PemReader(
                (StreamReader)File.OpenText(publicKeyFile)
            );  //"./public.pem"
            RsaKeyParameters keys = (RsaKeyParameters)pr.ReadObject();
 
            OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(true, keys);

            int length = plainTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> cipherTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                cipherTextBytes.AddRange(eng.ProcessBlock(
                    plainTextBytes, chunkPosition, chunkSize
                ));
            }
            //Inicializar encryptor
            var aes = new AesCryptoServiceProvider();
            //aes.GenerateKey();
            byte[] clave = plainTextBytes.ToArray();
            byte[] iv = Convert.FromBase64String("Enf3G3rTMC7lwBJmtVcRvQ==");

            aes = new AesCryptoServiceProvider();
            encryptor = aes.CreateEncryptor(clave, iv);
            decryptor = aes.CreateDecryptor(clave, iv);
            File.WriteAllBytes(fileout, cipherTextBytes.ToArray());
        }

        public void loadKey(String filein, String privateKeyFile) {            
            byte[] cipherTextBytes = File.ReadAllBytes(filein);

            PemReader pr = new PemReader(
                (StreamReader)File.OpenText(privateKeyFile)
            ); //"./private.pem"
            AsymmetricCipherKeyPair keys = (AsymmetricCipherKeyPair)pr.ReadObject();

            OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(false, keys.Private);

            int length = cipherTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> plainTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                plainTextBytes.AddRange(eng.ProcessBlock(
                    cipherTextBytes, chunkPosition, chunkSize
                ));
            }

            //Inicializar encryptor
            var aes = new AesCryptoServiceProvider();
            //aes.GenerateKey();          
            byte[] clave = plainTextBytes.ToArray();
            byte[] iv = Convert.FromBase64String("Enf3G3rTMC7lwBJmtVcRvQ==");
            
            aes = new AesCryptoServiceProvider();
            encryptor = aes.CreateEncryptor(clave, iv);
            decryptor = aes.CreateDecryptor(clave, iv);

            AESKey = Convert.ToBase64String(plainTextBytes.ToArray());
        }



        public void EncryptFile(String infile, String outfile)
        {
            string file = infile;            

            //byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            //byte[] passwordBytes = Convert.FromBase64String(AESKey);

            using (var inputFileStream = new FileStream(infile, FileMode.Open, FileAccess.Read))
            {
                using (var outputFileStream = new FileStream(outfile, FileMode.Create, FileAccess.Write))
                using (var cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                {
                    inputFileStream.CopyTo(cryptoStream);
                }
            }

            // Hash the password with SHA256
            //passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            //byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            //string fileEncrypted = "C:\\SampleFileEncrypted.DLL";

            //File.WriteAllBytes(outfile, bytesEncrypted);
        }

        public void DecryptFile(String infile, String outfile)
        {
            using (var inputFileStream = new FileStream(infile, FileMode.Open, FileAccess.Read))
            {
                using (var cryptoStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
                using (var outputFileStream = new FileStream(outfile, FileMode.Create, FileAccess.Write))
                {
                    cryptoStream.CopyTo(outputFileStream);
                }
            }
        }

             

    }
}
