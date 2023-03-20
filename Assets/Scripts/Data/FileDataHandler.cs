using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath;

    private string dataFileName;

    private string mapFileName;

    private bool useEncryption = false;

    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string dataFileName, string mapFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
        this.mapFileName = mapFileName;
    }

    public DataPlay LoadDataPlay()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

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

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            } catch(Exception e) {
                Debug.Log("Error Load file::: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void SaveDataPlay(DataPlay data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            if (useEncryption)
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
        catch(Exception e) {
            Debug.Log("Error Save file::: " + fullPath + "\n" + e);
        }
    }

    public DataGame LoadDataGame()
    {
        string fullPath = Path.Combine(dataDirPath, mapFileName);

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

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

            }
            catch (Exception e)
            {
                Debug.Log("Error Load file::: " + fullPath + "\n" + e);
            }
        }

        return loadedData;

    }

    public void SaveDataGame(DataGame data)
    {
        string fullPath = Path.Combine(dataDirPath, mapFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            if (useEncryption)
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
            Debug.Log("Error Save file::: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifierData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifierData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifierData;
    }
}
