using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Collections.Generic;

namespace Parkway.CBS.Module.API
{
    public class ApiRoutes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new RouteDescriptor[] {
                 new HttpRouteDescriptor {
                    Name = "payePaymentNotif",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/paye-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "PayePaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "createMDAv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/mda/create",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "MDA",
                        action = "CreateMDA"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "editMDAv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/mda/edit",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "MDA",
                        action = "EditMDA"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "createRevenueHeadv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/revenuehead/create",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "RevenueHead",
                        action = "CreateRevenueHead"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "editRevenueHeadv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/revenuehead/edit",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "RevenueHead",
                        action = "EditRevenueHead"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "createBillingv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/billing/create",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Biling",
                        action = "CreateBilling"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "editBillingv1",
                    Priority = -10,
                    RouteTemplate = "api/v1/billing/edit",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Billing",
                        action = "EditBilling"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "createInvoicev1",
                    Priority = -10,
                    RouteTemplate = "api/v1/invoice/create",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "CreateInvoice"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "GenerateInvoicev1",
                    Priority = -10,
                    RouteTemplate = "api/v1/invoice/create/multiple",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "GenerateInvoice"
                    }
                },
                new HttpRouteDescriptor
                {
                    Name = "appregistercbsuser",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/register",
                    Defaults = new
                    {
                        area = "Parkway.CBS.Module.API",
                        controller = "User",
                        action = "RegisterCBSUser"
                    }
                },
                 new HttpRouteDescriptor
                {
                    Name = "search-by-filter",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/search-by-filter",
                    Defaults = new
                    {
                        area = "Parkway.CBS.Module.API",
                        controller = "User",
                        action = "SearchByFilter"
                    }
                },
                new HttpRouteDescriptor
                {
                    Name = "processpaye",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/process-paye",
                    Defaults = new
                    {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "ProcessPaye"
                    }
                },
                new HttpRouteDescriptor
                {
                    Name = "apicollectionreport",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/collection-report",
                    Defaults = new
                    {
                        area = "Parkway.CBS.Module.API",
                        controller = "Report",
                        action = "CollectionReport"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "paydirect",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/pay-direct",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "PayDirectPaymentProcessing"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "paydirectflat",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/pay-direct/flat",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "PayDirectFlatPaymentProcessing"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "validateInvoiceForPayment",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/validate-invoice",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "ValidateInvoiceNumber"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "Validate",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/validate",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "Validate"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "validateInvoiceForPaymentFlat",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/validate-invoice/flat",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "ValidateInvoiceNumberFlat"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "PaymentNotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/payment-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "PaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "ValidateInvoiceNumberNIBSS",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/nibss-ebills/validate-invoice",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "ValidateInvoiceNumberNIBSS"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "NIBSSEBillsPayPaymentNotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/nibss-ebills/payment-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "NIBSSEBillsPayPaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "NIBSSEBillsPayPaymentNotificationV2",
                    Priority = -10,
                    RouteTemplate = "api/v2/payment/nibss-ebills/payment-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Paymentv2",
                        action = "PaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "ResetNIBSSIntegrationCredentials",
                    Priority = -10,
                    RouteTemplate = "api/v1/nibss-email-notification/reset",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "NIBSSEmailNotification",
                        action = "SendNIBSSIntegrationCredentials"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "NIBSSEBillsValidationV2",
                    Priority = -10,
                    RouteTemplate = "api/v2/invoice/nibss-ebills/validate-invoice",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoicev2",
                        action = "Validation"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "TestIntegration",
                    Priority = -10,
                    RouteTemplate = "api/v1/integration/test",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Integration",
                        action = "Test"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "TripleDESEncryption",
                    Priority = -10,
                    RouteTemplate = "api/v1/integration/encryption/encrypt-this",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Integration",
                        action = "EncryptThis"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "createInvoicev3",
                    Priority = -10,
                    RouteTemplate = "api/v1/invoice/batchresponse",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "BatchResponse"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "NetPayPaymentNotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/netpay/payment-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "NetPayPaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "Notification",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/notification",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Payment",
                        action = "Notification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "InvoiceStatus",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/status",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "QueryStatus"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "CancelInvoice",
                    Priority = -10,
                    RouteTemplate = "api/v1/{controller}/invalidate",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "Invoice",
                        action = "Invalidate"
                    }
                },

                new HttpRouteDescriptor {
                    Name = "InitializeBatch",
                    Priority = -10,
                    RouteTemplate = "api/v1/PAYE/initialize-batch",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "PayeAPI",
                        action = "InitializeBatch"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "ValidateBatchItems",
                    Priority = -10,
                    RouteTemplate = "api/v1/paye/validate-batch",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "PAYEValidation",
                        action = "ValidateBatch"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "AddBatchItems",
                    Priority = -10,
                    RouteTemplate = "api/v1/PAYE/add-batch-items",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "PayeAPI",
                        action = "AddBatchItems"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "statetincreation",
                    Priority = -10,
                    RouteTemplate = "api/v1/statetin/create",
                    Defaults = new {
                        area = "Parkway.CBS.Module.API",
                        controller = "StateTIN",
                        action = "CreateStateTIN"
                    }
                },
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }
    }
}