using System.IO;
using System.Xml;
using System.Windows;
using System.Xml.Serialization;

namespace Hidder.Models
{
    /// <summary>
    /// xmlの読み書きを行うクラス。
    /// </summary>
    public static class XmlFileSerializer
    {
        public static void Serialize<T>(T T1, string path)
        {
            XmlSerializer serializer = new XmlSerializer(T1.GetType());

            XmlWriterSettings writeSettings = new XmlWriterSettings();
            writeSettings.OmitXmlDeclaration = true;
            writeSettings.Indent = true;

            using (var writer = XmlWriter.Create(path, writeSettings))
            {
                serializer.Serialize(writer,T1);
            }
        }

        /// <summary>
        /// xmlをデシリアライズします。
        /// </summary>
        /// <typeparam name="T">クラス</typeparam>
        /// <param name="path">xmlファイルのパス</param>
        /// <returns>xmlをデシリアライズした結果</returns>
        public static T Deserialize<T>(T T1, string path)
            where T : class
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            using (var fs = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(T1.GetType());
                T obj = serializer.Deserialize(fs) as T;
                return obj;
            }
        }
    }
}
