using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace Framework.Web
{
    /// <summary>
    /// Class that encrypts and decrypts querystring params in the Url.
    /// </summary>
   public class EncryptedQueryString : Dictionary<string, string>
	{
        #region| Properties |

        // Change the following keys to ensure uniqueness -- Must be 8 bytes
        private byte[] _keyBytes = { 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18 };

        // Must be at least 8 characters
        private string _keyString = "ABC12345";

        // Name for checksum value (unlikely to be used as arguments by user)
        private string _checksumKey = "__$$"; 

        #endregion

        #region| Constructor |

        /// <summary>
        /// Creates an empty dictionary
        /// </summary>
        public EncryptedQueryString()
        {

        }

        /// <summary>
        /// Creates a dictionary from the given, encrypted string
        /// </summary>
        /// <param name="EncryptedData"></param>
        /// <example>
        /// <code>
        /// var args = new EncryptedQueryString(Request.QueryString["args"]);
        /// 
        /// var param1 = args["arg1"];
        /// var param2 = args["arg2"];
        /// var param3 = args["arg3"];
        /// </code>
        /// </example>
        public EncryptedQueryString(string EncryptedData)
        {
            // Descrypt string
            var data = Decrypt(EncryptedData);

            // Parse out key/value pairs and add to dictionary
            string checksum = null;
            string[] args = data.Split('&');

            foreach (string arg in args)
            {
                int i = arg.IndexOf('=');

                if (i != -1)
                {
                    string key = arg.Substring(0, i);
                    string value = arg.Substring(i + 1);

                    if (key == _checksumKey)
                    {
                        checksum = value;
                    }
                    else
                    {
                        base.Add(HttpUtility.UrlDecode(key), HttpUtility.UrlDecode(value));
                    }
                }
            }

            // Clear contents if valid checksum not found
            if (checksum == null || checksum != ComputeChecksum())
            {
                base.Clear();
            }
        } 
        
        #endregion

        #region| Methods |

        /// <summary>
        /// Returns an encrypted string that contains the current dictionary
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///    var args = new EncryptedQueryString();
        ///
        ///    args["arg1"] = "valor1";
        ///    args["arg2"] = "valor1";
        ///    args["arg3"] = "valor3";
        ///
        ///    Response.Redirect("Default2.aspx?args=" + args.ToString());
        ///    
        /// </code>
        /// </example>
        public override string ToString()
        {
            // Build query string from current contents
            var oSB = new StringBuilder();

            foreach (string key in base.Keys)
            {
                if (oSB.Length > 0)
                {
                    oSB.Append('&');
                }

                oSB.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(base[key]));
            }

            // Add checksum
            if (oSB.Length > 0)
            {
                oSB.Append('&');
            }

            oSB.AppendFormat("{0}={1}", _checksumKey, ComputeChecksum());

            return Encrypt(oSB.ToString());
        }

        /// <summary>
        /// Returns a simple checksum for all keys and values in the collection
        /// </summary>
        /// <returns></returns>
        private string ComputeChecksum()
        {
            int checksum = 0;

            foreach (KeyValuePair<string, string> pair in this)
            {
                checksum += pair.Key.Sum(c => c - '0');
                checksum += pair.Value.Sum(c => c - '0');
            }

            return checksum.ToString("X");
        }

        /// <summary>
        /// Encrypts the given text
        /// </summary>
        /// <param name="text">Text to be encrypted</param>
        /// <returns></returns>
        private string Encrypt(string text)
        {
            try
            {
                byte[] keyData = Encoding.UTF8.GetBytes(_keyString.Substring(0, 8));

                byte[] textData = Encoding.UTF8.GetBytes(text);

                var oCSP = new DESCryptoServiceProvider();

                var oMS = new MemoryStream();
                var oCS = new CryptoStream(oMS, oCSP.CreateEncryptor(keyData, _keyBytes), CryptoStreamMode.Write);

                oCS.Write(textData, 0, textData.Length);
                oCS.FlushFinalBlock();

                return GetString(oMS.ToArray());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Decrypts the given encrypted text
        /// </summary>
        /// <param name="text">Text to be decrypted</param>
        /// <returns></returns>
        private string Decrypt(string text)
        {
            try
            {
                byte[] keyData = Encoding.UTF8.GetBytes(_keyString.Substring(0, 8));
                byte[] textData = GetBytes(text);

                var oCSP = new DESCryptoServiceProvider();

                var oMS = new MemoryStream();
                var oCS = new CryptoStream(oMS, oCSP.CreateDecryptor(keyData, _keyBytes), CryptoStreamMode.Write);

                oCS.Write(textData, 0, textData.Length);
                oCS.FlushFinalBlock();

                return Encoding.UTF8.GetString(oMS.ToArray());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Converts a byte array to a string of hex characters
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetString(byte[] data)
        {
            var oSB = new StringBuilder();

            foreach (byte b in data)
            {
                oSB.Append(b.ToString("X2"));
            }

            return oSB.ToString();
        }

        /// <summary>
        /// Converts a string of hex characters to a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] GetBytes(string data)
        {
            // GetString() encodes the hex-numbers with two digits
            byte[] results = new byte[data.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
            {
                results[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }

            return results;
        } 
        
        #endregion
	}
}