using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.Relational.Services.Impl;
using NLog;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class LookBackDatesInfoService
    /// </summary>
    public class LookBackDatesInfoService : ServiceBase<LookBackDatesInfo, ILookBackDatesInfoRepository>, ILookBackDatesInfoService
    {
        /// <summary>
        /// The logger is inherited from the base class. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }
        /// <summary>
        /// Child service used for logging whenever a lookback date changes
        /// </summary>
        private ILookbackDateLogService _lookbackDateLogService;

        /// <summary>
        /// LookBackDatesInfoService 
        /// </summary>
        /// <param name="Repository"></param>
        /// <param name="lookBackDateLogService"></param>
        public LookBackDatesInfoService(ILookBackDatesInfoRepository Repository, ILookbackDateLogService lookBackDateLogService) :  base(Repository)
        {
            _lookbackDateLogService = lookBackDateLogService;
        }


        #region Public Methods

        /// <summary>
        /// Handles an UpdateLookBackDatesInfoCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task Handle(UpdateLookBackDatesInfoCommand command)
        {
            Log.Debug("UpdateLookBackDatesInfoCommand Command Args: {0}", command.Dump());

            try
            {
                var lookBackDatesInfo = Load(command.MemberId);

                // if no records found than create one ...
                if (lookBackDatesInfo == null)
                {
                    //add new record
                    lookBackDatesInfo = LookBackDatesInfo.Create(command.MemberId,
                                                            command.Lookback2YearStartDate,
                                                            command.Lookback2YearEndDate,
                                                            command.Lookback5YearStartDate,
                                                            command.Lookback5YearEndDate,
                                                            command.UserName);

                    await Repository.AddLookBackDatesInfo(lookBackDatesInfo);
                    //Issue commands to log the list of changed values
                    CreateLookbackDateLogCommands(null, lookBackDatesInfo, command);
                }
                // update existing records ...
                else
                {
                    //Grab a copy of the original values for use in comparison after call to ApplyUpdateLookBackDatesInfoCommand.
                    var origValues = LookBackDatesInfo.Create(lookBackDatesInfo.ExternalId,
                        lookBackDatesInfo.Lookback2YearStartDate,lookBackDatesInfo.Lookback2YearEndDate, 
                        lookBackDatesInfo.Lookback5YearStartDate,lookBackDatesInfo.Lookback5YearEndDate, lookBackDatesInfo.AuditData.CreatedBy);

                    //There is logic in ApplyUpdateLookBackDatesInfoCommand that controls when values get updated from the command
                    lookBackDatesInfo.ApplyUpdateLookBackDatesInfoCommand(command.Lookback2YearStartDate,
                                        command.Lookback2YearEndDate,
                                        command.Lookback5YearStartDate,
                                        command.Lookback5YearEndDate,
                                        command.UserName);

                    await Repository.UpdateLookBackDatesInfo(lookBackDatesInfo);
                    //Issue commands to log the list of changed values
                    CreateLookbackDateLogCommands(origValues, lookBackDatesInfo, command);
                }
            }
            finally
            {
                Log.Trace("Ended Handle for UpdateLookBackDatesInfoCommand");
            }
        }
        /// <summary>
        /// Helper method to create a instances of AddLookbackDateLogEntry commands and issue Handle calls for each command
        /// </summary>
        /// <param name="oldVals"></param>
        /// <param name="newVals"></param>
        /// <param name="command"></param>
        private void CreateLookbackDateLogCommands(LookBackDatesInfo oldVals, LookBackDatesInfo newVals, UpdateLookBackDatesInfoCommand command)
        {
            //Case when we are inserting for first time - there will be no oldVals
            if (oldVals == null)
            {
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.TwoYearStart, null, newVals.Lookback2YearStartDate, command));
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.TwoYearEnd, null, newVals.Lookback2YearEndDate, command));
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.FiveYearStart, null, newVals.Lookback5YearStartDate, command));
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.FiveYearEnd, null, newVals.Lookback5YearEndDate, command));

                return;
            }

            if (HaveDatesChanged(oldVals.Lookback2YearStartDate, newVals.Lookback2YearStartDate))
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.TwoYearStart, oldVals.Lookback2YearStartDate, newVals.Lookback2YearStartDate, command));

            if (HaveDatesChanged(oldVals.Lookback2YearEndDate, newVals.Lookback2YearEndDate))
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.TwoYearEnd, oldVals.Lookback2YearEndDate, newVals.Lookback2YearEndDate, command));

            if (HaveDatesChanged(oldVals.Lookback5YearStartDate, newVals.Lookback5YearStartDate))
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.FiveYearStart, oldVals.Lookback5YearStartDate, newVals.Lookback5YearStartDate, command));

            if (HaveDatesChanged(oldVals.Lookback5YearEndDate, newVals.Lookback5YearEndDate))
                _lookbackDateLogService.Handle(CreateLookbackDateLogCommand(LookbackDateType.FiveYearEnd, oldVals.Lookback5YearEndDate, newVals.Lookback5YearEndDate, command));
        }
        /// <summary>
        /// Helper method to create a single instance of a AddLookbackDateLogEntry command
        /// </summary>
        /// <param name="dateType"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private AddLookbackDateLogEntry CreateLookbackDateLogCommand(LookbackDateType dateType, DateTime? oldValue,DateTime? newValue, UpdateLookBackDatesInfoCommand command)
        {
            return new AddLookbackDateLogEntry
            {
                LookbackDate = dateType,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedDate = DateTime.Now,
                UserName = command.UserName,
                MemberGuid = command.MemberId
            };
        }
        /// <summary>
        /// Helper method to determine if two nullable dates differ by Date without time
        /// </summary>
        /// <param name="oldVal"></param>
        /// <param name="newVal"></param>
        /// <returns></returns>
        private bool HaveDatesChanged(DateTime? oldVal, DateTime? newVal)
        {
            if ((oldVal.HasValue && !newVal.HasValue) || (!oldVal.HasValue && newVal.HasValue))
                return true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!oldVal.HasValue && !newVal.HasValue)
                return false;

            if (oldVal.Value.Date != newVal.Value.Date)
                return true;

            return false;
        }
        /// <summary>
        /// GetLookBackDatesInfo
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public Task<LookBackDatesInfo> GetLookBackDatesInfo(Guid memberId)
        {
            var lookBackDatesInfo = Load(memberId);

            return Task.FromResult(lookBackDatesInfo);
        }

        /// <summary>
        /// GetExpiredLookBackDatesInfo
        /// </summary>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        public Task<IEnumerable<LookBackDatesInfo>> GetExpiredLookBackDatesInfo(DateTime expiredDate)
        {
            var lookBackDatesInfo = Repository.GetExpiredYearEndLookBackDatesInfo(expiredDate);

            return Task.FromResult(lookBackDatesInfo); 
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the child services.
        /// </summary>
        protected override void DisposeChildServices()
        {
            if(_lookbackDateLogService != null)
                _lookbackDateLogService.Dispose();
        }

        #endregion
    }
}
