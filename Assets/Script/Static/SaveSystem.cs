using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class SaveSystem
{
    [System.Serializable]
    public struct SaveData
    {
        //public SoftMaxData SDM;
    }
    private static SaveData save = new();
    private static string fileName = Application.persistentDataPath + "/save.save";

    public static void Save()
    {
        HandleSave();
        File.WriteAllText(fileName, JsonUtility.ToJson(save, true));

        //string json = JsonUtility.ToJson(save, true);
        //string encryptData = EncryptString(json);
        //File.WriteAllText(fileName, encryptData);
        Debug.Log("Saved");
    }

    private static void HandleSave()
    {
        //AIModel.SaveData(ref save.SDM);
    }

    public static void Load()
    {
        if(!File.Exists(fileName))
        {
            //AIModel.LoadData();
            return;
        }
        Debug.Log("Loaded");
        string saveContent = File.ReadAllText(fileName);
        save = JsonUtility.FromJson<SaveData>(saveContent);
        //string decryptData = DecryptString(saveContent);
        //save = JsonUtility.FromJson<SaveData>(decryptData);

        HandleLoad();
    }
    private static void HandleLoad()
    {
        //AIModel.LoadData(save.SDM);
    }

    private static readonly string encryptionKey = "ds58we832goidhf9b837g8ufb920ndjvbribno93";
    public static string EncryptString(string plainText)
    {
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 32));
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV();
            ICryptoTransform encrypter = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using(var csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Write))
                {
                    using(var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    public static string DecryptString(string text)
    {
        byte[] fullCipher = Convert.FromBase64String(text);
        byte[] iv = new byte[16];
        byte[] cipher = new byte[fullCipher.Length - 16];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

        byte[] key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 32));
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform decrypter = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decrypter, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
                
            }
        }
    }
}
