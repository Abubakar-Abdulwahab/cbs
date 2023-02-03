using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Payee;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IBillingHandler : IDependency
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billingController"></param>
        /// <param name="model"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="frequencyModel"></param>
        /// <param name="discountModel"></param>
        /// <param name="penaltyModel"></param>
        /// <param name="assessmentAdapters"></param>
        /// <exception cref="AlreadyHasBillingException"></exception>
        /// <exception cref="CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException"></exception>
        /// <exception cref="NoBillingTypeSpecifiedException"></exception>
        /// <exception cref="DateTimeCouldNotBeParsedException"></exception>
        /// <exception cref="NoFrequencyTypeFoundException"></exception>
        void TryPostBillingDetails(BillingController billingController, BillingViewModel model, string revenueHeadSlug, int revenueHeadId, BillingFrequencyModel frequencyModel, ICollection<DiscountModel> discountModel, ICollection<PenaltyModel> penaltyModel, List<AssessmentInterface> assessmentInterfaces, bool isEdit);

        bool TryUpdateBillingDetails(BillingController billingController, BillingViewModel model, string revenueHeadSlug, int revenueHeadId, BillingFrequencyModel frequencyModel, ICollection<DiscountModel> discountModel, ICollection<PenaltyModel> penaltyModel, List<AssessmentInterface> assessmentInterfaces, bool isEdit, out bool isSuccess);


        /// <summary>
        /// Try get the billing view model for billing
        /// <para>Pass a true value for isEdit is this is an edit operation</para>
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="isEdit"></param>
        /// <returns>BillingViewModel</returns>
        BillingViewModel GetBillingView(int revenueHeadId, string revenueHeadSlug, bool isEdit);


        #region Edit ops

        /// <summary>
        /// Get list of assessment interfaces
        /// </summary>
        /// <returns>List{AssessmentInterface}</returns>
        List<AssessmentInterface> GetAssessmentInterfaces();


        /// <summary>
        /// Get file upload templates view model
        /// <para>Returns a list full or an empty list never a null value</para>
        /// </summary>
        /// <returns>FileUploadTemplatesVM</returns>
        FileUploadTemplatesVM GetFileUploadTemplatesVM();

        

        #endregion
    }
}
