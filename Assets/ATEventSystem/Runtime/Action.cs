using System.Reflection;

namespace AT.Sequence.Runtime
{
    public abstract class Action : ATSequenceEvent
    {
        public ActionMethod actionMethod;

        protected override void BindEvent ()
        {
            if ( !string.IsNullOrEmpty (actionMethod.methodName) ) {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                this.GetType ().GetMethod (actionMethod.methodName, flags).Invoke (this, null);
            }
        }
    }
}