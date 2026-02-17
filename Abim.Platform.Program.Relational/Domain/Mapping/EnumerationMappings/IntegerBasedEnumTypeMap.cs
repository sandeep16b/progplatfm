using NHibernate.SqlTypes;
using System;
using System.Data;

namespace Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings
{
    /// <summary>
    /// The use of this mapping class indicates that the enum's int/char code (e.g. "A" if the entry is AAA='A', or 65 if the entry is AAA=65)
    /// corresponds to the integer database value (e.g. 65, which in this example is the ASCII value for the character 'A', though it would be
    /// clearer on the part of the developer to avoid using characters and set enum value AAA to equal the integer 65 instead of 'A')
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerBasedEnumTypeMap<T> : NHibernate.Type.PersistentEnumType
        where T : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerBasedEnumTypeMap{T}"/> class.
        /// </summary>
        public IntegerBasedEnumTypeMap()
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
            return Convert.ToInt32(code);
        }

        /// <summary>
        /// Gets an instance of the Enum
        /// </summary>
        /// <param name="code">The underlying value of an item in the Enum.</param>
        /// <returns>
        /// An instance of the Enum set to the <c>code</c> value.
        /// </returns>
        public override object GetInstance(object code)
        {
            return (T) Enum.ToObject(typeof(T), (Byte) ((Char) GetValue(code)));
        }

        /// <summary>
        /// Sets the specified command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            IDataParameter par = (IDataParameter)cmd.Parameters[index];
            par.DbType = DbType.Int32;

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
            return SqlTypeFactory.Int32;
        }
    }
}
