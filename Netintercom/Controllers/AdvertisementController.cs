﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Netintercom.Models;
using Telerik.Web.Mvc;
using System.IO;
using Telerik.Web.Mvc.Extensions;
using System.Drawing;
using System.Drawing.Imaging;

namespace Netintercom.Controllers
{
    [AutoLogOffActionFilter]
    public class AdvertisementController : Controller
    {
        private AdvertisementRepository AdvertisementRep = new AdvertisementRepository();
        private AppRequestRepository AppRep = new AppRequestRepository();
        private CommunicationRepository comrep = new CommunicationRepository();
        private PictureRepository picRep = new PictureRepository();
        private Functions func = new Functions();

        public ActionResult Advertisement()
        {
            return View();
        }
       
        [GridAction]
        public ActionResult _ListAdvertisement()
        {
            //...Initialize List...
            List<Advertisement> lst = new List<Advertisement>();

            //...Populate List...
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));

            //...Return List to Grid...
            return View(new GridModel(lst));
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> attachments)
        {
            PictureRepository picRep = new PictureRepository();
            Functions functions = new Functions();
            // The Name of the Upload component is "attachments" 
            int NewPicID = picRep.GetLastPictureId(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            NewPicID++;
            Picture ins = new Picture();

            foreach (var file in attachments)
            {
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var extention = fileName.Substring(fileName.IndexOf('.'));
                var newfilename = NewPicID.ToString() + extention;
                var physicalPath = Path.Combine(Server.MapPath("~/Images/Schools/" + HttpContext.Session["SchoolId"].ToString() + "/Ads"), newfilename);

                //...Resize..
                Image original = Image.FromStream(file.InputStream, true, true);
                Image resized = functions.ResizeImage(original, new Size(100, 100));

                resized.Save(physicalPath, ImageFormat.Jpeg);

                //...Save In DB...                
                ins.PicUrl = physicalPath;
                ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

                ins = picRep.InsertPicture(ins);
            }
            // Return an empty string to signify success
            if (ins.PictureId != 0)
                return Json(new { status = ins.PictureId.ToString() }, "text/plain");
            else
                return Json(new { status = "0" }, "text/plain");
        }
        public ActionResult Remove(string[] fileNames)
        {
            PictureRepository picRep = new PictureRepository();
            int NewPicID = picRep.GetLastPictureId(Convert.ToInt32(HttpContext.Session["SchoolId"]));

            // The parameter of the Remove action must be called "fileNames"
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var extention = fileName.Substring(fileName.IndexOf('.'));
                var newfilename = NewPicID.ToString() + extention;
                var physicalPath = Path.Combine(Server.MapPath("~/Images/Schools/" + HttpContext.Session["SchoolId"].ToString() + "/Ads"), newfilename);

                // TODO: Verify user permissions
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);

                    picRep.RemovePicture(NewPicID);
                }
            }
            // Return an empty string to signify success
            return Content("");
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAdvertisement(Advertisement ins)
        {
            //...Fix...
            ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

            //...Insert into Database...
            Advertisement ins2 = AdvertisementRep.InsertAdvertisement(ins);

            //...Notify...
            string regIds = AppRep.GetAllRegIds(ins.ClientId);
            comrep.NewsyncData(regIds, "CMD_NEWAD");

            //...Repopulate Grid...
            List<Advertisement> lst = new List<Advertisement>();
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }
        
        [GridAction]
        public ActionResult _UpdateAdvertisement(Advertisement ins)
        {
            //...ViewData...
            ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

            Advertisement ins2 = AdvertisementRep.UpdateAdvertisement(ins);

            //...Repopulate Grid...
            List<Advertisement> lst = new List<Advertisement>();
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAdvertisement(int id)
        {
            Advertisement ins = AdvertisementRep.GetAdvertisement(id);
            Picture pic = picRep.GetPicture(ins.PictureId);

            picRep.RemovePicture(pic.PictureId);
            func.DeleteFile(pic.PicUrl);
            
            bool ins2 = AdvertisementRep.RemoveAdvertisement(id);

            //...Repopulate Grid...
            List<Advertisement> lst = new List<Advertisement>();
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }

    }
}