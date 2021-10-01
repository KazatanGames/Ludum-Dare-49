namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System;
    using System.Text;
    using System.Security.Cryptography;

    /**
     * Base Persistance
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * This is the base class for other persistance classes whose roles are to persist
     * data between game sessions.
     */
    public abstract class BasePersistance<T>
    {
        protected string baseFilePath = "sav-";

        protected T data;

        protected abstract string Filepath { get; }

        public T Data { get { return data; } }

        public BasePersistance()
        {
            CreateData();
            Initialise();
        }

        public void Save()
        {
            WriteData();
        }

        protected void CreateData()
        {
            if (File.Exists(Application.persistentDataPath + "/" + baseFilePath + Filepath))
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream file = File.Open(Application.persistentDataPath + "/" + baseFilePath + Filepath, FileMode.Open))
                    {
                        using (BinaryReader br = new BinaryReader(file))
                        {
                            byte[] fileData = br.ReadBytes((int)file.Length);
                            fileData = DecryptBytes(fileData);
                            using (MemoryStream ms = new MemoryStream(fileData))
                            {
                                data = (T)bf.Deserialize(ms);
                            }
                        }
                    }
                }
                catch
                {
                    Debug.LogWarning("Failed to load or parse save data. Starting a new data.");
                    data = (T)Activator.CreateInstance(typeof(T));
                }
            }
            else
            {
                data = (T)Activator.CreateInstance(typeof(T));
            }
        }

        protected void WriteData()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(Application.persistentDataPath + "/" + baseFilePath + Filepath, FileMode.OpenOrCreate))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, Data);
                    using (BinaryWriter bw = new BinaryWriter(file))
                    {
                        byte[] fileData = EncryptBytes(ms.ToArray());
                        bw.Write(fileData);
                    }
                }
            }
        }

        protected virtual void Initialise()
        {
            // do nothing by default
        }

        protected byte[] EncryptBytes(byte[] input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Secrets.EncryptionKey));
                using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform tr = trip.CreateEncryptor();
                    return tr.TransformFinalBlock(input, 0, input.Length);
                }
            }
        }

        protected byte[] DecryptBytes(byte[] input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Secrets.EncryptionKey));
                using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform tr = trip.CreateDecryptor();
                    return tr.TransformFinalBlock(input, 0, input.Length);
                }
            }
        }
    }
}