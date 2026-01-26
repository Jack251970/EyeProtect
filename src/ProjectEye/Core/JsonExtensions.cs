using System;
using System.IO;
using Newtonsoft.Json;

namespace ProjectEye.Core
{
    public class JsonExtensions
    {
        private readonly string file;
        
        public JsonExtensions(string file)
        {
            this.file = file;
        }
        
        public object ToModel(Type type)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return null;
                }
                
                var json = File.ReadAllText(file);
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                };
                return JsonConvert.DeserializeObject(json, type, settings);
            }
            catch (Exception e)
            {
                LogHelper.Warning(e.ToString());
                return null;
            }
        }
        
        public bool Save(object data)
        {
            try
            {
                var dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                };
                
                var json = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(file, json);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Warning(e.ToString());
                return false;
            }
        }
    }
}
