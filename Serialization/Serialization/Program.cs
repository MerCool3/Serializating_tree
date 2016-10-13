using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            IntHolder temp = new IntHolder(6, new IntHolder(134, new DoubleHolder(5.31, new CharArrayHolder("Mercool")),  new DoubleHolder(3.14, new IntHolder(34))));
            var attrOverrides = PrepareOverriddenAttributes();
            SaveAsBinaryFile(temp, "D:/tree.dat");

            LoadFromBinaryFile("D:/tree.dat");

            SaveAsSoapFile(temp, "D:/tree.soap");

            LoadFromSoapFile("D:/tree.soap");

            SaveAsXmlFile(temp, attrOverrides, "D:/tree.xml");

            BaseHolder second = LoadFromXmlFile("D:/tree.xml", attrOverrides);

            SaveAsXmlFile(second, attrOverrides, "D:/treesec.xml");

            Console.Read();
        }

        static void SaveAsBinaryFile(object objGraph, string fileName)
        {
            // Сохранить объект в файле tree.dat в формате dat.
            BinaryFormatter xmlFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(fileName,
                                    FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, objGraph);
            }
            Console.WriteLine("\n=> Saved tree in Binary format!\n");
        }

        static void LoadFromBinaryFile(string fileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            // Read tree from binary file.
            using (Stream fStream = File.OpenRead(fileName))
            {
                BaseHolder treeFromDisk = (BaseHolder)binFormat.Deserialize(fStream);
                BaseHolder.Display(treeFromDisk);
            }
        }

        static void SaveAsSoapFile(object objGraph, string fileName)
        {
            // Сохранить объект в файле tree.soap в формате soap.
            SoapFormatter xmlFormat = new SoapFormatter();
            using (Stream fStream = new FileStream(fileName,
                                    FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, objGraph);
            }
            Console.WriteLine("\n=> Saved tree in SOAP format!\n");
        }

        static void LoadFromSoapFile(string fileName)
        {
            SoapFormatter binFormat = new SoapFormatter();
            using (Stream fStream = File.OpenRead(fileName))
            {
                BaseHolder treeFromDisk = (BaseHolder)binFormat.Deserialize(fStream);
                BaseHolder.Display(treeFromDisk);
            }
        }

        static void SaveAsXmlFile(object objGraph, XmlAttributeOverrides attrOverrides, string fileName)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(BaseHolder), attrOverrides);
            using (Stream fStream = new FileStream(fileName,
                                    FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, objGraph);
            }
            Console.WriteLine("\n=> Saved tree in Xml format!\n");
        }

        static BaseHolder LoadFromXmlFile(string fileName, XmlAttributeOverrides attrOverrides)
        {
            XmlSerializer binFormat = new XmlSerializer(typeof(BaseHolder), attrOverrides);
            using (Stream fStream = File.OpenRead(fileName))
            {
                BaseHolder treeFromDisk = (BaseHolder)binFormat.Deserialize(fStream);
                BaseHolder.Display(treeFromDisk);
                return treeFromDisk;
            }
        }

        private static XmlAttributeOverrides PrepareOverriddenAttributes()
        {
            var attrOverrides = new XmlAttributeOverrides();
            var attrs = new XmlAttributes();

            /* Override the BaseHolder class.*/
            var attr = new XmlElementAttribute("String", typeof(CharArrayHolder));
            attrs.XmlElements.Add(attr);

            var attr1 = new XmlElementAttribute("Int", typeof(IntHolder));
            attrs.XmlElements.Add(attr1);

            var attr2 = new XmlElementAttribute("Double", typeof(DoubleHolder));
            attrs.XmlElements.Add(attr2);

            attrOverrides.Add(typeof(BaseHolder), "TreeRoot", attrs);

            return attrOverrides;
        }
    }

    [Serializable, XmlInclude(typeof(IntHolder)), XmlInclude(typeof(CharArrayHolder)), XmlInclude(typeof(DoubleHolder))]
    public abstract class BaseHolder
    {
        public BaseHolder() { }

        public BaseHolder[] Arr { get;  set; }
        public object Value { get;  set; }

        public static void Display(BaseHolder bh)
        {
            Console.Write(bh.Value + "\t");

            if (bh.Arr != null)
            {
                for (int i = 0; i < bh.Arr.Length; i++) Display(bh.Arr[i]);
            }
            else
            {
                return;
            }
        }
    }

    [Serializable]
    public class IntHolder : BaseHolder
    {
        public IntHolder() { }

        public IntHolder(int obj, params BaseHolder[] param)
        {
            Value = obj;
            Arr = new BaseHolder[param.Length];

            for (int i = 0; i < param.Length; i++) Arr[i] = param[i];
        }
    }

    [Serializable]
    public class CharArrayHolder : BaseHolder
    {
        public CharArrayHolder() { }

        public CharArrayHolder(string obj, params BaseHolder[] param)
        {
            Value = obj;
            Arr = new BaseHolder[param.Length];

            for (int i = 0; i < param.Length; i++) Arr[i] = param[i];
        }
    }

    [Serializable]
    public class DoubleHolder : BaseHolder
    {
        public DoubleHolder() { }

        public DoubleHolder(double obj, params BaseHolder[] param)
        {
            Value = obj;
            Arr = new BaseHolder[param.Length];

            for (int i = 0; i < param.Length; i++) Arr[i] = param[i];
        }
    }
}
