using System;
using System.IO;
using UnityEngine; 

namespace AT.Serialization
{
    using Xml;

    public static class Serializer
    {
        public delegate object UpdateObjectFunction ( object obj , object functionData );
         
        public static T FromFile<T> ( string fileName )
        { 
            if ( !string.IsNullOrEmpty ( fileName ) ) {
                return XmlConverter.FromFile<T> ( fileName );
            }

            return default ( T );
        }

        public static T FromTextAsset<T> ( TextAsset asset )
        { 
            if ( asset != null ) {
                return XmlConverter.FromXmlString<T> ( asset.text );
            }

            return default ( T );
        }

        public static T FromString<T> ( string text )
        { 
            return XmlConverter.FromXmlString<T> ( text );
        }

        public static object FromString ( Type type , string text )
        { 
            return XmlConverter.FromXmlString ( type , text );
        }

        public static string ToString<T> ( T obj )
        {
            return XmlConverter.ToXmlString ( obj );
        }

        public static bool ToFile<T> ( T obj , string fileName )
        {
            if ( Path.GetExtension ( fileName ).ToLower ( ) == ".xml" ) {
                string directory = Path.GetDirectoryName ( fileName );
                if ( !Directory.Exists ( directory ) ) {
                    Directory.CreateDirectory ( directory );
                }

                return XmlConverter.ToFile<T> ( obj , fileName );
            }

            return false;
        }

        public static T CreateCopy<T> ( T obj )
        {
            if ( obj != null ) {
                return XmlConverter.CreateCopy ( obj );
            }

            return default ( T );
        }

        public static bool DoesAssetContainObject<T> ( TextAsset asset )
        {
            if ( asset != null ) {
                return XmlConverter.DoesAssetContainNode<T> ( asset );
            }

            return false;
        }

        public static object UpdateChildObjects ( object obj , UpdateObjectFunction function , object functionData )
        {
            if ( obj != null ) {
                if ( obj.GetType ( ).IsArray ) {
                    Array array = obj as Array;

                    if ( array != null && array.Length > 0 ) {
                        for ( int i = 0 ; i < array.Length ; i++ ) {
                            object value = array.GetValue ( i );
                            value = UpdateChildObjects ( value , function , functionData );
                            array.SetValue ( value , i );
                        }
                    }
                } 
                else {
                    SerializedObjectMemberInfo [ ] nodeFields = SerializedObjectMemberInfo.GetSerializedFields ( obj.GetType ( ) );

                    foreach ( SerializedObjectMemberInfo nodeField in nodeFields ) {
                        object value = nodeField.GetValue ( obj );

                        if ( value != null )
                            value = function ( value , functionData );

                        value = UpdateChildObjects ( value , function , functionData );
                        nodeField.SetValue ( obj , value );
                    }
                }
            }

            return obj;
        } 
    }
}