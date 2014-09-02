﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Netintercom.Models;
using System.Text;

namespace Netintercom.Controllers
{
    public class AppRequestsController : Controller
    {
        private AppRequestRepository appRep = new AppRequestRepository();

        [HttpPost]
        public JsonResult GetNews(int ClientId, int LastId)
        {
            //...Query DB....
            List<NewsRequest> list = new List<NewsRequest>();
            list = appRep.GetNews(ClientId, LastId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNotifications(int ClientId, int LastId)
        {
            //...Query DB....
            List<Notifications> list = new List<Notifications>();
            list = appRep.GetNotifications(ClientId, LastId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEvents(int ClientId, int LastId)
        {
            //...Query DB....
            List<EventRequest> list = new List<EventRequest>();
            list = appRep.GetEvents(ClientId, LastId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetContacts(int ClientId)
        {
            //...Query DB....
            List<ContactRequest> list = new List<ContactRequest>();
            list = appRep.GetContact(ClientId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAds(int ClientId, int LastId)
        {
            //...Query DB....
            List<AdRequest> list = new List<AdRequest>();
            list = appRep.GetAd(ClientId, LastId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSplashAd(int ClientId)
        {
            //...Query DB....
            AdRequest list = new AdRequest();
            list = appRep.GetRandomSplashAdd(ClientId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegisterDevice(int ClientId, string RegId)
        {
            bool done = appRep.RegisterDevice(ClientId, RegId);

            return null;
        }

        [HttpPost]
        public JsonResult EditNews(int ClientId, int EditId)
        {
            //...Query DB....
            List<NewsRequest> list = new List<NewsRequest>();
            list = appRep.GetEditNews(ClientId, EditId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditEvent(int ClientId, int EditId)
        {
            //...Query DB....
            List<EventRequest> list = new List<EventRequest>();
            list = appRep.GetEditEvent(ClientId, EditId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditContact(int ClientId, int EditId)
        {
            //...Query DB....
            List<ContactRequest> list = new List<ContactRequest>();
            list = appRep.GetEditContact(ClientId, EditId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditAd(int ClientId, int EditId)
        {
            //...Query DB....
            List<AdRequest> list = new List<AdRequest>();
            list = appRep.GetEditAd(ClientId, EditId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegisterUser(int ClientId, string NameSurname, string Email, string Password, string Phone, string DeviceId)
        {
            DeviceUser newUser = new DeviceUser(ClientId, DeviceId, NameSurname, Phone, Email, Password);

            if (appRep.CheckDeviceUserRegistration(DeviceId, Phone, ClientId.ToString(), Password))
            {
                newUser.DeviceUserId = -1;
                var j = this.Json(newUser);
                return Json(j, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //* In app, check if the DeviceUserId field != 0 for a successfull registration *//
                DeviceUser insertedUser = appRep.AddDeviceUser(newUser);
                var j = this.Json(newUser);
                return Json(j, JsonRequestBehavior.AllowGet);                
            }         
        }

        [HttpPost]
        public ActionResult LoginDeviceUser(string DeviceId, string Phone, string ClientId, string Password)
        {
            return Content(appRep.CheckDeviceUserLogin(Phone, ClientId, Password), "text/html");
        }

        [HttpPost]
        public JsonResult GetDocs(int ClientId, int LastId)
        {
            DocumentsRepository DocRep = new DocumentsRepository();
            //...Query DB....
            List<Documents> list = new List<Documents>();
            list = DocRep.GetDocuments(ClientId, LastId);

            var j = this.Json(list);
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RequestService(int DeviceUserId, string ServiceRequest, string Query)
        {
            DeviceUserRepository DevURep = new DeviceUserRepository();
            ServicesRepository ServRep = new ServicesRepository();

            Services service = new Services();
            DeviceUser user = DevURep.GetDeviceUser(DeviceUserId);
            

            service.ClientId = user.ClientId;
            service.DeviceUserId = DeviceUserId;
            service.Service = ServiceRequest;
            service.Query = Query;
            service.ModifiedDate = DateTime.Now;

            StringBuilder w = new StringBuilder();

            //if (service.Service.Equals("Escort"))
            //{
            //    StringBuilder s = new StringBuilder();
            //    s.Append("<iframe src=\"").Append(service.Query).Append("\"></iframe>");
            //    w.Append("<html><body>");
            //    w.Append(user.Name).Append(" ").Append(user.Surname).Append(" Request: ").Append(service.Service);
            //    w.Append("<br>").Append(s.ToString());
            //    w.Append("</body></html>");
            //}
            //else
            //{
                w.Append(user.Name).Append(" ").Append(user.Surname).Append(" Request: ").Append(service.Service);
                w.Append("\n").Append(service.Query);
            //}

            Services ins = ServRep.AddServices(service);

            if (ins.ServiceId != 0)
            {
                //Send email
                Functions f = new Functions();

                if (user.ClientId == 11)
                {
                    f.SendEmail(w.ToString(), "alarms@gmail.com", service.Service);
                }
                else
                {
                    f.SendEmail(w.ToString(), Constants.MailerAddress, service.Service);
                }

                return Content("Success", "text/html");
            }
            else
            {
                return Content("Fail", "text/html");
            }

            
        }
    }
}
