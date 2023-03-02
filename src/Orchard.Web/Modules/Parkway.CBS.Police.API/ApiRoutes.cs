using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Collections.Generic;

namespace Parkway.CBS.Police.API
{
    public class ApiRoutes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new RouteDescriptor[] {
                new HttpRouteDescriptor {
                    Name = "P.API.Update.ProcessingFeeStatus",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/fee-confirmation",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "Request",
                        action = "ProcessingFeeConfirmation"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "P.API.USSD.ApprovalRequest",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/approval",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "USSDRequest",
                        action = "Approval"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "P.API.Notification.DeploymentAllowance.PaymentStatus",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/notification/deployment-allowance-payment-status",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "AllowanceSettlement",
                        action = "UpdateStatus"
                    }
                },
                  new HttpRouteDescriptor {
                    Name = "P.API.Character.Certificate.Details",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/details/{fileNumber}/{token}",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "CharacterCertificate",
                        action = "GetCharacterCertificateDetails"
                    }
                },
                  new HttpRouteDescriptor {
                    Name = "P.API.Character.Certificate.Biometrics",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/biometrics",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "CharacterCertificate",
                        action = "SubmitBiometrics"
                    }
                },
                  new HttpRouteDescriptor {
                    Name = "P.API.Payment.Settlement.Engine",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/account-wallet-payment-settlement-engine-call-back/callback",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "AccountWalletPaymentSettlementEngine",
                        action = "PaymentRequestStatusCallBack"
                    }
                },
                  new HttpRouteDescriptor {
                    Name = "P.API.Login.Signin",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/signin",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "Login",
                        action = "SignIn"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "P.API.ShortCodeSMS.ContentUpdate",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/content",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "ShortCodeSMS",
                        action = "ContentUpdate"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "P.API.RegularizationWithoutOfficers.DeploymentAllowance.Payment.Notification",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/deployment-allowance-settlement-engine-payment-callback",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngine",
                        action = "PaymentRequestStatusCallBack"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "P.API.EGS.Regularization.Recurring.Invoice.Payment.Confirmation",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/egs-regularization-recurring-invoice-payment-confirmation",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "RegularizationRecurringInvoice",
                        action = "ProcessingFeeConfirmation"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "P.API.ProxyAuthentication.Signin",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/signin",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "ProxyAuthentication",
                        action = "SignIn"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "P.API.ProxyAuthentication.ValidateUserName",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/validate-username",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "ProxyAuthentication",
                        action = "ValidateUserName"
                    }
                },
                  new HttpRouteDescriptor {
                    Name = "P.API.PSSExtract.GetPoliceExtractFormData",
                    Priority = -10,
                    RouteTemplate = "api/v1/pss/{controller}/formData",
                    Defaults = new {
                        area = "Parkway.CBS.Police.API",
                        controller = "PSSExtract",
                        action = "GetPSSFormData"
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