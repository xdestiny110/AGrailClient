using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Framework.AssetBundle
{
    public class CheckFile
    {
        public enum Location
        {
            Local = 0,
            Persistent,
            Remote
        }

        public string name;
        public string hash;
        [JsonConverter(typeof(StringEnumConverter))]
        public Location location = Location.Local;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if ((obj.GetType().Equals(GetType())) == false) return false;
            var o = obj as CheckFile;
            return o.name == name && o.hash == hash;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() + hash.GetHashCode();
        }
    }
}