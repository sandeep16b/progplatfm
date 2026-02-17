using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.App.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class RuleResults
    {
#pragma warning restore 1591
        #region Private Members
        private IList<string> _validations { get; set; }
        private bool _validationIsValid { get; set; }
        private IList<IStep> _steps { get; set; }
        #endregion

        #region Private Methods

        /// <summary>
        /// AddEvaluationStepResult
        /// </summary>
        /// <param name="step"></param>
        private void AddStepResult(IStep step)
        {
            _steps.Add(step);
            MetLastStepRequirement = step.MeetStepRule;
        }

        /// <summary>
        /// Add ValidationResult 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="validatorName"></param>
        private void AddValidationResult(ValidationResult result, string validatorName)
        {

            if (_validations == null)
                _validations = new List<string>();

            _validations.Add($"Validator:'{validatorName.Substring(validatorName.LastIndexOf('.') + 1)}' Error(s):'{string.Join(", ", result.Errors.ToList())}'");
        }

        #endregion

        #region Public Members

        /// <summary>
        /// MetLastStepRequirement
        /// </summary>
        public bool MetLastStepRequirement { get; set; }

        /// <summary>
        /// MeetRuleRequirement
        /// </summary>
        public bool MeetRuleRequirement { get; set; }

        /// <summary>
        /// CorrectiveActionResult
        /// </summary>
        public CorrectiveActionResult CorrectiveActionResult { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// ValidateCredential
        /// </summary>
        /// <param name="func"></param>
        /// <param name="validatorName"></param>
        /// <returns></returns>
        public RuleResults Validate(Func<ValidationResult> func, string validatorName)
        {
            var validationResult = func();
            _validationIsValid = validationResult.IsValid;

            if (!_validationIsValid)
                AddValidationResult(validationResult, validatorName);

            return this;
        }

        /// <summary>
        /// successfull Event  OnMeetRequirements
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public RuleResults OnMeetStep(Func<IStep> func)
        {
            if (!MetLastStepRequirement)
                return this;

            var resultStep = func();

            AddStepResult(resultStep);

            return this;
        }

        /// <summary>
        /// To start chain
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public RuleResults BeginStep(Func<IStep> func)
        {
            if (!_validationIsValid)
                return this;

            _steps = new List<IStep>();

            var resultStep = func();

            AddStepResult(resultStep);

            return this;
        }

        /// <summary>
        ///  Execute Func
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public void Update(Func<IEnumerable<RuleResults>> func)
        {
            func();
        }

        /// <summary>
        /// 
        /// </summary>
        public void MapStepsToCorrectiveActionResults(bool isLastIteration = false)
        {
            var meetRules = AreRuleRequirementsMet();

            if (_validations != null)
            {
                CorrectiveActionResult.ValidationResults = string.Join("; ", _validations);
                return;
            }

            // only do step mapping when meetRules or get to the end (isLastIteration)
            if (isLastIteration || meetRules)
                MapSteps();
        }

        /// <summary>
        /// MapMeetRuleRequirement
        /// </summary>
        public bool AreRuleRequirementsMet()
        {
            //steps are not null and more than one step (MaintenanceStatusStep is always last step)
            if (_steps?.Count() > 1)
                MeetRuleRequirement = _steps.Last().MeetStepRule;
            else if (_steps?.Count() == 1) // one step process is always has just MaintenanceStatusStep
            {
                var maintenanceStatusStep = _steps.First() as MaintenanceStatusStep;
                MeetRuleRequirement = maintenanceStatusStep != null ? maintenanceStatusStep.MeetMaintenanceStatus : MeetRuleRequirement;
            }

            CorrectiveActionResult.MeetRule = MeetRuleRequirement;

            return MeetRuleRequirement;
        }
        /// <summary>
        /// MapSteps
        /// </summary>
        public void MapSteps()
        {
            //since this field can contain results from different steps...
            CorrectiveActionResult.AdditionalResults = null;

            foreach (IStep step in _steps)
            {
                var ca = CorrectiveActionResult;
                step.MapStep(ref ca);
                CorrectiveActionResult = ca;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="credentialCategory"></param>
        /// <param name="PBI"></param>
        /// <param name="createdBy"></param>
        public RuleResults(Guid credentialId, Guid memberId, DateTime eventDate, CredentialCategoryType credentialCategory, string PBI, string createdBy)
        {
            _validationIsValid = true;
            CorrectiveActionResult = CorrectiveActionResult.Create(credentialId, memberId, eventDate, credentialCategory, PBI, createdBy);
        }

        #endregion
#pragma warning restore 1591
    }
}
