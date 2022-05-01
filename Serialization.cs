using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MuonhoryoLibrary.Serialization
{
    /// <summary>
    /// Provides serialization's sample.
    /// </summary>
    public interface ISerializator
    {
        public string Serialize<T>(T obj);
        public T Deserialize<T>(string serializedObj);
    }
    /// <summary>
    /// Provides serialization's sample.
    /// </summary>
    public interface ISerializator<T>
    {
        public string Serialize(T obj);
        public T Deserialize(string serializedObj);
    }
    public static class Serialization
    {
        /// <summary>
        /// Inserted in end of every element of serialized collection.
        /// </summary>
        internal const string EndText = "\"End\":\"\"";

        [Serializable]
        public struct Pair<T1, T2>
        {
            public Pair(T1 first, T2 second)
            {
                this.first = first;
                this.second = second;
            }
            public Pair(KeyValuePair<T1, T2> keyValuePair)
            {
                first = keyValuePair.Key;
                second = keyValuePair.Value;
            }
            public T1 first;
            public T2 second;
        }
    }
    /// <summary>
    /// ISerializator worked with Serialization.Pair.
    /// </summary>
    public static class DictionarySerializator
    {
        private static string Serialize<TKey,TValue>(Dictionary<TKey,TValue> dictionary,
           Func<Serialization.Pair<TKey, TValue>,string> serializeAction)
        {
            StringBuilder serializedDict = new StringBuilder("{\n\"KeyValuePairList\":\t[\n");
            if (dictionary.Count != 0)
            {
                Serialization.Pair<TKey, TValue>[] pairs = new Serialization.Pair<TKey, TValue>[dictionary.Count];
                {
                    int i = 0;
                    foreach (KeyValuePair<TKey, TValue> item in dictionary)
                    {
                        pairs[i++] = new Serialization.Pair<TKey, TValue>(item);
                    }
                }
                for (int i = 0; i < pairs.Length; i++)
                {
                    StringBuilder serializedItem = new StringBuilder(serializeAction(pairs[i]));
                    serializedItem = serializedItem.Insert(serializedItem.Length - 1, Serialization.EndText);
                    serializedDict.Append(serializedItem);
                    if (i < pairs.Length - 1)
                    {
                        serializedDict.Append(",\n\t\t");
                    }
                }
            }
            serializedDict.Append("\n]\n}");
            return serializedDict.ToString();
        }
        public static string Serialize<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
            ISerializator serializator)
        {
            return Serialize(dictionary, serializator.Serialize);
        }
        public static string Serialize<TKey,TValue>(Dictionary<TKey,TValue>dictionary,
            ISerializator<Serialization.Pair<TKey, TValue>> serializator)
        {
            return Serialize(dictionary, serializator.Serialize);
        }

        private static void Write<TKey,TValue>(string path,Dictionary<TKey,TValue>dictionary,
            Func<Serialization.Pair<TKey, TValue>, string> serializeAction)
        {
            using StreamWriter stream = new StreamWriter(path, false);
            stream.Write(Serialize(dictionary, serializeAction));
            stream.Close();
        }
        /// <summary>
        /// Serialize dictionary and write(with overwritting) in file on the path.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="dictionary"></param>
        /// <param name="serializator"></param>
        public static void Write<TKey, TValue>(string path, Dictionary<TKey, TValue> dictionary,
            ISerializator serializator)
        {
            Write(path, dictionary, serializator.Serialize);
        }
        public static void Write<TKey,TValue>(string path,Dictionary<TKey,TValue> dictionary,
            ISerializator<Serialization.Pair<TKey, TValue>> serializator)
        {
            Write(path,dictionary,serializator.Serialize);
        }

        private static void WriteOrCreateNewFile<TKey,TValue>(string path,Dictionary<TKey,TValue>dictionary,
            Func<Serialization.Pair<TKey,TValue>,string> serializeAction)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            Write(path, dictionary, serializeAction);
        }
        public static void WriteOrCreateNewFile<TKey, TValue>(string path, Dictionary<TKey, TValue> dictionary,
            ISerializator serializator)
        {
            WriteOrCreateNewFile(path, dictionary, serializator.Serialize);
        }
        public static void WriteOrCreateNewFile<TKey,TValue>(string path,Dictionary<TKey,TValue> dictionary,
            ISerializator<Serialization.Pair<TKey,TValue>> serializator)
        {
            WriteOrCreateNewFile(path, dictionary, serializator.Serialize);
        }



        private static Dictionary<TKey,TValue> Deserialize<TKey,TValue>(string serializedDictionary,
            Func<string,Serialization.Pair<TKey,TValue>> deserializeAction)
        {
            Dictionary<TKey, TValue> deserializedDictionary = new Dictionary<TKey, TValue> { };
            int start = serializedDictionary.IndexOf("[");
            while (true)
            {
                start = serializedDictionary.IndexOf("{", start + 1);
                if (start == -1)
                {
                    break;
                }
                var keyValuePair =deserializeAction(serializedDictionary.Substring
                    (start, serializedDictionary.IndexOf(Serialization.EndText, start)) + "}");
                deserializedDictionary.Add(new KeyValuePair<TKey, TValue>(keyValuePair.first, keyValuePair.second));
            }
            return deserializedDictionary;
        }
        public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(string serializedDictionary,
            ISerializator serializator)
        {
            return Deserialize(serializedDictionary,
                serializator.Deserialize<Serialization.Pair<TKey,TValue>>);
        }
        public static Dictionary<TKey,TValue>Deserialize<TKey,TValue>(string serializedDictionary,
            ISerializator<Serialization.Pair<TKey,TValue>> serializator)
        {
            return Deserialize(serializedDictionary, serializator.Deserialize);
        }

        private static Dictionary<TKey,TValue> Read<TKey,TValue>(string path,
            Func<string,Serialization.Pair<TKey,TValue>> deserializeAction)
        {
            Dictionary<TKey, TValue> deserializedDictionary = new Dictionary<TKey, TValue> { };
            bool arrayIsOpen = false;
            int start = -1;
            int symbolCount = 0;
            var diapasons = new LinkedList<Serialization.Pair<int, int>> { };
            foreach (string line in File.ReadLines(path))
            {
                if (arrayIsOpen)
                {
                    int i;
                    if (start == -1)
                    {
                        i = line.IndexOf("{");
                        if (i != -1)
                        {
                            start = i + symbolCount;
                        }
                    }
                    else
                    {
                        i = line.IndexOf(Serialization.EndText);
                        if (i != -1)
                        {
                            i += symbolCount;
                            diapasons.AddLast(new Serialization.Pair<int, int>(start, i));
                            start = -1;
                        }
                    }
                }
                else
                {
                    if (line.Contains("["))
                    {
                        arrayIsOpen = true;
                    }
                }
                symbolCount += line.Length + 1;
            }
            Encoding encoding;
            using (StreamReader str = new StreamReader(path))
            {
                encoding = str.CurrentEncoding;
                str.Close();
            }
            using FileStream stream = new FileStream(path, FileMode.Open);
            foreach (Serialization.Pair<int, int> item in diapasons)
            {
                byte[] array = new byte[item.second - item.first];
                stream.Seek(item.first, SeekOrigin.Begin);
                stream.Read(array, 0, array.Length);
                var keyValuePair =deserializeAction(encoding.GetString(array) + "}");
                if (!deserializedDictionary.ContainsKey(keyValuePair.first))
                {
                    deserializedDictionary.Add(new KeyValuePair<TKey, TValue>
                        (keyValuePair.first, keyValuePair.second));
                }
            }
            stream.Close();
            return deserializedDictionary;
        }
        /// <summary>
        /// Return deserialized dictionary from file on the path.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="serializator"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Read<TKey, TValue>(string path, ISerializator serializator)
        {
            return Read(path, serializator.Deserialize<Serialization.Pair<TKey,TValue>>);
        }
        public static Dictionary<TKey,TValue> Read<TKey,TValue>(string path,
            ISerializator<Serialization.Pair<TKey,TValue>> serializator)
        {
            return Read(path, serializator.Deserialize);
        }

        private static Dictionary<TKey,TValue> ReadOrCreateNewFile<TKey,TValue>(string path,
            Func<string,Serialization.Pair<TKey,TValue>> deserializeAction)
        {
            Dictionary<TKey, TValue> deserializedDictionary = new Dictionary<TKey, TValue> { };
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            else
            {
                deserializedDictionary = Read(path, deserializeAction);
            }
            return deserializedDictionary;
        }
        public static Dictionary<TKey, TValue> ReadOrCreateNewFile<TKey, TValue>(string path,
            ISerializator serializator)
        {
            return ReadOrCreateNewFile(path, serializator.Deserialize<Serialization.Pair<TKey,TValue>>);
        }
        public static Dictionary<TKey,TValue> ReadOrCreateNewFile<TKey,TValue>(string path,
            ISerializator<Serialization.Pair<TKey,TValue>> serializator)
        {
            return ReadOrCreateNewFile(path, serializator.Deserialize);
        }
    }
    /// <summary>
    /// ISerializator worked with KeyValuePair and should serialize in one string
    /// without symbol of endline.
    /// </summary>
    public static class DictionaryCompactSerializator
    {
        private static string Serialize<TKey,TValue>(Dictionary<TKey,TValue> dictionary,
            Func<KeyValuePair<TKey,TValue>,string> serializeAction)
        {
            StringBuilder str = new StringBuilder();
            foreach (var pair in dictionary)
            {
                str.AppendLine(serializeAction(pair));
            }
            return str.ToString();
        }
        public static string Serialize<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
            ISerializator serializator)
        {
            return Serialize(dictionary, serializator.Serialize);
        }
        public static string Serialize<TKey,TValue>(Dictionary<TKey,TValue> dictionary,
            ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            return Serialize(dictionary, serializator.Serialize);
        }

        private static void Write<TKey,TValue>(string path,Dictionary<TKey,TValue>dictionary,
            Func<KeyValuePair<TKey,TValue>,string> serializeAction)
        {
            using StreamWriter writer = new StreamWriter(path, false);
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                writer.WriteLine(serializeAction(pair));
            }
            writer.Close();
        }
        /// <summary>
        /// Write dictionary in file at path. Serializes each dictionary's element separately.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="dictionary"></param>
        /// <param name="serializator"></param>
        public static void Write<TKey, TValue>(string path, Dictionary<TKey, TValue> dictionary,
            ISerializator serializator)
        {
            Write(path,dictionary,serializator.Serialize);
        }
        /// <summary>
        /// Write dictionary in file at path. Serializes each dictionary's element separately.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="dictionary"></param>
        /// <param name="serializator"></param>
        public static void Write<TKey,TValue>(string path,Dictionary<TKey,TValue>dictionary,
            ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            Write(path, dictionary, serializator.Serialize);
        }

        private static void WriteOrCreateNewFile<TKey,TValue>(string path,Dictionary<TKey,TValue> dictionary,
            Func<KeyValuePair<TKey,TValue>,string> serializeAction)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            Write(path, dictionary,serializeAction);
        }
        public static void WriteOrCreateNewFile<TKey, TValue>(string path,
            Dictionary<TKey, TValue> dictionary, ISerializator serializator)
        {
            WriteOrCreateNewFile(path, dictionary, serializator.Serialize);
        }
        public static void WriteOrCreateNewFile<TKey,TValue>(string path,
            Dictionary<TKey,TValue> dictionary,ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            WriteOrCreateNewFile(path, dictionary, serializator.Serialize);
        }



        private static Dictionary<TKey,TValue> Deserialize<TKey,TValue>(string serializedDictionary,
            Func<string,KeyValuePair<TKey,TValue>> deserializeAction)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue> { };
            int start = 0;
            int index = serializedDictionary.IndexOf("\n");
            while (index != -1)
            {
                dict.Add(deserializeAction(serializedDictionary.Substring(start, index - start)));
                start = index + 1;
                index = serializedDictionary.IndexOf("\n", start);
            }
            return dict;
        }
        public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(string serializedDictionary, 
            ISerializator serializator)
        {
            return Deserialize(serializedDictionary, serializator.Deserialize<KeyValuePair<TKey,TValue>>);
        }
        public static Dictionary<TKey,TValue> Deserialize<TKey,TValue>(string serializedDictionary,
            ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            return Deserialize(serializedDictionary, serializator.Deserialize);
        }

        private static Dictionary<TKey,TValue> Read<TKey,TValue>(string path,
            Func<string,KeyValuePair<TKey,TValue>> deserializeAction)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue> { };
            foreach (string line in File.ReadLines(path))
            {
                dict.Add(deserializeAction(line));
            }
            return dict;
        }
        /// <summary>
        /// Read file at path. Deserializes line by line.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="serializator"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Read<TKey, TValue>(string path, ISerializator serializator)
        {
            return Read(path, serializator.Deserialize<KeyValuePair<TKey,TValue>>);
        }
        /// <summary>
        /// Read file at path. Deserializes line by line.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="serializator"></param>
        /// <returns></returns>
        public static Dictionary<TKey,TValue>Read<TKey,TValue>(string path,
            ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            return Read(path, serializator.Deserialize);
        }

        private static Dictionary<TKey,TValue> ReadOrCreateNewFile<TKey,TValue>(string path,
            Func<string,KeyValuePair<TKey,TValue>> deserializeAction)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            return Read(path, deserializeAction);
        }
        public static Dictionary<TKey, TValue> ReadOrCreateNewFile<TKey, TValue>(string path,
            ISerializator serializator)
        {
            return ReadOrCreateNewFile(path, serializator.Deserialize<KeyValuePair<TKey, TValue>>);
        }
        public static Dictionary<TKey,TValue> ReadOrCreateNewFile<TKey,TValue>(string path,
            ISerializator<KeyValuePair<TKey,TValue>> serializator)
        {
            return ReadOrCreateNewFile(path, serializator.Deserialize);
        }

        private static KeyValuePair<TKey,TValue>ReadOne<TKey,TValue>(string path,
            Func<string,KeyValuePair<TKey,TValue>> deserializeAction,TKey key)
        {
            string keyText = key.ToString();
            foreach (string line in File.ReadLines(path))
            {
                if (line.Contains(keyText))
                {
                    return deserializeAction(line);
                }
            }
            return new KeyValuePair<TKey, TValue>();
        }
        /// <summary>
        /// Find pair with key and deserialize it. 
        /// Return default KeyValuePair if file not contain key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="serializator"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey, TValue> ReadOne<TKey, TValue>
            (string path, ISerializator serializator, TKey key)
        {
            return ReadOne(path, serializator.Deserialize<KeyValuePair<TKey, TValue>>, key);
        }
        /// <summary>
        /// Find pair with key and deserialize it. 
        /// Return default KeyValuePair if file not contain key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="serializator"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyValuePair<TKey,TValue>ReadOne<TKey,TValue>(string path,
            ISerializator<KeyValuePair<TKey,TValue>>serializator,TKey key)
        {
            return ReadOne(path, serializator.Deserialize, key);
        }

        private static KeyValuePair<TKey,TValue> ReadOneOrCreateNewFile<TKey,TValue>(string path,
            Func<string,KeyValuePair<TKey,TValue>> deserializeAction,TKey key)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            return ReadOne(path, deserializeAction, key);
        }
        public static KeyValuePair<TKey, TValue> ReadOneOrCreateNewFile<TKey, TValue>
            (string path,ISerializator serializator, TKey key)
        {
            return ReadOneOrCreateNewFile(path, serializator.Deserialize<KeyValuePair<TKey,TValue>>, key);
        }
        public static KeyValuePair<TKey,TValue>ReadOneOrCreateNewFile<TKey,TValue>
            (string path, ISerializator<KeyValuePair<TKey,TValue>> serializator,TKey key)
        {
            return ReadOneOrCreateNewFile(path, serializator.Deserialize, key);
        }
    }
}
