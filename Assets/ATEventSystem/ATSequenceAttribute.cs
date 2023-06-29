using System;

namespace AT.Sequence
{ 
    [AttributeUsage(AttributeTargets.Class)]
    public class ATSequenceAttribute : Attribute
    {
        private string menuPath = null;

        private Type variableType = null;

        public string MenuPath {
            get {
                return menuPath;
            } 
        }

        public Type VariableType {
            get {
                return variableType;
            }
        }

        public ATSequenceAttribute(string path, Type type = null)
        {
            menuPath = path;
            variableType = type;
        }
    }
}