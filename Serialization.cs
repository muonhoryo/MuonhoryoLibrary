using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MuonhoryoLibrary.Exceptions;

namespace MuonhoryoLibrary.Serialization
{
    /// <summary>
    /// Used in list's serializations.
    /// </summary>
    public interface ISerializator
    {
        public string Serialize<T>(T obj);
        public T Deserialize<T>(string json);
    }
    public static class Serialization
    {
        /// <summary>
        /// Inserted in end of every element of serialized collection.
        /// </summary>
        private const string EndText = "\"End\":\"\"";

        [Serializable]
        private struct Pair<T1, T2>
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
        public static class DictionarySerializator
        {
            public static string Serialize<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
                ISerializator serializator)
            {
                StringBuilder serializedDict = new StringBuilder("{\n\"KeyValuePairList\":\t[\n");
                if (dictionary.Count != 0)
                {
                    Pair<TKey, TValue>[] pairs = new Pair<TKey, TValue>[dictionary.Count];
                    {
                        int i = 0;
                        foreach (KeyValuePair<TKey, TValue> item in dictionary)
                        {
                            pairs[i++] = new Pair<TKey, TValue>(item);
                        }
                    }
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        StringBuilder serializedItem = new StringBuilder(serializator.Serialize(pairs[i]));
                        serializedItem = serializedItem.Insert(serializedItem.Length - 1, EndText);
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
                using StreamWriter stream = new StreamWriter(path, false);
                stream.Write(Serialize(dictionary,serializator));
                stream.Close();
            }

            public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(string dictionary,
                ISerializator serializator)
            {
                Dictionary<TKey, TValue> deserializedDictionary = new Dictionary<TKey, TValue> { };
                int start = dictionary.IndexOf("[");
                while (true)
                {
                    start = dictionary.IndexOf("{", start + 1);
                    if (start == -1)
                    {
                        break;
                    }
                    var keyValuePair = serializator.Deserialize<Pair<TKey, TValue>>
                        (dictionary.Substring(start, dictionary.IndexOf(EndText, start)) + "}");
                    deserializedDictionary.Add(new KeyValuePair<TKey, TValue>(keyValuePair.first, keyValuePair.second));
                }
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
                Dictionary<TKey, TValue> deserializedDictionary = new Dictionary<TKey, TValue> { };
                if (!File.Exists(path))
                {
                    using FileStream stream = File.Create(path);
                    stream.Close();
                }
                else
                {
                    bool arrayIsOpen = false;
                    int start = -1;
                    int symbolCount = 0;
                    List<Pair<int, int>> diapasons = new List<Pair<int, int>> { };
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
                                i = line.IndexOf(EndText);
                                if (i != -1)
                                {
                                    i += symbolCount;
                                    diapasons.Add(new Pair<int, int>(start, i));
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
                    using(StreamReader str=new StreamReader(path))
                    {
                        encoding = str.CurrentEncoding;
                        str.Close();
                    }
                    using FileStream stream = new FileStream(path, FileMode.Open);
                    foreach (Pair<int, int> item in diapasons)
                    {
                        byte[] array = new byte[item.second - item.first];
                        stream.Seek(item.first, SeekOrigin.Begin);
                        stream.Read(array, 0, array.Length);
                        var keyValuePair = serializator.Deserialize<Pair<TKey, TValue>>
                            (encoding.GetString(array)+"}");
                        if (!deserializedDictionary.ContainsKey(keyValuePair.first))
                        {
                            deserializedDictionary.Add(new KeyValuePair<TKey, TValue>(keyValuePair.first, keyValuePair.second));
                        }
                    }
                    stream.Close();
                }
                return deserializedDictionary;
            }
        }
    }
}
