using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemoteLib.Utils
{
    public class Configuration
    {
        public List<ConfigBlob> Blobs { get; set; }

        public string FilePath { get; set; }

        public ConfigBlob this[object key]
        {
            get
            {
                if (key == null)
                    return new ConfigBlob();
                for (int i = 0; i < Blobs.Count; i++)
                    if (Blobs[i].Key.Equals(key))
                        return Blobs[i];
                return new ConfigBlob(key, null);
            }
            set
            {
                if (this[key].Value == null && !KeyExists(key))
                {
                    Blobs.Add(value);
                }
                else
                {
                    this[key].Value = value.Value;
                }
            }
        }

        public Configuration(string file)
        {
            Blobs = new List<ConfigBlob>();
            FilePath = file;
            Reload();
        }

        public void Save()
        {
            if (FilePath == null) return;

            var lines = Newtonsoft.Json.JsonConvert.SerializeObject(Blobs, Newtonsoft.Json.Formatting.Indented);
            
            if (!File.Exists(FilePath))
            {
                using (var notused = File.CreateText(FilePath)) { }
            }

            File.WriteAllText(FilePath, lines);
        }

        public void Reload()
        {
            if(FilePath == null) return;
            if (!File.Exists(FilePath))
            {
                using (var notused = File.CreateText(FilePath)) { }
            }

            using (var reader = File.OpenText(FilePath))
            {
                string read = reader.ReadToEnd();
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigBlob>>(read);

                if (list != null)
                {
                    Blobs.Clear();
                    Blobs.AddRange(list);
                }
            }
        }

        public bool KeyExists(object key)
        {
            return Blobs.Any(blob => blob.Key.Equals(key));
        }


    }

    public class ConfigBlob
    {
        public object Value { get; set; }
        public object Key { get; set; }

        public ConfigBlob()
        {
        }

        public ConfigBlob(object key, object value)
        {
            Value = value;
            Key = key;
        }

        public T AsPrimitive<T>() where T : struct
        {
            if (Value == null) return default(T);
            if (Value is T)
            {
                return (T)Value;
            }
            else
            {
                return default(T);
            }
        }
    }
}
