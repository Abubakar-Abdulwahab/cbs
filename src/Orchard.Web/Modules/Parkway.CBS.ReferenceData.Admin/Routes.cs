using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.ReferenceData.Admin
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor {
                    Name = "UploadData",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Enumeration/UploadData",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"}, // this is the name of your module
                            {"controller", "Enumeration"},
                            {"action", "UploadData"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "CheckReferenceDataStatus",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Enumeration/CheckStatus",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"}, // this is the name of your module
                            {"controller", "Enumeration"},
                            {"action", "CheckBatchRecords"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "DownloadBatchPDFInvoice",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Enumeration/DownloadBatchPDFInvoice/{batchRef}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"}, // this is the name of your module
                            {"controller", "Enumeration"},
                            {"action", "DownloadBatchPdfInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "DownloadBatchExcelInvoice",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Enumeration/DownloadBatchExcelInvoice/{batchId}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"}, // this is the name of your module
                            {"controller", "Enumeration"},
                            {"action", "ExportNAGISRecordRecords"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ReferenceData.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}