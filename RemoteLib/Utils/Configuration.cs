using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemoteLib.Utils
{
    public class Configuration
    {
        /// <summary>
        /// Gets or sets the blobs.
        /// </summary>
        /// <value>
        /// The blobs.
        /// </value>
        public List<ConfigBlob> Blobs { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[object key]
        {
            get
            {
                return key == null ? null : (from t in Blobs where t.Key.Equals(key) select t.Value).FirstOrDefault();
            }
            set
            {
                if (this[key] == null && !KeyExists(key))
                {
                    Blobs.Add(new ConfigBlob(key, value));
                }
                else
                {
                    foreach (ConfigBlob blob in Blobs.Where(t => t.Key.Equals(key)))
                        blob.Value = value;
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

                if (list == null) return;
                Blobs.Clear();
                Blobs.AddRange(list);
            }
        }

        public bool KeyExists(object key)
        {
            return Blobs.Any(blob => blob.Key.Equals(key));
        }
    }

    public class ConfigBlob
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public object Key { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigBlob"/> class.
        /// </summary>
        public ConfigBlob()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigBlob"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public ConfigBlob(object key, object value)
        {
            Value = value;
            Key = key;
        }

        /// <summary>
        /// Returns the value of the blob as a struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AsStructure<T>() where T : struct
        {
            if (Value == null) return default(T);
            if (Value is T)
            {
                return (T)Value;
            }
            return default(T);
        }
    }
}
