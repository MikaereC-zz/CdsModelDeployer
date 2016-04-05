using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ScriptExecutor
{
    public class GenericSerializer<T> 
    {
        public string Serialize(T source)
        {
            using (StringWriter writer = new StringWriter())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(writer, source);
                return writer.ToString();
            }
        }

        public T Deserialize(string xml)
        {
            using (StringReader reader = new StringReader(xml))
            {
                var s = new XmlSerializer(typeof(T));
                return (T)s.Deserialize(reader);
            }
        }
    }
}
