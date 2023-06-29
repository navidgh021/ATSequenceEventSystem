using System;

namespace AT.Serialization.Xml
{
    [AttributeUsage ( AttributeTargets.Class , AllowMultiple = false )]
    public sealed class XmlObjectConverterAttribute : Attribute
    { 
        public readonly Type ObjectType;

        public readonly string XmlTag;

        public readonly string OnConvertToXmlNodeMethod;

        public readonly string OnConvertFromXmlNodeMethod;

        public readonly string ShouldWriteMethod; 
         
        public XmlObjectConverterAttribute ( Type objectType , string xmlTag , string onConvertToXmlNodeMethod , string onConvertFromXmlNodeMethod , string shouldWriteMethod )
        {
            ObjectType = objectType;
            XmlTag = xmlTag;
            OnConvertToXmlNodeMethod = onConvertToXmlNodeMethod;
            OnConvertFromXmlNodeMethod = onConvertFromXmlNodeMethod;
            ShouldWriteMethod = shouldWriteMethod;
        } 
    }
}