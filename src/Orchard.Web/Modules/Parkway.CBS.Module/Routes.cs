using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Module
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
            return new[] {
                //web api route
                new RouteDescriptor {
                    Name = "API.Route",
                    Priority = 5,
                    Route = new Route(
                       "api", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Integration"},
                            { "action", "tes" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "MDA.Dashboard",
                    Priority = 5,
                    Route = new Route(
                       "Admin", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "MainDashboard"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // create mda route
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "CreateMDASettings"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end create route
                // mda list route
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDAs", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "List"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end list route
                // mda revenue head list route
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/{slug}/RevenueHeads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "ViewMDARevenueHeads"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end mda revenue head list route
                // mda change status route
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/ChangeStatus/{TaxEntityId}/{pageNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "ChangeStatus"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end mda change status route
                // CREATE REVENUE HEAD FROM MDA PAGE
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/{slug}/RevenueHead/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "CreateFromMDA"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end mda revenue head list route
                // CREATE REVENUE HEAD FROM MDA PAGE
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "CreateFromMDA"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },// CREATE REVENUE HEAD FROM MDA PAGE
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/{slug}/{id}/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "CreateFromRevenueHead"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // mda revenue head edit route
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/{slug}/Edit", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "Edit"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end revenue head edit route
                // mda revenue head hierarchy view
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/MDA/{slug}/Overview", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "MDA"},
                            {"action", "ViewHierarchy"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                // end revenue head edit route
                // mda revenue head hierarchy view
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Overview", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "ViewHierarchy"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Edit", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "Edit"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/{slug}/{id}/SubRevenueHeads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "ViewSubRevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHead/{parentrevenuedheadslug}/{parentrevenueheadid}/SubRevenueHead/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "SubRevenueHead"},
                            {"action", "Create"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5, //billing create
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Billing/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Billing"},
                            {"action", "Create"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5, //form create
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Dashboard", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "RevenueHeadDashBoard"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5, //form create
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Form/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Form"},
                            {"action", "Create"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5, //form create
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Form/Edit", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Form"},
                            {"action", "Edit"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5, //billing create
                    Route = new Route(
                       "Admin/RevenueHead/{revenueHeadSlug}/{revenueHeadId}/Billing/Edit", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Billing"},
                            {"action", "Edit"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                //reports
                new RouteDescriptor {
                    Name = "AssessmentReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Reports/Asessments", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "AssessmentReport"},
                            {"id", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
              
                //reports
                new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Reports/TaxEntity", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "TaxEntity"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/RevenueHeads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "List"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Priority = 5, //view billable rev head
                    Route = new Route(
                       "Admin/RevenueHeads/Assessment", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHead"},
                            {"action", "BillablesList"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Settings/ChangePassword", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "ChangePassword"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/Default", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "AdminSettings"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "ListOfExpertSystems"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/State", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "SetState"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/GetRegsiteredRefDataItems", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "GetRegsiteredRefDataItems"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/GetClientSecret", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "GetClientSecret"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/New", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "CreateExpertSystem"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/Edit/{identifier}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "EditExpertSystem"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Priority = 5, //admin settings
                    Route = new Route(
                       "Admin/Settings/ReferenceData", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settings"},
                            {"action", "ReferenceDataSettings"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Reports/RevenueHeads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "MDARevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Report/RevenueHeads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "MDARevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   //reports
                new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Reports/TaxPayersAccount/RevenueHeads/Tax", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "MDARevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Reports/MonthlyPaymentReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "MdaPaymentReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },//AddPaymentAdmin
                   new RouteDescriptor {
                    Name = "AddPaymentAdmin",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Payments/Add/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Payment"},
                            {"action", "AddInvoicePayment"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "AdminViewInvoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Invoice/View/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "PreviewInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "Collection",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/CollectionReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "CollectionReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "SearchInvoicePayment",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Payment/SearchInvoicePayment", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Payment"},
                            {"action", "SearchInvoicePayment"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceSearchForTaxEntity",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/SearchForUser", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "GenerateInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceSelectRevenue",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/SelectRevenue/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "GenerateInvoiceSelectRevenueHead"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceConfirmTaxPayer",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/ConfirmTaxPayer", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "ConfirmTaxPayer"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceConfirmedTaxPayerAndRevenueHead",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/ConfirmedTaxPayerAndRevenueHead", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "ConfirmedTaxPayerAndRevenueHead"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceCreateBill",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/CreateBill", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "CreateBill"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceConfirmInvoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/ConfirmInvoice", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "ConfirmInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "GenerateInvoiceDetails",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/GenerateInvoice/InvoiceDetails/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "InvoiceDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "AdminViewReceipt",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Receipts/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Payment"},
                            {"action", "ViewReceipt"},
                            },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "TaxPayersReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/TaxPayersReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "TaxProfilesReport"},
                            },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "AJAXInvoiceReportRevenueHeadsPerMDA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/X/InvoiceReportRevenueHeadsPerMDA", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "ReportAJAX"},
                            {"action", "GetInvoiceReportMDARevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "FileExportDownload",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/FileExport/Download/C/{format}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "FileExport"},
                            {"action", "CollectionReportDownload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "AssessmentReportDownload",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/FileExport/Download/A/{format}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "FileExport"},
                            {"action", "AssessmentReportDownload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "CreateSettlement",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Settlement/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settlement"},
                            {"action", "CreateSettlementRule"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {

                    Name = "AdminCheckStatementOfAccount",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/ViewStatementOfAccount/{payerId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "StatementOfAccountDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Name = "ExcelTaxProfilesReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/ExcelTaxProfilesReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "ExcelTaxProfilesReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Name = "PdfTaxProfilesReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/PdfTaxProfilesReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Report"},
                            {"action", "PdfTaxProfilesReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "SettlementGetRevenueHeadsForMDA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Settlement/Ajax/Revenueheads", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "SettlementAJAX"},
                            {"action", "MDARevenueHeads"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "SettlementGetRevenueHeadsPerMDA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Settlement/Ajax/RevenueheadsPerMda", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "SettlementAJAX"},
                            {"action", "GetRevenueHeadsPerMda"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "ListOfSettlementRules",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Settlement/Rules", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Settlement"},
                            {"action", "Settlements"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },

                   new RouteDescriptor {
                    Name = "SearchInvoiceNumberForPaymentRef",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Invoice/SearchForPaymentRefs", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "SearchInvoiceNumberForPaymentRef"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "ExternalPaymentProvider.Create",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/Create", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "ExternalPaymentProvider"},
                            {"action", "Create"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PaymentProviderValidationConstraint.Assign",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/Assign-Restrictions/{providerId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "PaymentProviderValidationConstraint"},
                            {"action", "Assign"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "ExternalPaymentProvider.List",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/List", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "ExternalPaymentProvider"},
                            {"action", "List"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PaymentProviderValidationConstraintAJAX.Ajax.Fetch.Revenue.Heads.Per.Mda",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/RevenueHeadsPerMda", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "PaymentProviderValidationConstraintAJAX"},
                            {"action", "GetRevenueHeadsPerMda"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "ExternalPaymentProviderAJAX.Ajax.Fetch.Client.Secret",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/Settings/GetClientSecret", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "ExternalPaymentProviderAJAX"},
                            {"action", "GetClientSecret"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "RevenueHeadPermissions.Assign",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExpertSystem/RevenueHead/Permission/{expertSystemId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHeadPermissions"},
                            {"action", "Assign"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "RevenueHeadPermissions.Ajax.Fetch.Revenue.Heads.Per.Mda",
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHeadPermissions/RevenueHeadsPerMda", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHeadPermissions"},
                            {"action", "GetRevenueHeadsPerMda"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "RevenueHeadPermissions.Ajax.Fetch.Mda.For.AccessType",
                    Priority = 5,
                    Route = new Route(
                       "Admin/RevenueHeadPermissions/MdasForAccessType", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "RevenueHeadPermissions"},
                            {"action", "GetMdasForAccessType"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.GenerateInvoice.ShowFormFields,
                    Priority = 5,
                    Route = new Route(
                       "Admin/GenerateInvoice/FormFields", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "Invoice"},
                            {"action", "ShowFormFields"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PaymentProviderValidationConstraintAJAX.Update",
                    Priority = 5,
                    Route = new Route(
                       "Admin/ExternalPaymentProvider/X/Update-Restrictions", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"}, // this is the name of your module
                            {"controller", "PaymentProviderValidationConstraintAJAX"},
                            {"action", "UpdateStagingData"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Module"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },

            };
        }
    }
}