using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Module.Web
{
    public static class BaseRoutes
    {
        public static List<RouteDescriptor> GetRoutes(string prefix, string clientPrefix)
        {
            return new List<RouteDescriptor> {
                //C.HomePage
                new RouteDescriptor {
                    Name = prefix + "C.HomePage",
                    Priority = 5,
                    Route = new Route(
                       "",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Index"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "C.ShowDownloads",
                    Priority = 5,
                    Route = new Route(
                       "c/downloads",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "ShowDownloadsFiles"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },       
               new RouteDescriptor {
                    Name = prefix + "Payment.Notification",
                    Priority = 5,
                    Route = new Route(
                       "c/notify/{paymentRef}",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Notify"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "C.About",
                    Priority = 5,
                    Route = new Route(
                       "c/about",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "About"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "C.ProcessNetPay",
                    Priority = 5,
                    Route = new Route(
                       "c/process-netpay",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "GetNetPayToken"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "C.ContactUs",
                    Priority = 5,
                    Route = new Route(
                       "c/contact",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Contact"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = prefix + "Confirm.Bill",
                    Priority = 5,
                    Route = new Route(
                       "c/confirm-bill",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "ConfirmBill"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "CBS.User.Register",
                    Priority = 5,
                    Route = new Route(
                       "c/register",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "RegisterUser"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = prefix + "BIN.Search",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/search-for-bin", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "SearchByInvoiceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name= prefix + "C.SelfAssessment",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/generate-invoice", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "GenerateInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = prefix + "C.InvoiceProceed",
                    Priority = 5,
                    Route = new Route(
                       "c/invoice-bill",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "InvoiceProceed"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = prefix + "C.MakePayment",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/make-payment", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "MakePayment"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = prefix + "C.MakePayment.Invoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/make-payment/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "MakePaymentByInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = prefix + "C.Check.Requires.Login",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/check-category", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "CheckIfRequiresLogin"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = prefix + "C.PayerProfile",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/payer-profile", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "TaxProfile"},
                            {"action", "PayerProfile"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = prefix + "C.User.Login",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/user-login", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "UserLogin"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },new RouteDescriptor {
                    Name = prefix + "C.Signout",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/sign-out", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Signout"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },new RouteDescriptor {
                    Name = prefix + "C.SignIn",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/sign-in", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "SignIn"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "PaymentReference.Number",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/getreferencenumber", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetReferenceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = prefix + "C.ViewInvoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/view-invoice/{BIN}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "GetInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = prefix + "C.ViewReceipt",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/view-receipt/{ReceiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "GetReceipt"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = prefix + "C.AJX.GetProfiles",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-tax-entities-bycategory", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetTaxEntitiesByCategory"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },  new RouteDescriptor {
                    Name= prefix + "C.AJX.GetLgas",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-lgas-bystates", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetLgasByStates"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = prefix + "C.Invoice.ThirdParty.Redirect",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/external-redirect", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "RedirectForInvoiceGeneration"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + "CBS.Payments",
                    Priority = 5,
                    Route = new Route(
                       "c/payments",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Payments"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = prefix + "C.PaymentListMoveRight",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/get-next-payment-list-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "PaymentListMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = prefix + "Receipt.Search",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/search-for-receipt", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "SearchByReceiptNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = prefix + "C.ReceiptDetails",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/receipt-details/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "ViewReceiptDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = prefix + "C.Invoice.ReceiptDetails",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/invoice/receipts/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "Receipt"},
                            {"action", "InvoiceNumberReceipts"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = prefix + RouteName.InvoiceConfirmation.ConfirmInvoice,
                    Priority = 5,
                    Route = new Route(
                       "c/confirm-invoice",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"}, // this is the name of your module
                            {"controller", "InvoiceConfirmation"},
                            {"action", "ConfirmInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.{clientPrefix}.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}