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
    public static string[] EncryptionKey { private get; set; }

    private static byte[] EncryptionKeyByte;

    //
    public static bool DoesFileExist(in string _path)
    {
        return File.Exists($"{Application.persistentDataPath}{_path}");
    }

    public static void BuildFolders(string _path)
    {
        //Given a folder path, build all the folders needed for that path to be correct;
    }

    #region XML
    public static void SaveDataXML<T>(in string _path, in T _data)
    {
        XmlSerializer _serializer = new XmlSerializer(typeof(T));

        FileStream _file = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Create);

        _serializer.Serialize(_file, _data);
        _file.Close();
    }

    public static void SaveDataSecureXML<T>(in string _path, in T _data)
    {

    }

    public static void LoadDataXML<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return;
        }

        XmlSerializer _serializer = new XmlSerializer(typeof(T));

        FileStream _file = File.Open($"{Application.persistentDataPath}{_path}", FileMode.Open);

        _data = (T)_serializer.Deserialize(_file);
        _file.Close();
    }

    public static void LoadDataSecureXML<T>(string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return;
        }

        _data = default;
    }
    #endregion

    #region JSON
    public static void SaveDataJSON<T>(in string _path, in T _data)
    {
        string _json = JsonUtility.ToJson(_data);
        SaveDataJSON(_path, _json);
    }

    public static void SaveDataJSON(in string _path, in string _data)
    {
        using (StreamWriter _writer = new StreamWriter(_path))
        {
            _writer.Write(_data);
        }
    }

    public static void SaveDataSecureJSON<T>(in string _path, in T _data)
    {
        string _json = JsonUtility.ToJson(_data);
        SaveDataSecureJSON(_path, _json);
    }

    public static void SaveDataSecureJSON(in string _path, in string _data)
    {
        Aes _encryption = Aes.Create();

        EncryptionKeyByte = _encryption.Key;

        byte[] _outputIV = _encryption.IV;

        FileStream _dataStream = File.Open(_path, FileMode.Open);
        _dataStream.Write(_outputIV, 0, _outputIV.Length);

        CryptoStream _cryptoStream = new CryptoStream(_dataStream, _encryption.CreateEncryptor(_encryption.Key, _encryption.IV), CryptoStreamMode.Write);


        StreamWriter _writer = new StreamWriter(_cryptoStream);

        _writer.Write(_data);

        _writer.Close();
        _cryptoStream.Close();
        _dataStream.Close();
    }

    public static void LoadDataJSON<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return;
        }

        using (StreamReader _reader = new StreamReader(_path))
        {
            string _saveData = _reader.ReadToEnd();
            _data = JsonUtility.FromJson<T>(_saveData);
        }
    }

    public static void LoadDataSecureJSON<T>(in string _path, out T _data)
    {
        if (!DoesFileExist(_path))
        {
            _data = default;
            return;
        }

        FileStream _dataStream = File.Open(_path, FileMode.Open);

        Aes _encryption = Aes.Create();

        byte[] _outputIV = new byte[_encryption.IV.Length];

        _dataStream.Read(_outputIV, 0, _outputIV.Length);

        CryptoStream _cryptoSteam = new CryptoStream(_dataStream, _encryption.CreateDecryptor(EncryptionKeyByte, _encryption.IV), CryptoStreamMode.Read);

        StreamReader _reader = new StreamReader(_cryptoSteam);

        string _saveData = _reader.ReadToEnd();
        _data = JsonUtility.FromJson<T>(_saveData);

        _reader.Close();
        _cryptoSteam.Close();
        _dataStream.Close();

        //_data = default;
    }
    #endregion
}
