using Orchard.Mvc.Routes;
using Parkway.CBS.Client.Web.RouteName;
using Parkway.CBS.Module.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;


namespace Parkway.CBS.Client.Web
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var clientRoutes = new[] {

                new RouteDescriptor {
                    Name = "C.C.X",
                    Priority = 5,
                    Route = new Route(
                       "c/ccx",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "DoForm"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = PAYEWithScheduleFileUpload.PAYEFileUpload,
                    Priority = 5,
                    Route = new Route(
                       "c/paye-file-upload",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithScheduleFileUpload"},
                            {"action", "PAYEFileUpload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = PAYEWithNoScheduleUploadFileUpload.NoScheduleFileUpload,
                    Priority = 5,
                    Route = new Route(
                       "c/paye-no-schedule-option/file-upload",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithNoScheduleUploadFileUpload"},
                            {"action", "NoScheduleFileUpload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = PAYEWithScheduleOnscreen.PAYEOnscreen,
                    Priority = 5,
                    Route = new Route(
                       "c/paye-onscreen",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithScheduleOnscreen"},
                            {"action", "PAYEOnscreen"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = PAYEWithNoScheduleUploadOnscreen.NoScheduleOnscreen,
                    Priority = 5,
                    Route = new Route(
                       "c/paye-no-schedule-option/onscreen",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithNoScheduleUploadOnscreen"},
                            {"action", "NoScheduleOnscreen"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYE.SelectPAYEOption,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/select-paye-option", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYE"},
                            {"action", "SelectPAYEOption"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = PAYENoSchedule.NoScheduleOption,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-no-schedule-option", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYENoSchedule"},
                            {"action", "NoScheduleOption"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PAYEWithSchedule.WithScheduleOption,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-with-schedule-option", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithSchedule"},
                            {"action", "WithScheduleOption"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                        new RouteDescriptor {
                     Name = PAYENoScheduleUpload.NoScheduleUploadOption,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-no-schedule-option/schedule", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYENoScheduleUpload"},
                            {"action", "NoScheduleUploadOption"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PAYEWithSchedule.ShowScheduleResult,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-schedule-results", // this is the name of the page urlShowScheduleResult
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithSchedule"},
                            {"action", "ShowScheduleResult"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                     new RouteDescriptor {
                     Name = PAYENoScheduleUpload.NoScheduleUploadShowScheduleResult,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-no-schedule-option/schedule-upload-results", // this is the name of the page urlShowScheduleResult
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYENoScheduleUpload"},
                            {"action", "NoScheduleUploadShowScheduleResult"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                     new RouteDescriptor {
                     Name = "C.RegisterBusiness",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/register-business", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "RegisterBusiness"},
                            {"action", "RegisterBusiness"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TCCApplication.TccApplication,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tcc-application", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCApplication"},
                            {"action", "TCCApplication"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = StateTINValidation.ValidateStateTIN,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/validate-state-tin", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "ValidateStateTin"},
                            {"action", "ValidateStateTin"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = StateTINValidation.StateTINDetails,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/state-tin-details/{stateTIN}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "ValidateStateTin"},
                            {"action", "StateTINDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "C.BvnValidation",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/bvn-validation", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "BvnValidation"},
                            {"action", "BvnValidation"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "C.Ajax.ValidateBvn",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/validate-bvn", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "AjaxValidateBvn"},
                            {"action", "ValidateBvn"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYEWithScheduleAJAX.GetProcessPercentage,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/paye-get-process-percentage", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithScheduleAJAX"},
                            {"action", "GetProcessPercentage"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = PAYEWithScheduleAJAX.GetReportData,
                    Priority = 5,
                    Route = new Route(
                       "c/x/get-paye-report-data",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithScheduleAJAX"},
                            {"action", "GetReportData"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = PAYEWithScheduleAJAX.GetPAYEMoveRight,
                    Priority = 5,
                    Route = new Route(
                       "c/x/get-paye-next-page",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEWithScheduleAJAX"},
                            {"action", "GetPAYEMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = TCCRequestHistory.RequestHistory,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/requests-history", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCRequestHistory"},
                            {"action", "RequestHistory"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "C.Tax.Receipt.Utilizations",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-receipt-utilizations", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilization"},
                            {"action", "List"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxReceiptUtilization.ReceiptUtilization,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-receipt-utilization/{scheduleBatchRef}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilization"},
                            {"action", "ReceiptUtilization"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxReceiptUtilization.GenerateInvoice,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-receipt-utilization/generate-invoice/{batchRef}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilization"},
                            {"action", "GenerateInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxReceiptUtilization.AJAXPAYEReceipt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-paye-receipt", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilizationAJAX"},
                            {"action", "GetPAYEReceipt"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxReceiptUtilization.AJAXApplyPAYEReceipt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/apply-paye-receipt", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilizationAJAX"},
                            {"action", "ApplyReceiptToBatch"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxReceiptUtilization.AJAXBatchOutstandingAmt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-outstanding-amount", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxReceiptUtilizationAJAX"},
                            {"action", "GetOutstandingAmount"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "C.Ajax.Get.TCC.Requests",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-next-tcc-requests", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCRequestHistoryAjax"},
                            {"action", "RequestHistoryMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemReceiptValidation.ValidatePAYEBatchItemReceipt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-tax-receipt-validate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceiptValidation"},
                            {"action", "ValidateReceipt"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemReceiptValidation.PAYEBatchItemReceiptDetails,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye/batchitem-receipt-details/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceiptValidation"},
                            {"action", "ReceiptDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemReceiptValidation.PAYEBatchItemReceipt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-tax-receipt", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceipt"},
                            {"action", "Receipt"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemReceiptValidation.GeneratePAYEBatchItemReceipt,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/generate-paye-batch-item-receipt/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceiptValidation"},
                            {"action", "GetReceipt"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = RouteName.TCCValidation.ValidateTCC,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/validate-tcc", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCValidation"},
                            {"action", "ValidateTCC"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = TCCValidation.TCCDetails,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tcc-details/{applicationNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCValidation"},
                            {"action", "TCCDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = TCCValidation.DownloadFile,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tcc-file-download", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCValidation"},
                            {"action", "TCCAttachmentDownload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYEBatchItemReceipt.PAYEBatchItemReceipts,
                    Priority = 5,
                    Route = new Route(
                       "c/paye/batchitem-receipts",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceipt"},
                            {"action", "Receipts"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYEBatchItemReceipt.PAYEBatchItemReceiptsNavigation,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/paye/get-next-receipt-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemReceiptAJAX"},
                            {"action", "ReceiptsMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-next-receipt-page
                },

                 new RouteDescriptor {
                     Name = TCCApplication.ValidatePayerId,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/validate-payerid", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCApplicationAJAX"},
                            {"action", "ValidatePayerId"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TCCApplication.ValidateDevelopmentLevyInvoice,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/validate-development-levy-invoice", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TCCApplicationAJAX"},
                            {"action", "ValidateDevelopmentLevyInvoiceStatus"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEReceiptUtilizationReport.Receipts,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye/receipts", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEReceiptUtilizationReport"},
                            {"action", "Receipts"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEReceiptUtilizationReport.ReceiptUtilzations,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-receipt-utilizations/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEReceiptUtilizationReport"},
                            {"action", "ReceiptUtilizations"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYEReceiptUtilizationReport.PAYEReceiptsNavigation,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/paye/receipts/get-next-receipt-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEReceiptAJAX"},
                            {"action", "ReceiptsMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-next-receipt-page
                },
                new RouteDescriptor {
                    Name = PAYESchedule.PAYESchedules,
                    Priority = 5,
                    Route = new Route(
                       "c/paye/schedules",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYESchedule"},
                            {"action", "PAYESchedules"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYESchedule.PAYEScheduleUtilizedReceipts,
                    Priority = 5,
                    Route = new Route(
                       "c/paye/schedules/utilized-receipts/{batchRef}",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYESchedule"},
                            {"action", "PAYEScheduleUtilizedReceipts"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYESchedule.PAYESchedulesNavigation,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/paye/get-next-schedules-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEScheduleAJAX"},
                            {"action", "BatchRecordsMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-next-receipt-page
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemSearch.PAYEBatchValidation,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-batch-search", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemSearch"},
                            {"action", "PAYEBatchValidation"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = PAYEBatchItemSearch.PAYEBatchItems,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye/{batchRef}/batch-items", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemSearch"},
                            {"action", "PAYEBatchItems"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PAYEBatchItemSearch.PAYEBatchItemsNavigation,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/paye/batch-items/get-next-batch-items", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEBatchItemSearchAJAX"},
                            {"action", "BatchItemsMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-next-receipt-page
                },
                 new RouteDescriptor {
                     Name = PAYEScheduleValidation.ValidatePAYESchedule,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/paye-schedule-validate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "PAYEScheduleValidation"},
                            {"action", "ValidatePAYESchedule"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumeration.UploadSchedule,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-payer-enumeration-schedule-upload", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumeration"},
                            {"action", "UploadSchedule"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumeration.ShowScheduleResult,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-payer-enumeration-schedule-upload-result", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumeration"},
                            {"action", "ShowScheduleResult"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumeration.OnScreenUrl,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-payer-enumeration-schedule-upload-onscreen", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumeration"},
                            {"action", "OnScreen"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumeration.FileUploadUrl,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/tax-payer-enumeration-schedule-upload-file", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumeration"},
                            {"action", "FileUpload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumerationAJAX.CheckIfEnumerationUploadCompleted,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/tax-payer-enumeration-schedule-completion-status", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumerationAJAX"},
                            {"action", "CheckIfEnumerationUploadCompleted"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumerationAJAX.GetReportData,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/tax-payer-enumeration-schedule-report-data", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumerationAJAX"},
                            {"action", "GetReportData"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = TaxPayerEnumerationAJAX.GetEnumerationLineItemsMoveRight,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-tax-payer-enumeration-line-items-next-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"}, // this is the name of your module
                            {"controller", "TaxPayerEnumerationAJAX"},
                            {"action", "GetEnumerationLineItemsMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.Client.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };

            var routes = BaseRoutes.GetRoutes("", "Client");
            routes.AddRange(clientRoutes);
            return routes;

        }
    }
}