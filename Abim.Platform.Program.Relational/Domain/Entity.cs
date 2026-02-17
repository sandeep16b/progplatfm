using System;
using Abim.Platform.Program.Relational.Domain.Types;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// Entity base class for non-aggregate-root domain objects
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.IEntity{Int32}" />
    [Serializable]
    public abstract class Entity : IEntity<int>
    {
        /// <summary>
        /// A unique identifier based on our generic type.
        /// </summary>
        public virtual int Id { get; protected set;}

        /// <summary>
        /// Auditing metadata for the given model object.
        /// </summary>
        public virtual AuditData AuditData { get; protected internal set; }

        /// <summary>
        /// Equals method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return EntityEquals(obj as Entity);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is transient.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        protected bool IsTransient { get { return Id == 0; } }

        /// <summary>
        /// EntityEquals method.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected virtual bool EntityEquals(Entity other)
        {
            if(other == null)
            {
                return false;
            }
            
            //One entity is transient and the other is not.
           if(IsTransient ^ other.IsTransient)
            {
                return false;
            }
            
            //Both entities are not saved.
           if(IsTransient && other.IsTransient)
            {
                //perhaps cutom overload on reference equals .
                return ReferenceEquals(this, other);
            }
            
            //Compare transient instances.
            return Id.Equals(other.Id);
        }
    
        /// <summary>
        /// The cached hash code
        /// </summary>    
        /// <remarks>
        /// The hash code is cached because a requirement of a hash code is that it does not change once calculated. For example, if this entity was
        /// added to a hashed collection when transient and then saved, we need the same hash code or else it could get lost because it would no 
        /// longer live in the same bin.    
        /// </remarks>
        private int? _cachedHashCode;

        /// <summary>
        /// GetHashCode method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if(_cachedHashCode.HasValue)
                return _cachedHashCode.Value;

            _cachedHashCode = IsTransient ? base.GetHashCode() : Id.GetHashCode();
            return _cachedHashCode.Value;
        }

        /// <summary>
        /// Equality Operator
        /// </summary>
        /// <remarks>
        /// By default, == and Equals compares references. In order to maintain these semantics with entities, we need to compare by 
        /// identity value. The Equals(x, y) override is used to guard against null values; it then calls EntityEquals().
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Entity x, Entity y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Inquality Operator
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Entity x, Entity y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Sets the modified date and by-string.
        /// </summary>
        /// <param name="modifiedBy">The modified by.</param>
        /// <exception cref="System.ArgumentException">SetModified() cannot be given a null modifiedBy parameter</exception>
        /// <exception cref="System.Exception">SetModified() cannot proceed because AuditData is null</exception>
        public virtual void SetModified(string modifiedBy)
        {
            if(modifiedBy == null) throw new ArgumentException("SetModified() cannot be given a null modifiedBy parameter");
            if(AuditData == null) throw new Exception("SetModified() cannot proceed because AuditData is null");
            AuditData.ModifiedBy = modifiedBy;
            AuditData.Modified = DateTime.Now;
        }
    }
}
