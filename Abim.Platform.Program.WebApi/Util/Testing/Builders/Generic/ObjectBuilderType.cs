using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Non-generic base class
    /// </summary>
    public abstract class ObjectBuilderType
    {
        #region Properties

        /// <summary>
        /// Whether guids in the command will be valid or not
        /// </summary>
        protected bool ValidGuids = true;

        /// <summary>
        /// Whether enum values in the command will be valid or not
        /// </summary>
        protected bool ValidEnums = true;

        /// <summary>
        /// Whether strings in the command will be valid or not
        /// </summary>
        protected bool ValidStrings = true;

        /// <summary>
        /// Whether ints in the command will be valid or not
        /// </summary>
        protected bool ValidIntegers = true;

        /// <summary>
        /// Whether DateTimes in the command will be valid or not
        /// </summary>
        protected bool ValidDateTimes = true;

        /// <summary>
        /// Whether lists in the command will be valid or not
        /// </summary>
        protected bool ValidLists = true;
        
        /// <summary>
        /// Whether zero will be used as an integer. Only relevant when ValidIntegers is true
        /// </summary>
        protected bool AllowZeroInIntegers = true;

        /// <summary>
        /// The random
        /// </summary>
        protected static Random Random = new Random();

        #endregion

        /// <summary>
        /// Gets an integer.
        /// </summary>
        /// <returns></returns>
        protected int GetInteger()
        {
            if(ValidIntegers)
            {
                if(AllowZeroInIntegers && Random.Next() % 4 == 0) return 0;
                return Random.Next(1, int.MaxValue);
            }
            return Random.Next(int.MinValue, int.MaxValue);
        }

        /// <summary>
        /// Gets a string.
        /// </summary>
        /// <returns></returns>
        protected string GetString()
        {
            if(ValidStrings) return RandomString.Build();
            return RandomString.BuildOrNull();
        }

        /// <summary>
        /// Gets a date time.
        /// </summary>
        /// <returns></returns>
        protected DateTime GetDateTime()
        {
            if(ValidDateTimes)
            {
                var minValidTicks = 1000;
                var maxValidTicks = (int)(Math.Min((new DateTime(2030, 12, 31)).Ticks, int.MaxValue));
                return new DateTime(Random.Next(minValidTicks, maxValidTicks));
            }
            var highInvalidButBindableTicks = (int)(Math.Min((new DateTime(3000, 12, 31)).Ticks, int.MaxValue));
            return new DateTime(Random.Next(0, highInvalidButBindableTicks));
        }

        /// <summary>
        /// Gets a unique identifier.
        /// </summary>
        /// <returns></returns>
        protected Guid GetGuid()
        {
            if(ValidGuids) return Guid.NewGuid();
            return RandomBool.IsTrue ? Guid.NewGuid() : Guid.Empty;
        }

        /// <summary>
        /// Gets a list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected Object GetList(Type type)
        {
            if(ValidLists) return (typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]).Invoke(new object[0]));
            return null;
        }
    }
}
