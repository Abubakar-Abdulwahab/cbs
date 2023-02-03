using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public abstract class BaseBilling
    {
        public ILogger Logger { get; set; }

        public BaseBilling()
        {
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get Duration Model for fixed billing
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="billing"></param>
        /// <param name="startDateAndTime"></param>
        /// <returns>DurationModel</returns>
        protected DurationModel GetDuration(DurationModel duration, DateTime startDateAndTime, ref List<ErrorModel> errors, bool validateVariable = false)
        {
            if (duration == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.durationisrequired().ToString(), FieldName = "Duration" });
                throw new DirtyFormDataException();
            }

            DurationModel model = new DurationModel();
            model.DurationType = duration.DurationType;

            if (duration.DurationType == DurationType.EndsAfter)
            {
                if (duration.EndsAfterRounds <= 0)
                {
                    Logger.Error("Ends after round not valid. Value " + duration.EndsAfterRounds);
                    errors.Add(new ErrorModel { ErrorMessage = "Ends after round not valid. Insert a valid number greater than 0.", FieldName = "Duration.EndsAfterRounds" });
                    throw new DirtyFormDataException();
                }
                model.EndsAfterRounds = duration.EndsAfterRounds;
                return duration;
            }
            else if (duration.DurationType == DurationType.EndsOn)
            {
                DateTime endsDate = new DateTime();
                try
                { endsDate = DateTime.ParseExact(duration.EndsAtDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture); }
                catch (Exception exception)
                {
                    Logger.Error(exception, "Could not parse billing duration end date");
                    errors.Add(new ErrorModel { ErrorMessage = "Unexpected date format found. Expected Date format is dd/MM/yyyy e.g 29/12/2017", FieldName = "Duration.EndsAtDate" });
                    throw new DirtyFormDataException("Unexpected date format found. Expected Date format is dd/MM/yyyy e.g 29/12/2017");
                }

                if (validateVariable == true)
                {
                    if (DateTime.Now.ToLocalTime() >= endsDate)
                    {
                        Logger.Error(string.Format("Error on duration ends on. Start date {0} is greater tha end date {1}", startDateAndTime.ToString(), endsDate.ToString()));
                        errors.Add(new ErrorModel { FieldName = "Duration.EndsAtDate", ErrorMessage = string.Format("Start date {0} is greater tha end date {1}", startDateAndTime.ToString("dd'/'MM'/'yyyy"), endsDate.ToString("dd'/'MM'/'yyyy")) });
                        throw new DirtyFormDataException(string.Format("Start date {0} is greater tha end date {1}", startDateAndTime.ToString(), endsDate.ToString()));
                    }
                }
                else
                {
                    if (DateTime.Now.ToLocalTime() >= endsDate || startDateAndTime >= endsDate)
                    {
                        Logger.Error(string.Format("Error on duration ends on. Start date {0} is greater tha end date {1}", startDateAndTime.ToString(), endsDate.ToString()));
                        errors.Add(new ErrorModel { FieldName = "Duration.EndsAtDate", ErrorMessage = string.Format("Start date {0} is greater tha end date {1}", startDateAndTime.ToString("dd'/'MM'/'yyyy"), endsDate.ToString("dd'/'MM'/'yyyy")) });
                        throw new DirtyFormDataException(string.Format("Start date {0} is greater tha end date {1}", startDateAndTime.ToString(), endsDate.ToString()));
                    }
                }
                model.EndsDate = endsDate;
                return model;
            }
            else if (duration.DurationType == DurationType.NeverEnds) { return model; }
            errors.Add(new ErrorModel { ErrorMessage = ErrorLang.durationisrequired().ToString(), FieldName = "Duration" });
            throw new DirtyFormDataException();
        }


        protected void SetCommon(BillingHelperModel helperModel, BillingModel billing)
        {
            billing.DemandNotice = helperModel.DemandNotice != null && helperModel.DemandNotice.IsChecked ? JsonConvert.SerializeObject(helperModel.DemandNotice) : null;
            billing.Discounts = helperModel.DiscountCollection != null ? JsonConvert.SerializeObject(helperModel.DiscountCollection) : null;
            billing.Penalties = helperModel.PenaltyCollection != null ? JsonConvert.SerializeObject(helperModel.PenaltyCollection) : null;
            billing.DueDate = JsonConvert.SerializeObject(helperModel.DueDateModel);
            billing.StillRunning = true;
        }
    }
}