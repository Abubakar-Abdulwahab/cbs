using System;
using Orchard.UI.Resources;


namespace Parkway.CBS.Resources
{
    public class ResourceManifest : IResourceManifestProvider
    {

        #region CSS

        private void CSSResources(Orchard.UI.Resources.ResourceManifest manifest)
        {
            manifest
                 .DefineStyle("CBS.MDA.Style.BootStrap")
                 .SetUrl("bootstrap.min.css");

            manifest
                 .DefineStyle("CBS.Bootstrap")
                 .SetUrl("client-bootstrap.min.css");

            manifest
                 .DefineStyle("CBS.MDA.Style")
                 .SetUrl("cbs-mda-style.css");

            manifest
                  .DefineStyle("CBS.MDA..Tree.Style")
                  .SetUrl("cbs-mda-tree-style.css");

            manifest
               .DefineStyle("CBS.Billing.Style")
               .SetUrl("cbs.billing.css");

            manifest
                .DefineStyle("CBS.MDA.Style.ParkStrap")
                .SetUrl("cbs-mda-style-parkstrap.css");

            manifest
                .DefineStyle("CBS.Style.Main")
                .SetUrl("cbs-main-style.css?v=5");

            manifest
                .DefineStyle("CBS.Style.Pages")
                .SetUrl("cbs-pages-style.css");

            manifest
                 .DefineStyle("CBS.MDA.Style.RevenueStrap")
                 .SetUrl("cbs-mda-style-revenuestrap.css");

            manifest
                .DefineStyle("CBS.MDA.Style.RevenueHead")
                .SetUrl("cbs-mda-style-revenuehead.css");

            manifest
                .DefineStyle("Style.Datepicker")
                .SetUrl("datepicker.css");

            manifest
                .DefineStyle("CBS.MDA.Style.Chart")
                .SetUrl("custom_chart.css");

            manifest.DefineStyle("Registration").SetUrl("Registration.css").SetDependencies("jQueryUI", "CBS.MDA.Style.BootStrap");

            manifest.DefineStyle("FormWizard").SetUrl("FormWizard.css");

            manifest.DefineStyle("font-awesome").SetUrl("font-awesome.css");

            manifest.DefineStyle("DateTimePicker").SetUrl("DateTimePicker.css").SetDependencies("CBS.MDA.Style.BootStrap");

            manifest
                .DefineStyle("CBS.MDA.Style.RevenueDashboard")
                .SetUrl("revenue_dashboard.css");

            manifest
                .DefineStyle("CBS.MDA.Style.MonthPicker")
                .SetUrl("monthpicker.css");

            manifest
                .DefineStyle("CBS.MDA.Style.Assesment")
                .SetUrl("assesment.css");

            manifest
                .DefineStyle("CBS.MDA.Style.Admin_Login")
                .SetUrl("backend-update-1.css");

            manifest
                .DefineStyle("CBS.MDA.Style.Demand")
                .SetUrl("cbs-demand.css");

            manifest
                .DefineStyle("CBS.Jsdelivr.Style")
                .SetUrl("jsdelivrdaterangepicker.css");

            manifest
                .DefineStyle("CBS.TSA.Style")
                .SetUrl("cbs-tsa.css");

            //style to restyle awkward paginations with bootstrap on the view page
            manifest.DefineStyle("CBS.Report.Reset.Pagination").SetUrl("reset-pagination.css");


            #region NPF Styles
            manifest.DefineStyle("CBS.NPF.Main").SetUrl("cbs-npf-main.css?v=18");
            manifest.DefineStyle("Style.PSS.Character.Certificate").SetUrl("pss-character-certificate.css");
            #endregion

            #region RSTVL Styles
            manifest.DefineStyle("RSTVL.Main").SetUrl("rstvl-main.css");
            #endregion

        }

        #endregion

        #region JS

        private void JsResources(Orchard.UI.Resources.ResourceManifest manifest)
        {
            manifest
                .DefineScript("CBS.MDA.EDIT.BILLING.Script")
                .SetUrl("cbs-mda-edit-billing-script.js")
                .SetDependencies("jQuery");

            manifest
                 .DefineScript("CBS.MDA.Script")
                 .SetUrl("cbs-mda-script.js?v=1")
                 .SetDependencies("jQuery");

            manifest
                 .DefineScript("CBS.MDA.Client.Script")
                 .SetUrl("cbs-mda-client-script.js")
                 .SetDependencies("jQuery");

            manifest
                 .DefineScript("CBS.MDA.RevenuHead.Script")
                 .SetUrl("cbs-mda-revenue-head-script.js")
                 .SetDependencies("jQuery");

            manifest
                 .DefineScript("CBS.MDA.General.Script")
                 .SetUrl("cbs-mda-g-script.js")
                 .SetDependencies("jQuery");


            manifest
                .DefineScript("CBS.Billing.Script")
                .SetUrl("cbs.billing.script.js");

            manifest
               .DefineScript("CBS.Report.Script")
               .SetUrl("cbs.reportscripts.js?v=2");

            manifest
               .DefineScript("CBS.Office.Web.Script")
               .SetUrl("cbs-web-update-1.js");

            manifest
                .DefineScript("CBS.Chart.JS")
                .SetUrl("Chart.min.js");

            manifest
                 .DefineScript("CBS.Dashboard.Chart")
                 .SetUrl("cbs.maindashboard.chart.js");

            manifest
                .DefineScript("CBS.MDA.Settings.Script")
                .SetUrl("cbs.mda.settings.js").SetDependencies("jQuery");

            manifest
            .DefineScript("CBS.MDA.Tax.Script")
            .SetUrl("payer-script.js").SetDependencies("jQuery");

            manifest
               .DefineScript("CBS.Element.Script")
               .SetUrl("element-browser.js").SetDependencies("jQuery");

            manifest
               .DefineScript("CBS.MDA.Discount.Script")
               .SetUrl("cbs-discount-script.js");

            manifest
              .DefineScript("CBS.MDA.Setting.Script")
              .SetUrl("cbs-settings-script.js").SetDependencies("jQuery");

            manifest
               .DefineScript("CBS.MDA.Client.DirectAssessment.Script")
               .SetUrl("cbs-client-direct-script.js").SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.Admin.Client.SearchInvoicePayment.Script")
                .SetUrl("cbs-client-searchinvoicepayment.js").SetDependencies("jQuery");


            manifest
              .DefineScript("CBS.Main.Script")
              .SetUrl("cbs-main.js");

            manifest
              .DefineScript("CBS.MakePayment.Script")
              .SetUrl("cbs-make-payment.js?v=5");

            manifest
             .DefineScript("CBS.Receipt.Script")
             .SetUrl("cbs-receipt-update-3.js?v=2");

            manifest
             .DefineScript("CBS.PAYE.Receipt.Script")
             .SetUrl("cbs-paye-receipt-update.js");

            manifest
             .DefineScript("CBS.Payments.Script")
             .SetUrl("cbs-payment.js?v=1");

            //css
            //scripts
            //Script.Require("CBS.Moments.Script").AtFoot();
            //Script.Require("CBS.Jsdelvr.Script").AtFoot();
            manifest
                .DefineScript("CBS.Moments.Script")
                .SetUrl("moment.min.js");

            manifest
               .DefineScript("CBS.DateRangePicker.Script")
               .SetUrl("daterangepicker.min.js");

            manifest
                .DefineScript("Payee.Assessment.Report.Script")
                .SetUrl("cbs-paye-assessment.js?v=8");

            manifest
                .DefineScript("Payee.Old.Assessment.Report.Script")
                .SetUrl("cbs-payee-old-assessment.js");

            manifest
            .DefineScript("OSGOF.CellSite.Report.Script")
            .SetUrl("cbs-cellsite-report-update-2.js");

            manifest
                .DefineScript("OSGOF.AddCellSite.Report.Script")
                .SetUrl("osgof-add-cellsite-client.js");

            manifest
                .DefineScript("OSGOF.ListCellSite.Report.Script")
                .SetUrl("osgof-list-cellsite-client.js");

            manifest
                .DefineScript("CBS.Self.Assessment.Script")
                .SetUrl("cbs-self-assessment.js?v=7");

            manifest
                .DefineScript("CBS.Fetch.Lgas.Script")
                .SetUrl("cbs-fetch-lgas-script.js");

            manifest.DefineScript("Payee.Script")
                .SetUrl("payee-script.js?v=5");

            manifest.DefineScript("Payee.Old.Script")
                .SetUrl("payee-old-script.js?v=1");

            manifest.DefineScript("OSGOF.Cell.Sites.Script")
               .SetUrl("osgof-cell-site-script-update-3.js");

            manifest.DefineScript("OSGOF.Cell.Sites.Upload.Client.Script")
               .SetUrl("osgof-cell-sites-upload-script.js");

            manifest.DefineScript("CBS.Reference.Data.Script")
               .SetUrl("reference-data-upload-script.js")
               .SetDependencies("jQuery");

            manifest
               .DefineScript("CBS.MDA.BootstrapDatepicker.Script")
               .SetUrl("bootstrap-datepicker.js")
               .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.Datepicker.Script")
                .SetUrl("datepickerinitialization.js")
                .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.BillingDatepicker.Script")
                .SetUrl("billing-datepicker.js")
                .SetDependencies("jQuery");

            manifest
              .DefineScript("EIRS.MDA.SettlementReportS.Script")
              .SetUrl("cbs-mda-eirs-settlementreport.js")
              .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.Bootstrap.Script")
                .SetUrl("bootstrap.min.js")
                .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.HighChart.Script")
                .SetUrl("highcharts.js");

            manifest
                .DefineScript("CBS.Qquery331")
                .SetUrl("jquery.min.js");

            // manifest
            //.DefineScript("jQuery")
            //.SetUrl("jquery-3.3.1.min.js").SetVersion("9.0.0");

            manifest
                .DefineScript("CBS.Popper.Script")
                .SetUrl("popper.min.js");


            manifest
                .DefineScript("CBS.MDA.CustomScript.Script")
                .SetUrl("custom_chart.js");

            manifest
                .DefineScript("CBS.MDA.PDF.Script")
                .SetUrl("jspdf.min.js");

            manifest
                .DefineScript("CBS.MDA.PDF_autoTable.Script")
                .SetUrl("jspdf.plugin.autotable.js");

            manifest
                .DefineScript("CBS.MDA.Excel_Export.Script")
                .SetUrl("excellentexport.min.js");

            manifest
                .DefineScript("CBS.MDA.HighChartsExport.Script")
                .SetUrl("HighChartsExport.js");

            manifest
                .DefineScript("CBS.MDA.RevenueReport.Script")
                .SetUrl("MdaReport.js")
                .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.NewDashboard.Script")
                .SetUrl("NewDashboard.js")
                .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.MDA.MonthPicker.Script")
                .SetUrl("monthpicker.js");

            manifest
                .DefineScript("CBS.MDA.AssessmentDatePicker.Script")
                .SetUrl("Assessment.js")
                .SetDependencies("jQuery");

            manifest
                .DefineScript("CBS.Operator.CellSites.Script")
                .SetUrl("cbs-operator-add-cellsites.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("Toastr").SetUrl("toastr.min.js");
            manifest.DefineScript("Plugin").SetUrl("plugin.js");

            manifest.DefineScript("DateTimePicker").SetUrl("DateTimePicker.js").SetDependencies("CBS.Qquery331", "CBS.MDA.Bootstrap.Script");

            manifest.DefineScript("TINRegistration").SetUrl("TINRegistration.js");
            manifest.DefineScript("ReportsJs").SetUrl("Reports.js");

            manifest.DefineScript("EregistryFormWizard").SetUrl("EregistryFormWizard.js").SetDependencies("Toastr", "CBS.Qquery331"); ;
            manifest.DefineScript("CBS.Bootstrap4.Script").SetUrl("bootstrap4.min.js").SetDependencies("jQuery");
            manifest.DefineScript("CBS.Admin.AddPayment.Script").SetUrl("cbs-add-payment-update-1.js");

            manifest.DefineScript("CBS.SelectTaxPayer.Confirm.Script").SetUrl("cbs-select-taxpayer.js").SetDependencies("jQuery");

            manifest.DefineScript("CBS.View.Invoice.Script").SetUrl("cbs-view-invoice.js").SetDependencies("jQuery");

            manifest.DefineScript("CBS.Tcc.Application.Form.Script").SetUrl("tcc-application-form.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("CBS.Reg.Business.Script").SetUrl("cbs-reg-business-script.js").SetDependencies("jQuery");
            manifest.DefineScript("CBS.Tcc.Req.History.Script").SetUrl("cbs-tcc-req-history-script.js").SetDependencies("jQuery");
            manifest.DefineScript("CBS.Assign.Payment.Provider.Script").SetUrl("cbs-assign-payment-provider.js?v=5").SetDependencies("jQuery") ;
            manifest.DefineScript("CBS.Payment.Provider.list.Script").SetUrl("cbs-payment-provider-list.js");
            manifest.DefineScript("CBS.Revenue.Head.Permissions.Constraints.Script").SetUrl("cbs-revenue-head-permissions-constraints.js?v=2");
            manifest.DefineScript("CBS.LGA.Partial.Script").SetUrl("cbs-lga-partial-script.js").SetDependencies("jQuery");
            manifest.DefineScript("CBS.MDA.Revenue.Access.Restrictions.Staging.Script").SetUrl("cbs-mda-revenue-access-restrictions-staging.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("CBS.TCC.Receipt.Utilization.Script").SetUrl("cbs-tcc-receipt-utilization.js?v=4");
            manifest.DefineScript("CBS.Revenue.Head.Permissions.Constraints.Script").SetUrl("cbs-revenue-head-permissions-constraints.js");
            manifest.DefineScript("CBS.PAYE.Schedule.List.Script").SetUrl("cbs-paye-schedule-list-script.js");
            manifest.DefineScript("CBS.PAYE.Batch.Items.List.Script").SetUrl("cbs-paye-batch-items-list.js");
            manifest.DefineScript("CBS.TAX.PAYER.Enumeration.Upload.Script").SetUrl("tax-payer-enumeration-upload-script.js?v=2");
            manifest.DefineScript("CBS.TAX.PAYER.Enumeration.Schedule.Result.Script").SetUrl("tax-payer-enumeration-schedule-result-script.js?v=2");
            manifest.DefineScript("CBS.State.LGA.By.Name.Script").SetUrl("cbs.state.lga.by.name.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("Tooltip.Toggling.Script").SetUrl("tooltip-toggling-script.js?v=2").SetDependencies("jQuery");

            //generate invoice confirm tax payer: Invoice controller
            manifest.DefineScript("CBS.GenerateInvoice.Admin.ConfirmTaxpayer.Script").SetUrl("cbs-generate-invoice-admin.js").SetDependencies("jQuery");

            //External Invoice Generation
            manifest.DefineScript("CBS.ExternalInvoiceGeneration.Script").SetUrl("cbs-invoice-generation-redirect.js");
            //CBS.SettlementRule.Script
            manifest.DefineScript("CBS.SettlementRule.Script").SetUrl("cbs.settlement.script.js?v=2");
            manifest.DefineScript("PSS.PCC.Country.Of.Residence.Script").SetUrl("cbs-pcc-country-of-residence.script.js?v=1");

            #region bvn validation mock script
            manifest.DefineScript("CBS.Bvn.Validation.Script").SetUrl("cbs-validate-bvn.js?v=1").SetDependencies("jQuery");
            #endregion


            #region NPF Scripts
            manifest.DefineScript("CBS.npf.main.Script").SetUrl("cbs-npf-main.js");
            manifest.DefineScript("PSS.Select.Service.Script").SetUrl("pss.select.service.js?v=5");
            manifest.DefineScript("CBS.State.LGA.Script").SetUrl("cbs.state.lga.js?v=4").SetDependencies("jQuery");
            manifest.DefineScript("CBS.LGA.Command.Script").SetUrl("pss.get.command.js?v=2");
            manifest.DefineScript("CBS.Admin.LGA.Command.Script").SetUrl("pss.admin.get.command.js?v=2");
            manifest.DefineScript("CBS.Admin.Reports.LGA.Commands.Script").SetUrl("pss.admin.reports.get.commands.js?v=2");
            manifest.DefineScript("CBS.Admin.Police.Officers.Script").SetUrl("pss.admin.get.officers.js?v=2");
            manifest.DefineScript("CBS.npf.extract.details.Script").SetUrl("cbs-npf-extract-details.js?v=10");
            manifest.DefineScript("PPS.Escort.Forms.Script").SetUrl("pss.escort.forms.js?v=6");
            manifest.DefineScript("PSS.DatePicker.Script").SetUrl("pss-date-picker.js?v=5").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Resend.Code.Script").SetUrl("pss.resend.code.js?v=2");
            manifest.DefineScript("PSS.User.Proile.Script").SetUrl("pss.user.profile.js?v=9");
            manifest.DefineScript("PSS.Contact.Us.Script").SetUrl("pss.contact.us.js");
            manifest.DefineScript("PSS.Verify.Account.Code.Script").SetUrl("pss-verify-account-code.js");
            manifest.DefineScript("PSS.User.Profile.Pwd.Script").SetUrl("pss.user.profile.pwd.js?v=3");
            manifest.DefineScript("PSS.Request.List.Script").SetUrl("pss.request.list.js?v=2");
            manifest.DefineScript("PSS.Request.Branch.List.Script").SetUrl("pss.request.branch.list.js?v=3");
            manifest.DefineScript("PSS.Extract.Category.Script").SetUrl("pss.extract.category.js?v=5");
            manifest.DefineScript("PSS.Resend.Password.Reset.Code.Script").SetUrl("pss.resend.password.reset.code.js?v=2");
            manifest.DefineScript("PSS.Admin.Approval.Request.List.Script").SetUrl("pss.admin.approval.request.list.js");
            manifest.DefineScript("PSS.Estimate.Calculator.Script").SetUrl("pss.estimate.calc.js?v=3");
            manifest.DefineScript("PSS.Home.Carousel.Script").SetUrl("pss.home.carousel.js?v=5");
            manifest.DefineScript("PSS.Security.Change.Password.Script").SetUrl("pss.security.change.password.js?v=5");
            manifest.DefineScript("PSS.User.Reg.Script").SetUrl("pss.user.reg.js?v=9").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Request.Details.Script").SetUrl("pss.request.details.js");
            manifest.DefineScript("PSS.Select.Service.Sub.Categories.Script").SetUrl("pss.select.service.sub.categories.js?v=3");
            manifest.DefineScript("PSS.Remember.Me.Script").SetUrl("pss.remember.me.js");
            manifest.DefineScript("PSS.Side.Note.Pop.Up.Script").SetUrl("pss.side.note.pop.up.js");
            manifest.DefineScript("PSS.Confirm.Request.Script").SetUrl("pss.confirm.request.js?v=2");
            manifest.DefineScript("PSS.Terms.Of.Use.Modal.Script").SetUrl("pss.terms.of.use.modal.js");
            manifest.DefineScript("PSS.Character.Certificate.Script").SetUrl("pss.character.certificate.js?v=8");
            manifest.DefineScript("PSS.Character.Certificate.Diaspora.Script").SetUrl("pss-character-certificate-diaspora.js?v=2");
            manifest.DefineScript("PSS.View.Blob.Script").SetUrl("pss.view.blob.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Multi.Select.Extract.Category.Script").SetUrl("pss.multi.select.extract.category.js?v=3");
            manifest.DefineScript("PSS.Sub.Users.List.Script").SetUrl("pss.sub.users.list.js?v=4");
            manifest.DefineScript("PSS.Branches.List.Script").SetUrl("pss.branches.list.js?v=2");
            manifest.DefineScript("PSS.Draft.Service.Document.Script").SetUrl("pss.draft.service.document.js?v=1");
            manifest.DefineScript("PSS.Assign.Tactical.Squad.Script").SetUrl("pss.assign.tactical.squad.js?v=8");
            manifest.DefineScript("PSS.Assign.Formation.Script").SetUrl("pss.assign.formation.js?v=10");
            manifest.DefineScript("PSS.Admin.UserCreation.Script").SetUrl("pss.admin.usercreation.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.LGA.Command.For.Admin.Script").SetUrl("pss.admin.get.command.for.admin.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.View.Allocated.Officers.Script").SetUrl("pss.admin.view.allocated.officers.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Formation.Approval.Comment.Script").SetUrl("pss.admin.formation.approval.comment.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Assign.Officers.Submit.Prompt.Script").SetUrl("pss.admin.assign.officers.submit.prompt.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("PSS.DCP.Assign.Formation.Script").SetUrl("pss.dcp.assign.formation.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.DCP.Get.Commands.Script").SetUrl("pss.dcp.get.commands.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Get.DPO.Partial.Script").SetUrl("pss.get.dpo.partial.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Get.Central.Registrar.Partial.Script").SetUrl("pss.get.central.registrar.partial.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Upload.Signature.Script").SetUrl("pss.admin.upload.signature.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.UserCreation.Edit.Script").SetUrl("pss.admin.usercreation.edit.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Add.Fee.Party.Script").SetUrl("pss.admin.add.fee.party.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Settlements.Edit.Parties.Script").SetUrl("pss.admin.settlements.edit.parties.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Change.Deployed.Officer.Script").SetUrl("pss.admin.change.deployed.officer.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Wallet.Configuration.Script").SetUrl("pss.add.wallet.configuration.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Wallet.Initiate.Request.Script").SetUrl("pss.admin.initiate.wallet.payment.js?v=3").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.User.Report.Script").SetUrl("pss.admin.user.report.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Assgin.Escort.Process.Flow.Script").SetUrl("pss.assign.escort.process.flow.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.PCC.Document.Upload.Preview.Partial.Script").SetUrl("pss.pcc.document.upload.preview.partial.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.PCC.Document.Upload.Confirmation.Modal.Script").SetUrl("pss.pcc.document.upload.confirmation.modal.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.PCC.Passport.Upload.Guidelines.Modal.Script").SetUrl("pss.pcc.passport.upload.guidelines.modal.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.PCC.ChangePhoto.Script.Script").SetUrl("pss.pcc.change.passport.photo.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Resend.Retrieve.Email.Code.Script").SetUrl("pss.resend.retrieve.email.code.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.State.Service.Command.Script").SetUrl("pss.get.state.command.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Upload.Branch.Sub.Users.Script").SetUrl("pss.admin.upload.branch.sub.users.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Branch.Sub.Users.Upload.Validation.Result.Script").SetUrl("pss.admin.branch.sub.users.upload.validation.result.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Upload.Branch.Officer.Script").SetUrl("pss.admin.upload.branch.officer.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Branch.Officer.Upload.Validation.Result.Script").SetUrl("pss.admin.branch.officer.upload.validation.result.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Branch.Officer.Upload.Generate.Request.Script").SetUrl("pss.admin.branch.officer.upload.generate.request.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Notification.Message.List.Script").SetUrl("pss.notification.message.list.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Generate.Request.Without.Officers.Upload.Script").SetUrl("pss.admin.generate.request.without.officers.upload.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Generate.Request.Without.Officers.Upload.Validation.Result.Script").SetUrl("pss.admin.generate.request.without.officers.upload.validation.result.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Generate.Escort.Request.Without.Officers.Script").SetUrl("pss.admin.generate.escort.request.without.officers.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Initiate.Deployment.Allowance.Payment.Script").SetUrl("pss.admin.initiate.deployment.allowance.payment.js?v=2").SetDependencies("jQuery");
            manifest.DefineScript("PSS.PCC.Change.Bio.Data.Page.Script").SetUrl("pss.pcc.change.bio.data.page.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Disable.Submit.Btn.On.Click.Script").SetUrl("pss.admin.disable.submit.btn.on.click.js?v=1").SetDependencies("jQuery");
            manifest.DefineScript("PSS.Admin.Account.Wallet.Payment.Approval.View.Details.Script").SetUrl("pss.admin.account.wallet.payment.approval.view.details.js?v=1").SetDependencies("jQuery");
            #endregion

            #region POSSAP Scheduler Scripts
            manifest.DefineScript("POSSAP.Scheduler.Police.Officer.Report.Script").SetUrl("possap-scheduler-police-officer-report-script.js?v=2");
            #endregion
        }

        #endregion


        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();


            #region CSS Resources

            CSSResources(manifest);

            #endregion


            #region JS Resources

            JsResources(manifest);

            #endregion
        }


    }
}