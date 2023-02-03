using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Collections.Generic;

namespace Parkway.CBS.Tenant.Bridge
{
    public class ApiRoutes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new RouteDescriptor[] {
                new HttpRouteDescriptor {
                    Name = "readycashinvoicevalidation",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/invoice/rdc/validate-invoice",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "Bridge",
                        action = "ValidateInvoiceReadyCash"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "readycashpaymentnotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/payment/rdc/notification",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "PaymentBridge",
                        action = "ReadycashPaymentNotification"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "bridgeinvoicevalidation",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/invoice/validation",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "Bridge",
                        action = "ValidateInvoice"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "bridgepaymentnotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/payment/notification",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "PaymentBridge",
                        action = "PaymentNotification"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "bridgereadycashrequery",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/payment/rdc/requery-ref",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "PaymentBridge",
                        action = "ReadycashRequeryReference"
                    }
                },
                    new HttpRouteDescriptor {
                    Name = "bridgereadycashagentcreateinvoice",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/agent/rdc/invoice/create",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "RDCCreateInvoice",
                        action = "BillerCreateInvoice"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "bridgegenericinvoicevalidation",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/invoice/validate-invoice",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "Bridge",
                        action = "Validate"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "bridgegenericpaymentnotification",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/payment/payment-notification",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "PaymentBridge",
                        action = "Notification"
                    }
                },
                 new HttpRouteDescriptor {
                    Name = "bridgestatetincreation",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/statetin/create",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "StateTINBridge",
                        action = "CreateStateTIN"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "bridgecreateinvoice",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/invoice/create",
                    Defaults = new {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "InvoiceBridge",
                        action = "CreateInvoice"
                    }
                },
                new HttpRouteDescriptor
                {
                    Name = "bridge-search-by-filter",
                    Priority = -10,
                    RouteTemplate = "api/v1/bridge/user/search-by-filter",
                    Defaults = new
                    {
                        area = "Parkway.CBS.Tenant.Bridge",
                        controller = "UserBridge",
                        action = "SearchByFilter"
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