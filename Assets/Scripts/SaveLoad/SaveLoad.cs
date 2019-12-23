using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad : MonoBehaviour {

    private static string basePath = Application.persistentDataPath + "/saves/";
    private static string fileExtension = ".sav";

    public static void Save<T>(T objectToSave, string key) {
        Directory.CreateDirectory(basePath);
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream fileStream = new FileStream(GenerateSavePath(key), FileMode.Create)) {
            formatter.Serialize(fileStream, objectToSave);
        }
    }

    public static T Load<T>(string key) {
        BinaryFormatter formatter = new BinaryFormatter();
        T loadedData = default(T);

        using (FileStream fileStream = new FileStream(GenerateSavePath(key), FileMode.Open)) {
            loadedData = (T)formatter.Deserialize(fileStream);
        }

        return loadedData;
    }

    public static bool SaveExistsAt(string key) {
        return File.Exists(GenerateSavePath(key));
    }

    public static void DeleteAllSaves() {
        DirectoryInfo dir = new DirectoryInfo(basePath);
        dir.Delete();
        Directory.CreateDirectory(basePath);
    }

    private static string GenerateSavePath(string key) {
        return basePath + key + fileExtension;
    }
}
