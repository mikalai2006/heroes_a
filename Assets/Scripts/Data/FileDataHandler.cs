using System;
using System.IO;

using UnityEngine;

public class FileDataHandler
{
    private readonly string _dataDirPath;

    private readonly string _dataFileName;

    private readonly string _mapFileName;

    private readonly bool _useEncryption = false;

    private readonly string _encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string dataFileName, string mapFileName, bool useEncryption)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
        this._useEncryption = useEncryption;
        this._mapFileName = mapFileName;
    }

    public DataPlay LoadDataPlay()
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);

        DataPlay loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<DataPlay>(dataToLoad);

                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Error Load file::: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void SaveDataPlay(DataPlay data)
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Save file::: " + fullPath + "\n" + e);
        }
    }

    public DataGame LoadDataGame()
    {
        string fullPath = Path.Combine(_dataDirPath, _mapFileName);

        DataGame loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<DataGame>(dataToLoad);

                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Error Load file::: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void SaveDataGame(DataGame data)
    {
        string fullPath = Path.Combine(_dataDirPath, _mapFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Save file::: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifierData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifierData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);
        }

        return modifierData;
    }
}
