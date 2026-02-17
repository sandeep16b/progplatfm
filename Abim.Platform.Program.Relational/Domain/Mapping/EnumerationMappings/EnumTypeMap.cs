using NHibernate.SqlTypes;
using System;
using System.Data;

namespace Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings
{
    /// <summary>
    /// Base class for NHibernate mapping classes for enumeration types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NHibernate.Type.PersistentEnumType" />
    public abstract class EnumTypeMap<T> : NHibernate.Type.PersistentEnumType
        where T : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTypeMap{T}"/> class.
        /// </summary>
        public EnumTypeMap()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the correct value for the Enum.
        /// </summary>
        /// <param name="code">The value to convert (an enum instance).</param>
        /// <returns>
        /// A boxed version of the code, converted to the correct type.
        /// </returns>
        /// <remarks>
        /// This handles situations where the DataProvider returns the value of the Enum
        /// from the db in the wrong underlying type.  It uses <see cref="T:System.Convert" /> to
        /// convert it to the correct type.
        /// </remarks>
        public override object GetValue(object code)
        {
            return Convert.ToChar(code);
        }

        /// <summary>
        /// Gets an instance of the Enum
        /// </summary>
        /// <param name="code">The underlying value of an item in the Enum.</param>
        /// <returns>
        /// An instance of the Enum set to the <c>code</c> value.
        /// </returns>
        public abstract override object GetInstance(object code);

        /// <summary>
        /// Sets the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            IDataParameter par = (IDataParameter)cmd.Parameters[index];
            par.DbType = DbType.StringFixedLength;

           if(value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = GetValue(value);
            }
        }

        /// <summary>
        /// Gets the underlying SQL type.
        /// </summary>
        /// <param name="enumClass">The enum class.</param>
        /// <returns></returns>
        public static SqlType GetUnderlyingSqlType(Type enumClass)
        {
           if(Enum.GetUnderlyingType(enumClass).FullName == "System.Char")
            {
                return SqlTypeFactory.GetString(1);
            }
            else
            {
                return SqlTypeFactory.GetString(50);
            }
        }
    }
}
