using System;
using FluentValidation;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// Use to set the LookBackDatesInfo 
    /// </summary>
    public class LookBackDatesInfo :
        AggregateRoot<LookBackDatesInfo>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Lookback2YearStartDate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual DateTime? Lookback2YearStartDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Lookback2YearEndDate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual DateTime? Lookback2YearEndDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Lookback5YearStartDate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual DateTime? Lookback5YearStartDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Lookback5YearStartDate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual DateTime? Lookback5YearEndDate { get; protected internal set; }


        #region Factory

        /// <summary>
        /// Initializes a new instance of the <see cref="LookBackDatesInfo"/> class.
        /// </summary>
        protected LookBackDatesInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lookback2YearStartDate"></param>
        /// <param name="lookback2YearEndDate"></param>
        /// <param name="lookback5YearStartDate"></param>
        /// <param name="lookback5YearEndDate"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        protected internal static LookBackDatesInfo Create(  Guid memberId,
                                                        DateTime? lookback2YearStartDate,
                                                        DateTime? lookback2YearEndDate,
                                                        DateTime? lookback5YearStartDate,
                                                        DateTime? lookback5YearEndDate,
                                                        string createdBy)
        {

            return new LookBackDatesInfo()
            {
                ExternalId = memberId,
                Lookback2YearStartDate = lookback2YearStartDate,
                Lookback2YearEndDate = lookback2YearEndDate,

                Lookback5YearStartDate = lookback5YearStartDate,
                Lookback5YearEndDate = lookback5YearEndDate,

                AuditData = AuditData.Create(createdBy)
            };

        }

        #endregion

        #region Apply Methods

        /// <summary>
        /// ApplyUpdateLookBackDatesInfoCommand
        /// </summary>
        /// <param name="lookback2YearStartDate"></param>
        /// <param name="lookback2YearEndDate"></param>
        /// <param name="lookback5YearStartDate"></param>
        /// <param name="lookback5YearEndDate"></param>
        /// <param name="modifiedBy"></param>
        public virtual void ApplyUpdateLookBackDatesInfoCommand(DateTime? lookback2YearStartDate,
                                                      DateTime? lookback2YearEndDate,
                                                      DateTime? lookback5YearStartDate,
                                                      DateTime? lookback5YearEndDate,
                                                      string modifiedBy)
        {

            if (lookback2YearEndDate.HasValue && lookback2YearStartDate.HasValue)
            {
                Lookback2YearStartDate = lookback2YearStartDate;
                Lookback2YearEndDate = lookback2YearEndDate;
            }

            if (lookback5YearStartDate.HasValue && lookback5YearEndDate.HasValue)
            {
                Lookback5YearStartDate = lookback5YearStartDate;
                Lookback5YearEndDate = lookback5YearEndDate;
            }

            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        #endregion


        #endregion Properties

        #region Factory

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as LookBackDatesInfo;
            if (other == null)
                return false;

            return (ExternalId == other.ExternalId);
        }
        /// <summary>
        /// GetHashCode <see cref="System.Object" />.
        /// </summary>
        /// <param></param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override int GetHashCode()
        {
            return ExternalId.GetHashCode();
        }
    }

    #region Validator Classes

    /// <summary>
    /// Validator for LookBackDatesInfo
    /// </summary>
    public class LookBackDatesInfoValidator : AbstractValidator<LookBackDatesInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceValidator"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public LookBackDatesInfoValidator(IValidationFactory factory)
        {
            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }

    #endregion
}
