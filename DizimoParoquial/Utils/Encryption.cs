using System.Security.Cryptography;
using System.Text;

namespace DizimoParoquial.Utils
{
    public class Encryption
    {

        public string EncryptPassword(string passwordEncripted, string key)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        // Gera o hash da chave usando SHA256
                        byte[] byteHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));

                        // Configura a chave e o modo de operação para AES
                        aesAlg.Key = byteHash;
                        aesAlg.Mode = CipherMode.ECB;
                        aesAlg.Padding = PaddingMode.PKCS7;

                        // Converte a senha para byte e criptografa
                        byte[] byteBuff = Encoding.UTF8.GetBytes(passwordEncripted);
                        byte[] encrypted = aesAlg.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);

                        // Retorna a senha criptografada em base64
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
            catch (Exception ex)
            {
                return "Erro ao criptografar: " + ex.Message;
            }
        }

    }
}
