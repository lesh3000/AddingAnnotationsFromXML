using DocuViewareREST.Models;
using GdPicture14;
using GdPicture14.Annotations;
using GdPicture14.WEB;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DocuViewareREST
{
    public class WebApiApplication : HttpApplication
    {
        public static readonly int SESSION_TIMEOUT = 20; //Set to 20 minutes. Use -1 to handle DocuVieware session timeout through ASP.NET session mechanism.
        private static readonly bool STICKY_SESSION = true; //Set false to use DocuVieware on Servers Farm with non sticky sessions.
        private const DocuViewareSessionStateMode DOCUVIEWARE_SESSION_STATE_MODE = DocuViewareSessionStateMode.InProc; //Set DocuViewareSessionStateMode.File is STICKY_SESSION is False.

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            DocuViewareManager.SetupConfiguration(STICKY_SESSION, DOCUVIEWARE_SESSION_STATE_MODE, GetCacheDirectory());
            DocuViewareLicensing.RegisterKEY("0451408572764866758421748"); //Unlocking DocuVieware. Please insert your demo or commercial license key here.
            DocuViewareEventsHandler.NewDocumentLoaded += NewDocumentLoadedHandler;
            DocuViewareEventsHandler.CustomAction += Dispatcher;
        }

        
        private static string GetCacheDirectory()
        {
            return HttpRuntime.AppDomainAppPath + "\\Cache";
        }

        private static void Dispatcher(object sender, CustomActionEventArgs e)
        {
            if (e.actionName == "load")
            {
                AnnotationManager oManager = new AnnotationManager();
                GdPictureImaging oTif = new GdPictureImaging();
                string pathFile = e.args.ToString() + ".tif";
                int imageNr = oTif.CreateGdPictureImageFromFile(HttpRuntime.AppDomainAppPath + "\\docs\\" + pathFile);
                oManager.InitFromGdPictureImage(imageNr);
                string pathAnnot = e.args.ToString() + "_OCR.xml";

                System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Batch));
                System.IO.StreamReader file = new System.IO.StreamReader(HttpRuntime.AppDomainAppPath + "\\docs\\" + pathAnnot);
                Batch overview = (Batch)reader.Deserialize(file);
                file.Close();
                int pgNr = -1;
                int.TryParse( overview.Doc.Pg.PgNo, out pgNr);
                float top = 0;
                float left = 0;
                float hight = 0;
                float width = 0;
                float Hresolution = oTif.GetHorizontalResolution(imageNr);
                float Vresolution = oTif.GetVerticalResolution(imageNr);
                for ( int i =1; i<=oManager.PageCount; i++)
                {
                    if (pgNr == i)
                    {

                        

                        oManager.SelectPage(pgNr);
                        for (int u = 0; u < overview.Doc.Pg.Wd.Count; u++)
                        {
                            float.TryParse(overview.Doc.Pg.Wd[u].Tp, out top);
                            float.TryParse(overview.Doc.Pg.Wd[u].Lt, out left);
                            float.TryParse(overview.Doc.Pg.Wd[u].Ht, out hight);
                            float.TryParse(overview.Doc.Pg.Wd[u].Wt, out width);
                            Annotation an=oManager.AddRectangleHighlighterAnnot(Color.Black, left/300, top/300, width/300, hight / 300);
                            an.ClientTag= overview.Doc.Pg.Wd[u].Txt;
                            an.CanRotate = false;
                            an.CanMove = false;
                            an.CanEdit = false;
                            an.CanDelete = true;// done for demo purposes so it behaves as annotation.
                            //set to false if there is no need to show annotation action button
                            //more properties are available:https://guides.gdpicture.com/content/GdPicture.NET.14~GdPicture14.Annotations.AnnotationRectangleHighlighter_members.html




                        }

                        oManager.SaveAnnotationsToPage();

                    }

                }

                e.docuVieware.LoadFromGdPictureImage(imageNr);
            }


        }

            private static void NewDocumentLoadedHandler(object sender, NewDocumentLoadedEventArgs e)
        {
            e.docuVieware.PagePreload = e.docuVieware.PageCount <= 50 ? PagePreloadMode.AllPages : PagePreloadMode.AdjacentPages;
        }
    }
}
