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
    public class EventsController : Controller
    {
        public CategoryRepository CatRep = new CategoryRepository();
        public EventRepository EventRep = new EventRepository();
        public AdvertisementRepository AdvertisementRep = new AdvertisementRepository();
        private AppRequestRepository AppRep = new AppRequestRepository();
        private CommunicationRepository comrep = new CommunicationRepository();
        private Functions func = new Functions();

        public ActionResult _AsyncSubCategorys(int? CategoryId)
        {
            List<SelectListItem> SubCategory = CatRep.GetSubCategoryList((int)CategoryId);
            return Json(SubCategory);
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> attachments)
        {
            PictureRepository picRep = new PictureRepository();
            Functions functions = new Functions();
            int NewPicID = picRep.GetLastPictureId(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            NewPicID++;
            Picture ins = new Picture();

            foreach (var file in attachments)
            {
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var extention = fileName.Substring(fileName.IndexOf('.'));
                var newfilename = NewPicID.ToString() + extention;
                var physicalPath = Path.Combine(Server.MapPath("~/Images/Schools/" + HttpContext.Session["SchoolId"].ToString() + "/Events"), newfilename);

                //...Resize..
                Image original = Image.FromStream(file.InputStream, true, true);
                Image resized = functions.ResizeImage(original, new Size(100, 100));

                resized.Save(physicalPath, ImageFormat.Jpeg);

                string finalpath = physicalPath.ToString();
                finalpath = finalpath.Substring(finalpath.IndexOf("Images"));
                finalpath = finalpath.Replace('\\', '/');
                finalpath = "http://www.Netintercom.co.za/" + finalpath;

                //...Save In DB...                
                ins.PicUrl = finalpath;
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
                var physicalPath = Path.Combine(Server.MapPath("~/Images/Schools/" + HttpContext.Session["SchoolId"].ToString() + "/Events"), newfilename);

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

        public ActionResult SaveAD(IEnumerable<HttpPostedFileBase> attachments)
        {
            PictureRepository picRep = new PictureRepository();
            Functions functions = new Functions();
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

                string finalpath = physicalPath.ToString();
                finalpath = finalpath.Substring(finalpath.IndexOf("Images"));
                finalpath = finalpath.Replace('\\', '/');
                finalpath = "http://www.Netintercom.co.za/" + finalpath;

                //...Save In DB...                
                ins.PicUrl = finalpath;
                ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

                ins = picRep.InsertPicture(ins);
            }
            // Return an empty string to signify success
            if (ins.PictureId != 0)
                return Json(new { status = ins.PictureId.ToString() }, "text/plain");
            else
                return Json(new { status = "0" }, "text/plain");
        }
        public ActionResult RemoveAD(string[] fileNames)
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
        
        public ActionResult Events()
        {
            CategoryRepository CatRep = new CategoryRepository();
            ViewData["Category"] = CatRep.GetListCategoryDropDown();
            return View();
        }
        
        [GridAction]
        public ActionResult _ListEvent()
        {
            //...Initialize List...
            List<Event> lst = new List<Event>();

            //...Populate List...
            lst = EventRep.GetListEvent(Convert.ToInt32(HttpContext.Session["SchoolId"]));

            //...Return List to Grid...
            return View(new GridModel(lst));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]        
        [GridAction]
        public ActionResult _InsertEvent(Event ins)
        {
            //...Fix...
            ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

            //...Insert into Database...
            Event ins2 = EventRep.InsertEvent(ins);

            //...Notify...
            string regIds = AppRep.GetAllRegIds(ins.ClientId);
            comrep.NewsyncData(regIds, "CMD_NEWEVENT");

            //...Repopulate Grid...
            List<Event> lst = new List<Event>();
            lst = EventRep.GetListEvent(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }
        
        [GridAction]
        public ActionResult _UpdateEvent(Event ins)
        {
            //...ViewData...
            ins.ClientId = Convert.ToInt32(HttpContext.Session["SchoolId"]);

            Event ins2 = EventRep.UpdateEvent(ins);

            //...Notify...
            string regIds = AppRep.GetAllRegIds(ins.ClientId);
            comrep.NewUpdateData(regIds, "CMD_EDITEVENT", ins2.EventsId.ToString());

            //...Repopulate Grid...
            List<Event> lst = new List<Event>();
            lst = EventRep.GetListEvent(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteEvent(int id)
        {
            PictureRepository picRep = new PictureRepository();
            Event ins = EventRep.GetEvent(id);
            if (ins.PictureId != 0)
            {
                //...Get Picture...
                Picture pic = picRep.GetPicture(ins.PictureId);
                string path = pic.PicUrl.Substring(pic.PicUrl.IndexOf("/Images/") + 8);
                path = path.Replace('/', '\\');
                var finalpath = Path.Combine(Server.MapPath("~/Images"), path);
                System.IO.File.Delete(finalpath);
                bool picrem = picRep.RemovePicture(ins.PictureId);
            }

            bool ins2 = EventRep.RemoveEvent(id);

            //...Notify...
            string regIds = AppRep.GetAllRegIds(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            comrep.NewUpdateData(regIds, "CMD_DELEVENT", id.ToString());

            //...Repopulate Grid...
            List<Event> lst = new List<Event>();
            lst = EventRep.GetListEvent(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
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

            //...Notify...
            string regIds = AppRep.GetAllRegIds(ins.ClientId);
            comrep.NewUpdateData(regIds, "CMD_EDITAD", ins2.AdvertisementId.ToString());

            //...Repopulate Grid...
            List<Advertisement> lst = new List<Advertisement>();
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAdvertisement(int id)
        {
            PictureRepository picRep = new PictureRepository();
            Advertisement ins = AdvertisementRep.GetAdvertisement(id);

            if (ins.PictureId != 0)
            {
                //...Get Picture...
                Picture pic = picRep.GetPicture(ins.PictureId);
                string path = pic.PicUrl.Substring(pic.PicUrl.IndexOf("/Images/") + 8);
                path = path.Replace('/', '\\');
                var finalpath = Path.Combine(Server.MapPath("~/Images"), path);
                System.IO.File.Delete(finalpath); 
                bool picrem = picRep.RemovePicture(ins.PictureId);
            }

            bool ins2 = AdvertisementRep.RemoveAdvertisement(id);

            //...Notify...
            string regIds = AppRep.GetAllRegIds(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            comrep.NewUpdateData(regIds, "CMD_DELAD", id.ToString());

            //...Repopulate Grid...
            List<Advertisement> lst = new List<Advertisement>();
            lst = AdvertisementRep.GetListAdvertisement(Convert.ToInt32(HttpContext.Session["SchoolId"]));
            return View(new GridModel(lst));
        }

    }
}