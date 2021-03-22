using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace myTube.Services.XML
{
    /// <summary>
    /// A classe é responsável pela leitura sequencial do xml e retorno das tags que correspondem 
    /// aos tipos de classe que foram passados por parâmetro no momento da criação
    /// 
    /// O método ReadNextTag() retorna o objeto correspondente a Tag XML e para sua execução
    /// até a próxima chamada, retornando nulo quando mais nenhuma correspondencia for encontrada
    /// </summary>
    public class ReadTagsXML
    {
        private readonly List<Type> listTypes;
        private readonly XmlTextReader reader;

        public ReadTagsXML(string fileName, List<Type> listTypes)
        {
            this.listTypes = listTypes;
            reader = new XmlTextReader(fileName);
        }

        public (object obj, string xml) ReadNextTag()
        {
            object obj = null;
            var tagName = "";
            var xml = "";

            while (reader.Read())
            {
                var readerName = reader.Name.Replace(":","");

                if (reader.NodeType == XmlNodeType.Element && listTypes.Exists(p => p.Name.ToLower() == readerName.ToLower()))
                {
                    tagName = readerName;
                }

                if (!string.IsNullOrEmpty(tagName))
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!reader.IsEmptyElement)
                            {
                                xml += "<" + readerName + ">";
                            }
                            break;
                        case XmlNodeType.Text:
                            xml += reader.Value.Replace("&","");
                            break;
                        case XmlNodeType.EndElement:
                            xml += "</" + readerName + ">";
                            if (readerName == tagName)
                            {
                                tagName = "";
                            }
                            break;
                    }
                }

                if (string.IsNullOrEmpty(tagName) && !string.IsNullOrEmpty(xml))
                {
                    var t = listTypes.Find(p => p.Name.ToLower() == readerName);
                    var xRoot = new XmlRootAttribute
                    {
                        ElementName = t.Name,
                        IsNullable = true,
                        Namespace = ""
                    };
                    var serializer = new XmlSerializer(t);
                    var sr = new StringReader(xml);

                    obj = serializer.Deserialize(sr);

                    break;
                }
            }

            return (obj, xml);
        }

        public void Close()
        {
            reader.Close();
        }

    }
}
