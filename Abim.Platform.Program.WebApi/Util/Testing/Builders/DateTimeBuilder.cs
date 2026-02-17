using System;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Constructs a random DateTime
    /// </summary>
    public class DateTimeBuilder
    {
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        /// <value>
        /// The day.
        /// </value>
        public int Day { get; set; }

        /// <summary>
        /// The System.Random.
        /// </summary>
        /// <value>
        /// The rand.
        /// </value>
        protected Random Rand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeBuilder"/> class.
        /// </summary>
        public DateTimeBuilder()
        {
            Rand = new Random();
        }

        /// <summary>
        /// Sets the day.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        public DateTimeBuilder WithDay(int day)
        {
            Day = day;
            return this;
        }

        /// <summary>
        /// Sets the month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        public DateTimeBuilder WithMonth(int month)
        {
            Month = month;
            return this;
        }

        /// <summary>
        /// Sets the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public DateTimeBuilder WithYear(int year)
        {
            Year = year;
            return this;
        }

        /// <summary>
        /// Randomizes the properties.
        /// </summary>
        /// <param name="startYear">The start year.</param>
        /// <param name="endYear">The end year.</param>
        /// <returns></returns>
        private DateTimeBuilder Random(int startYear, int endYear)
        {
            Year = Rand.Next(startYear, endYear);
            Month = Rand.Next(1, 12);
            
            var daysInFeb = 28;
            if(Year % 4 == 0)
            {
                if(Year % 400 == 0) daysInFeb = 29;
                else if(Year % 100 == 0) daysInFeb = 28;
                else daysInFeb = 29;
            }
            var daysInEachMonth = new[]{ 31, daysInFeb, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            Day = Rand.Next(1, daysInEachMonth[Month - 1]);
            
            return this;
        }

        /// <summary>
        /// Randomizes the properties.
        /// </summary>
        /// <returns></returns>
        public DateTimeBuilder Random()
        {
            return Random(1975, 2030);
        }

        /// <summary>
        /// Randomizes the properties but ensures that the resultant DateTime is in the past.
        /// </summary>
        /// <returns></returns>
        public DateTimeBuilder Past()
        {
            return Random(1975, DateTime.Now.Year - 1);
        }
        
        /// <summary>
        /// Randomizes the properties but ensures that the resultant DateTime is in the future.
        /// </summary>
        /// <returns></returns>
        public DateTimeBuilder Future()
        {
            return Random(DateTime.Now.Year + 1, 2030);
        }

        /// <summary>
        /// Builds the DateTime.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public DateTime Build()
        {
            try
            {
                return new DateTime(Year, Month, Day);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                var msg = string.Format("Error building DateTime from year {0}, month {1}, day {2}", Year, Month, Day);
                if(Year == 0 && Month == 0 && Day == 0)
                    msg += ". Did you forget to set the Year, Month, and Day properties on the DateTimeBuilder, or call Random() or another intialization method?";
                throw new Exception(msg, ex);
            }
        }

        /// <summary>
        /// Builds a random nullable DateTime, in which there's a 50% chance of it being null.
        /// </summary>
        /// <returns></returns>
        public DateTime? BuildOrNull()
        {
            if(Rand.Next(0, 1) == 0)
                return Build();
            return null;
        }
    }
}
