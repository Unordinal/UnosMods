using System;
using System.IO;
using System.Text;

namespace PrefabInfoExporter.UnityResources
{
    public class ResourceFile
    {
        public string FilePath { get; }
        public UnityEngine.Object UnityObject { get; }

        public ResourceFile(string resourcePath, UnityEngine.Object unityObject)
        {
            UnityObject = unityObject;
            FilePath = Path.Combine(resourcePath, unityObject.name);
        }

        public string GetInfo()
        {
            StringBuilder objInfo = new StringBuilder();

            Type type = UnityObject.GetType();
            objInfo.AppendLine($"[{type.FullName}]");
            objInfo.AppendLine(Util.CreateCategory("Fields", GetFields(type)));

            return objInfo.ToString();
        }

        private string GetFields(Type type)
        {
            StringBuilder fields = new StringBuilder();

            foreach (var fieldInfo in type.GetFields())
            {
                var fieldVal = fieldInfo.GetValue(UnityObject).ToString();
                fields.AppendLine($"{fieldInfo.Name} = {(string.IsNullOrWhiteSpace(fieldVal) ? "<empty>" : fieldVal)}");
            }

            return fields.ToString();
        }
    }
}
