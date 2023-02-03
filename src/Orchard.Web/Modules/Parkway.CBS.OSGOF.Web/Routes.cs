using System.Web.Mvc;
using Orchard.Mvc.Routes;
using System.Web.Routing;
using System.Collections.Generic;
using Parkway.CBS.Module.Web;
using Orchard.Logging;
using Newtonsoft.Json;

namespace Parkway.CBS.OSGOF.Web
{
    public class Routes : IRouteProvider
    {
        public ILogger Logger { get; set; }

        public Routes()
        {
            Logger = NullLogger.Instance;

        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }


        public IEnumerable<RouteDescriptor> GetRoutes()
        {
          var osogofRoutes = new[] {
               
                    new RouteDescriptor {
                    Name = "C.OSGOF.OnScreen",
                    Priority = 5,
                    Route = new Route(
                       "c/osgof-onscreen",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "OSGOFOnScreenInput"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                     new RouteDescriptor {
                    Name = "C.File.Upload",
                    Priority = 5,
                    Route = new Route(
                       "c/osgof-fileupload",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "OSGOFFileUpload"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },      
                 
                  new RouteDescriptor {
                     Name = "C.CellSiteProcessFile",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/process-cell-site-file", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "StartCellSiteProcessing"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.CellSiteComparisonLeg",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/process-cell-site-file-leg2", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "CellSiteComparison"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.CellSiteDataDetails",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/process-cell-site-schedule-details", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "CellSitePaymentScheduleDetails"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.CellSitesMoveRight",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/get-next-cellsites-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "CellSitesMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.CellSitesScheduleMoveRight",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/add-cellsite/x/get-next-cellsites-schedule-page/{scheduleRef}/{Page}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "CellSitesScheduleUploadMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.AJX.GetCellSites",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-operator-cellsites", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetCellSites"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.AJX.GetCellSite",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/get-cellsite", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetCellSite"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.AddCellSite.Upload",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/upload-cellsite", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "UploadCellSite"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "C.AddCellSite.Report",
                    Priority = 5,
                    Route = new Route(
                       "c/add-cellsite/{scheduleRef}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "CellSitesFileUploadReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "C.CellSites.Report",
                    Priority = 5,
                    Route = new Route(
                       "c/cellsites/Report",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CellSiteCollection"},
                            {"action", "CellSiteList"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                     Name = "C.CellSiteListMoveRight",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/cellsites/x/get-next-cellsites-list-page/{operatorId}/{Page}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "CellSiteListMoveRight"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Web"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };

            var routes = BaseRoutes.GetRoutes("OSGOF.", "OSGOF");
            routes.AddRange(osogofRoutes);
            return routes;

        }
    }
}