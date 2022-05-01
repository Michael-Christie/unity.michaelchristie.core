using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

using UnityEngine;


/// <summary>
/// A helper class to save data to files
/// </summary>
public class SaveFileHelper
{
    /// Little background on encryption, AES has two components, the key and the IV. The key is a public key, that is used to decrypt the file. Without the key
    /// the file couldn't be decrypted. The IV (Index Vector) is where the data should start to look in the key while decrypting, kind of like how you can set a seed
    /// for a random number generator to get the same results. The IV is written into the file, where as the key needs to be saved somewhere and set before calling any
    /// serialization;

    private static string _key;
    public static string EncryptionKey
    {
        private get
        {
            if (string.IsNullOrEmpty(_key))
            {
                _key = PlayerPrefs.GetString("InstanceID", string.Empty);

                if(string.IsNullOrEmpty(_key))
                {
                    CreateEncryptionKey();
                }
            }

            return _key;
        }
        set
        {
            _key = value;
            encryptionKeyByte = Convert.FromBase64String(_key);

            PlayerPrefs.SetString("InstanceID", _key);
        }
    }

    private static byte[] encryptionKeyByte;

    /// <summary>
    /// Checks to see if a file exists
    /// </summary>
    /// <param name="_path"></param>
    /// <returns>True if the file exists, false if not</returns>
    public static bool DoesFileExist(in string _path)
    {
        return File.Exists($"{Application.persistentDataPath}{_path}");
    }

    public static void BuildFolders(string _path)
    {
        //Given a folder path, build all the folders needed for that path to be correct;
    }

    /// <summary>
    /// Creates a key for you, so you don't have to and every game instance can be unique.
    /// </summary>
    public static void CreateEncryptionKey()
    {
        Aes _encryption = Aes.Create();

        encryptionKeyByte = _encryption.Key;
        EncryptionKey = Convert.ToBase64String(encryptionKeyByte);
    }

    #region XML
    /// <summary>
    /// Saves data in XML format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save</param>
    public static void SaveDataXML<T>(in string _path, in T _data)
    {
        XmlSerializer _serializer = new XmlSerializer(typeof(T));

        FileStream _file = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Create);

        _serializer.Serialize(_file, _data);
        _file.Close();
    }

    /// <summary>
    /// Save data in a secure AES XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save</param>
    public static void SaveDataSecureXML<T>(in string _path, in T _data)
    {
        Aes _encryption = Aes.Create();

        byte[] _outputIV = _encryption.IV;

        using (FileStream _dataStream = File.Open($"{Application.persistentDataPath}{_path}", FileMode.OpenOrCreate))
        {
            _dataStream.Write(_outputIV, 0, _outputIV.Length);

            using (CryptoStream _cryptoStream = new CryptoStream(_dataStream, _encryption.CreateEncryptor(encryptionKeyByte, _outputIV), CryptoStreamMode.Write))
            {
                XmlSerializer _serializer = new XmlSerializer(typeof(T));

                _serializer.Serialize(_cryptoStream, _data);
            }
        }
    }

    /// <summary>
    /// Loads an XML file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Output data</param>
    /// <returns>True if successful, false is fail</returns>
    public static bool LoadDataXML<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return false;
        }

        XmlSerializer _serializer = new XmlSerializer(typeof(T));

        using (FileStream _file = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Open))
        {

            _data = (T)_serializer.Deserialize(_file);
        }

        return true;
    }

    /// <summary>
    /// Loads an Secure AES XML file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Output data</param>
    /// <returns>True if successful, false is fail</returns>
    public static bool LoadDataSecureXML<T>(string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return false;
        }

        Aes _encryption = Aes.Create();

        byte[] _outputIV = new byte[_encryption.IV.Length];

        using (FileStream _dataStream = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Open))
        {
            _dataStream.Read(_outputIV, 0, _outputIV.Length);

            using (CryptoStream _cryptoSteam = new CryptoStream(_dataStream, _encryption.CreateDecryptor(encryptionKeyByte, _outputIV), CryptoStreamMode.Read))
            {
                XmlSerializer _serializer = new XmlSerializer(typeof(T));
                _data = (T)_serializer.Deserialize(_cryptoSteam);
            }
        }

        return true;
    }
    #endregion

    #region JSON
    /// <summary>
    /// Saves data in JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save</param>
    public static void SaveDataJSON<T>(in string _path, in T _data)
    {
        string _json = JsonUtility.ToJson(_data);
        SaveDataJSON(_path, _json);
    }

    /// <summary>
    /// Saves data in JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save, in case you wanna make the JSON yourself</param>
    public static void SaveDataJSON(in string _path, in string _data)
    {
        using (StreamWriter _writer = new StreamWriter($"{Application.persistentDataPath}{_path}"))
        {
            _writer.Write(_data);
        }
    }

    /// <summary>
    /// Saves data in a Secure AES JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save</param>
    public static void SaveDataSecureJSON<T>(in string _path, in T _data)
    {
        string _json = JsonUtility.ToJson(_data);
        SaveDataSecureJSON(_path, _json);
    }

    /// <summary>
    /// Saves data in a Secure AES JSON format
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Thing to save, in case you wanna make the JSON yourself</param>
    public static void SaveDataSecureJSON(in string _path, in string _data)
    {
        Aes _encryption = Aes.Create();

        byte[] _outputIV = _encryption.IV;

        using (FileStream _dataStream = File.Open($"{Application.persistentDataPath}{_path}", FileMode.OpenOrCreate))
        {
            _dataStream.Write(_outputIV, 0, _outputIV.Length);

            using (CryptoStream _cryptoStream = new CryptoStream(_dataStream, _encryption.CreateEncryptor(encryptionKeyByte, _outputIV), CryptoStreamMode.Write))
            {
                using (StreamWriter _writer = new StreamWriter(_cryptoStream))
                {
                    _writer.Write(_data);
                }
            }
        }
    }

    /// <summary>
    /// Loads a file from JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Output data</param>
    /// <returns>True if sucessful, false if fail</returns>
    public static bool LoadDataJSON<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return false;
        }

        using (StreamReader _reader = new StreamReader($"{Application.persistentDataPath}{_path}"))
        {
            string _saveData = _reader.ReadToEnd();
            _data = JsonUtility.FromJson<T>(_saveData);
        }

        return true;
    }

    /// <summary>
    /// Loads a file from a Secure AES JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path">Save path. (Does persistentDataPath+path in function)</param>
    /// <param name="_data">Output data</param>
    /// <returns>True if sucessful, false if fail</returns>
    public static bool LoadDataSecureJSON<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return false;
        }

        Aes _encryption = Aes.Create();

        byte[] _outputIV = new byte[_encryption.IV.Length];

        using (FileStream _dataStream = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Open))
        {
            _dataStream.Read(_outputIV, 0, _outputIV.Length);

            using (CryptoStream _cryptoSteam = new CryptoStream(_dataStream, _encryption.CreateDecryptor(encryptionKeyByte, _outputIV), CryptoStreamMode.Read))
            {
                using (StreamReader _reader = new StreamReader(_cryptoSteam))
                {
                    _data = JsonUtility.FromJson<T>(_reader.ReadToEnd());
                }
            }
        }

        return true;
    }
    #endregion
}
