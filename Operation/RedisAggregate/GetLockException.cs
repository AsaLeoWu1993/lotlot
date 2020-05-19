using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Operation.RedisAggregate
{
    [Serializable]
    public class GetLockExceptionArgs : ExceptionArgs
    {
    }

    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable
       where TExceptionArgs : ExceptionArgs
    {
        private const string c_args = "Args"; // For (de)serialization
        private readonly TExceptionArgs m_args;
        public TExceptionArgs Args { get { return m_args; } }
        public Exception(string message = null, Exception innerException = null)
            : this(null, message, innerException)
        {
        }

        public Exception(TExceptionArgs args, string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            m_args = args;
        }

        // This constructor is for deserialization; since the class is sealed, the constructor is
        // private. If this class were not sealed, this constructor should be protected
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_args = (TExceptionArgs)info.GetValue(c_args, typeof(TExceptionArgs));
        }

        // This method is for serialization; it’s public because of the ISerializable interface
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(c_args, m_args);
            base.GetObjectData(info, context);
        }

        public override string Message
        {
            get
            {
                string baseMsg = base.Message;
                return (m_args == null) ? baseMsg : baseMsg + " (" + m_args.Message + ")";
            }
        }
        public override Boolean Equals(Object obj)
        {
            if (!(obj is Exception<TExceptionArgs> other))
            {
                return false;
            }

            return object.Equals(m_args, other.m_args) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public abstract class ExceptionArgs
    {
        public virtual string Message
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
