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
                    return null;
                for (int i = 0; i < Blobs.Count; i++)
                    if (Blobs[i].Key == key)
                        return Blobs[i];
                return null;
            }
            set
            {
                if (this[key] == null)
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
                Blobs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigBlob>>(read);
            }
        }


    }

    public class ConfigBlob
    {
        public object Value { get; set; }
        public object Key { get; set; }
    }
}
