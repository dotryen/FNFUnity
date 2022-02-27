using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public static class XMLData {
    [XmlRoot(ElementName = "TextureAtlas")]
    public struct TextureAtlas {
        [XmlElement("SubTexture")]
        public SubTexture[] data;
    }

    public struct SubTexture : IXmlSerializable {
        public string name;

        public int x;
        public int y;

        public int width;
        public int height;

        public int? frameX;
        public int? frameY;

        public int? frameWidth;
        public int? frameHeight;

        // Offsets are hardcoded add to atlas for these
        // public int offsetX;
        // public int offsetY;

        public void ReadXml(XmlReader reader) {
            name = reader.GetAttribute("name");

            x = int.Parse(reader.GetAttribute("x"));
            y = int.Parse(reader.GetAttribute("y"));
            width = int.Parse(reader.GetAttribute("width"));
            height = int.Parse(reader.GetAttribute("height"));

            frameX = ConvertToNullable<int>(reader.GetAttribute("frameX"));
            frameY = ConvertToNullable<int>(reader.GetAttribute("frameY"));
            frameWidth = ConvertToNullable<int>(reader.GetAttribute("frameWidth"));
            frameHeight = ConvertToNullable<int>(reader.GetAttribute("frameHeight"));

            reader.Read();
        }

        private static T? ConvertToNullable<T>(string inputValue) where T : struct {
            if (string.IsNullOrEmpty(inputValue) || inputValue.Trim().Length == 0) {
                return null;
            }

            try {
                TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                return (T)conv.ConvertFrom(inputValue);
            } catch (NotSupportedException) {
                // The conversion cannot be performed
                return null;
            }
        }

        public XmlSchema GetSchema() { return null; }
        public void WriteXml(XmlWriter writer) { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Example class DO NOT USE
    /// </summary>
    public class Property : IXmlSerializable {
        public int? Value1 { get; private set; }
        public float? Value2 { get; private set; }

        public void ReadXml(XmlReader reader) {
            string attr1 = reader.GetAttribute("attr");
            string attr2 = reader.GetAttribute("attr2");
            reader.Read();

            Value1 = ConvertToNullable<int>(attr1);
            Value2 = ConvertToNullable<float>(attr2);
        }

        private static T? ConvertToNullable<T>(string inputValue) where T : struct {
            if (string.IsNullOrEmpty(inputValue) || inputValue.Trim().Length == 0) {
                return null;
            }

            try {
                TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                return (T)conv.ConvertFrom(inputValue);
            } catch (NotSupportedException) {
                // The conversion cannot be performed
                return null;
            }
        }

        public XmlSchema GetSchema() { return null; }
        public void WriteXml(XmlWriter writer) { throw new NotImplementedException(); }
    }
}
