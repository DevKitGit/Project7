using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Modularis.Editor.Helpers
{
    public static class ObjectExtension
    {
        /*public static T CopyObjectUsingBf<T>(this object objSource)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, objSource);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }

        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings
                { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }*/

        /*public static T CloneUsingMessagePack<T>(this T source)
        {
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                MessagePack.Unity.UnityResolver.Instance,
                StandardResolver.Instance);
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            MessagePackSerializer.DefaultOptions = options;

            var typeless = MessagePackSerializer.Serialize(source);
            return MessagePackSerializer.Deserialize<T>(typeless.to);
        }*/
        public static void CopyTo(this SerializedProperty origin, SerializedProperty destination)
        {
            /*if(origin.serializedObject.targetObject.GetType() == destination.serializedObject.targetObject.GetType())
            {
                EditorUtility.CopySerialized(origin.serializedObject.targetObject, destination.serializedObject.targetObject);
                return;
            }*/
            var dest = destination.serializedObject;
            SerializedProperty propIterator = origin.serializedObject.GetIterator();
            //jump into serialized object, this will skip script type so that we dont override the destination component's type
            if (propIterator.NextVisible(true))
            {
                while (propIterator.NextVisible(true)) //iterate through all serializedProperties
                {
                    //try obtaining the property in destination component
                    using var propElement = dest.FindProperty(propIterator.name);
                    if (propElement != null && propElement.propertyType == propIterator.propertyType) 
                    {
                        //copy value from source to destination component
                        dest.CopyFromSerializedProperty(propIterator); 
                    }
                }
            }
            dest.ApplyModifiedProperties();
        }

        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true ) 
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) && 
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}