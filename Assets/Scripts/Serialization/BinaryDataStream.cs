using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Serialization {
    public static class BinaryDataStream {
        private static readonly string Path = Application.persistentDataPath + "/saves/";
        private const string Ext = ".dat";

        public static void Save<T>(T data, string fileName) {
            Directory.CreateDirectory(Path);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GetFullPath(fileName), FileMode.Create);

            try {
                formatter.Serialize(stream, data);
            } catch (SerializationException e) {
                Debug.Log("Save failed. Error: " + e.Message);
            } finally {
                stream.Close();
            }
        }

        private static string GetFullPath(string fileName) {
            return Path + fileName + Ext;
        }

        public static bool Exists(string fileName) {
            return File.Exists(GetFullPath(fileName));
        }

        public static T Read<T>(string fileName) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GetFullPath(fileName), FileMode.Open);
            T returnType = default(T);

            try {
                returnType = (T)formatter.Deserialize(stream);
            } catch (SerializationException e) {
                Debug.Log("Read failed. Error: " + e.Message);
            } finally {
                stream.Close();
            }

            return returnType;
        }
    }
}
