using System;

namespace AT.Serialization
{
    public class CorruptFileException : Exception
    {
        public CorruptFileException ( Exception exception ) : base ( "Corrupt File" , exception )
        {

        }
    }
}