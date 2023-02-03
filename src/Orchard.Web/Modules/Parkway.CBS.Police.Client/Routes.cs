using Orchard.Mvc.Routes;
using Parkway.CBS.Police.Client.RouteName;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Client
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var npfRoutes = new[]
            {
                new RouteDescriptor {
                     Name = "P.Index",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Index"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "P.SelectService",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/select-service", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "SelectService"},
                            {"action", "SelectService"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "P.RequestUserProfile",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-user-profile", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "UserProfile"},
                            {"action", "RequestUserProfile"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "P.ConfirmUserProfile",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-confirm-user-profile", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "UserProfile"},
                            {"action", "ConfirmUserProfile"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = "P.ExtractRequest",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-extract", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSExtract"},
                            {"action", "ExtractRequest"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your moduleP.Escort.Request
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.Escort.Request",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-escort", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscort"},
                            {"action", "EscortRequest"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.GetCommandByLGA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-commands-in-lga", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "CommandsByLGA"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.GetAreaAndDivisionalCommandByLGA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-area-divisional-commands-in-lga", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "GetAreaAndDivisionalCommandsByLGA"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                                                                                                                                                  },
                        new MvcRouteHandler())
                },
                     new RouteDescriptor {
                     Name = "P.X.GetCommandsByState",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-commands-in-state", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "CommandsByState"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                     new RouteDescriptor {
                     Name = "P.Request.Confirm",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-confirmation", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestConfirmation"},
                            {"action", "ConfirmPSSRequest"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "P.PaymentReference.Number",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/getreferencenumber", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "MakePayment"},
                            {"action", "GetReferenceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },                      new RouteDescriptor {
                     Name = "P.Signout",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/sign-out", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Signout"},
                            {"action", "Signout"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
               new RouteDescriptor {
                     Name = "P.Make.Payment",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/make-payment/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "MakePayment"},
                            {"action", "MakePayment"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = Login.SignIn,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/login", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Login"},
                            {"action", "SignIn"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                new RouteDescriptor {
                    Name = "P.Register.User",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/signup", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Registration"},
                            {"action", "RegisterUser"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                new RouteDescriptor {
                    Name = "P.Verify.Account",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/verify-account", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "AccountVerification"},
                            {"action", "VerifyAccount"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                 new RouteDescriptor {
                    Name = "P.Resend.Verification.Code",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/resend-verification-code", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "AccountVerification"},
                            {"action", "ResendVerificationCode"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                  new RouteDescriptor {
                    Name = "P.PaymentClient.Notification",
                    Priority = 5,
                    Route = new Route(
                       "p/notify/{paymentRef}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "MakePayment"},
                            {"action", "Notify"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = ChangePassword.ShowChangePassword,
                    Priority = 5,
                    Route = new Route(
                       "p/change-password",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ChangePassword"},
                            {"action", "ChangePassword"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.BIN.Search",
                    Priority = 5,
                    Route = new Route(
                       "p/search-for-bin",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PssInvoice"},
                            {"action", "SearchByInvoiceNumber"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.Invoice.ReceiptDetails",
                    Priority = 5,
                    Route = new Route(
                       "p/invoice/receipts/{invoiceNumber}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Receipt"},
                            {"action", "Receipts"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.ReceiptDetails",
                    Priority = 5,
                    Route = new Route(
                       "p/receipt-details/{invoiceNumber}/{receiptNumber}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Receipt"},
                            {"action", "ReceiptDetails"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.Generate.Receipt",
                    Priority = 5,
                    Route = new Route(
                       "p/generate-receipt/{invoiceNumber}/{receiptNumber}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Receipt"},
                            {"action", "GetReceipt"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.ViewInvoice",
                    Priority = 5,
                    Route = new Route(
                       "p/view-invoice/{BIN}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PssInvoice"},
                            {"action", "GetInvoice"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.ErrorPage.Error404,
                    Priority = 5,
                    Route = new Route(
                       "p/error/404",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Error"},
                            {"action", "Error404"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "P.About",
                    Priority = 5,
                    Route = new Route(
                       "p/about",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "About"},
                            {"action", "About"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "P.Contact",
                    Priority = 5,
                    Route = new Route(
                       "p/contact",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Contact"},
                            {"action", "Contact"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.Error.Page",
                    Priority = 5,
                    Route = new Route(
                       "p/error",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "Error"},
                            {"action", "ErrorPage"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RequestList.ShowRequestList,
                    Priority = 5,
                    Route = new Route(
                       "p/request-list",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestList"},
                            {"action", "RequestList"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = SubUsersRequestList.ShowSubUsersRequestList,
                    Priority = 5,
                    Route = new Route(
                       "p/branch/request-list",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(SubUsersRequestList)},
                            {"action", nameof(SubUsersRequestList)},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "P.RequestListMoveRight",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/get-next-request-list-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestList"},
                            {"action", "RequestListMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = SubUsersRequestList.SubUsersRequestListMoveRight,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/branch/get-next-request-list-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(SubUsersRequestList)},
                            {"action", nameof(SubUsersRequestList.SubUsersRequestListMoveRight)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = "P.X.Extract.Sub.Categories",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-extract-sub-categories", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSExtractSubCategoryAJAX"},
                            {"action", "ExtractSubCategories"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = "P.Forgot.Password",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/forgot-password", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ForgotPassword"},
                            {"action", "ForgotPassword"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = ResetPasswordVerification.ResetPasswordVerify,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/reset-password-verify", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ResetPasswordVerification"},
                            {"action", "ResetPasswordVerify"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = "P.Reset.Password",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/reset-password", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ResetPassword"},
                            {"action", "ResetPassword"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = "P.Resend.Password.Reset.Verification.Code",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/resend-password-reset-verification-code", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ResetPasswordVerification"},
                            {"action", "ResendVerificationCode"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = "P.Calculate.Estimate",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/get-estimate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscort"},
                            {"action", "CalculateEstimate"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                  new RouteDescriptor {
                    Name = "P.Generic.Police.Request",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "GenericPoliceRequest"},
                            {"action", "GenericRequest"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = Client.RouteName.PrivacyTerms.PrivacyPolicy,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/privacy-policy", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PrivacyTerms"},
                            {"action", "PrivacyTerms"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RouteName.TermsOfUse.ShowTermsOfUse,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/terms-of-use", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "TermsOfUse"},
                            {"action", "TermsOfUse"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RequestDetails.ShowRequestDetails,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-details/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestDetails"},
                            {"action", "RequestDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RequestDetails.ShowBranchRequestDetails,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/branch/request-details/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestDetails"},
                            {"action", "RequestBranchDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RouteName.ValidateDocument.ShowValidateDocument,
                    Priority = 5,
                    Route = new Route(
                       "p/validate-document",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ValidateDocument"},
                            {"action", "ValidateDocument"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RouteName.ValidateDocument.ValidatedDocumentInfo,
                    Priority = 5,
                    Route = new Route(
                       "p/validate-document/{approvalNumber}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ValidateDocument"},
                            {"action", "ValidatedDocumentInfo"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = "P.X.PSS.Sub.Categories",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-pss-sub-categories", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "SelectServiceAJAX"},
                            {"action", "PSSSubCategories"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = "P.X.PSS.Sub.Sub.Categories",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-pss-sub-Sub-categories", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "SelectServiceAJAX"},
                            {"action", "PSSSubSubCategories"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = SelectServiceAJAX.GetServicesPerCategory,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-service-per-category", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "SelectServiceAJAX"},
                            {"action", nameof(SelectServiceAJAX.GetServicesPerCategory)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.Request.Invoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-invoice/{fileNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestInvoices"},
                            {"action", "RequestInvoices"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "P.Request.Invoice.View",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-invoice-view/{bin}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestInvoices"},
                            {"action", "GetInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Contact.Us",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/contact-us", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ContactUsAJAX"},
                            {"action", "ContactUs"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },

                    new RouteDescriptor {
                     Name = "P.X.Validate.Identification.Number",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/validate-identification-number", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ValidateIdentificationNumberAJAX"},
                            {"action", "ValidateIdentificaitonNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Get.Identification.Tyoes",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-identification-types", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "ValidateIdentificationNumberAJAX"},
                            {"action", "GetIdentificationTypes"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = PSSCharacterCertificate.CharacterCertificateRequest,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-character-certificate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(PSSCharacterCertificate)},
                            {"action", nameof(PSSCharacterCertificate.CharacterCertificateRequest)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your moduleP.Escort.Request
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Get.Escort.Service.Category.Types",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-escort-service-category-types", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscortDetailsAJAX"},
                            {"action", "GetServiceCategoryTypes"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSSubUser.SubUsers,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/sub-users", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSSubUser"},
                            {"action", "SubUsers"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSBranches.Branches,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/branches", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSBranches"},
                            {"action", "Branches"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSBranches.CreateBranch,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/branches/create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSBranches"},
                            {"action", "CreateBranch"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSBranchesAJAX.PSSBranchesMoveRight,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/branches/move-right", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSBranchesAJAX"},
                            {"action", "PSSBranchesMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSSubUser.CreateSubUser,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/sub-users/create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSSubUser"},
                            {"action", "CreateSubUser"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSSubUserAJAX.PSSSubUsersMoveRight,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/sub-users/move-right", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSSubUserAJAX"},
                            {"action", "PSSSubUsersMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSSubUserAJAX.ToggleSubUserStatus,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/sub-users/toggle-status", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller",nameof(PSSSubUserAJAX)},
                            {"action", nameof(PSSSubUserAJAX.ToggleSubUserStatus)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = RequestDetails.ViewServiceDocument,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-details/view/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RequestDetails"},
                            {"action", "ViewServiceDocument"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Get.Escort.Tactical.Squads",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-escort-tactical-squads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscortDetailsAJAX"},
                            {"action", "GetTacticalSquads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Get.Next.Level.Commands",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-next-level-commands", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscortDetailsAJAX"},
                            {"action", "GetNextLevelCommandsWithCode"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = "P.X.Get.Formations.For.State",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-formations-for-state", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "PSSEscortDetailsAJAX"},
                            {"action", "GetStateFormations"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = RetrieveEmail.RetrieveEmailAction,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/retrieve-email", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RetrieveEmail"},
                            {"action", "RetrieveEmail"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = RetrieveEmailVerification.RetrieveEmailVerify,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/retrieve-email-verify", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RetrieveEmailVerification"},
                            {"action", "RetrieveEmailVerify"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = RetrieveEmailVerification.ResendVerificationCode,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/resend-retrieve-email-verification-code", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", "RetrieveEmailVerification"},
                            {"action", "ResendVerificationCode"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area","Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                   new RouteDescriptor {
                    Name = RequestServiceStateCommandsAJAX.GetCommands,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/x/get-request-service-state-commands", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(RequestServiceStateCommandsAJAX)},
                            {"action",  nameof(RequestServiceStateCommandsAJAX.GetCommands)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                     Name = ServiceOptions.SelectOption,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/service-options", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(ServiceOptions)},
                            {"action", nameof(ServiceOptions.SelectOption)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = PSSCharacterCertificateDiaspora.CharacterCertificateDiasporaRequest,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/request-character-certificate-diaspora", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(PSSCharacterCertificateDiaspora)},
                            {"action", nameof(PSSCharacterCertificateDiaspora.CharacterCertificateDiasporaRequest)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                     Name = NotificationMessage.List,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "p/notification-message-list", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}, // this is the name of your module
                            {"controller", nameof(NotificationMessage)},
                            {"action", nameof(NotificationMessage.List)},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Client"}
                        },
                        new MvcRouteHandler())
                },
            };

            return npfRoutes;
        }
    }
}