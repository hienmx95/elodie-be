using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Handlers;
using ELODIE.Helpers;
using ELODIE.Models;
using ELODIE.Repositories;
using ELODIE.Services.MProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace ELODIE.Rpc
{
    public class InstagramController : ControllerBase
    {
        private DataContext DataContext;
        //private IRabbitManager RabbitManager;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        private readonly IHttpContextAccessor HttpContextAccessor;

        private IUOW UOW;

        public InstagramController(DataContext DataContext, IItemService ItemService, ICurrentContext CurrentContext, IHttpContextAccessor HttpContextAccessor, IUOW UOW)
        {
            this.DataContext = DataContext;
            //this.RabbitManager = RabbitManager;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
            this.HttpContextAccessor = HttpContextAccessor;
            this.UOW = UOW;
        }

        [HttpGet, Route("instagram/tracking")]
        public RedirectResult Tracking()
        {
            string supplier = HttpContext.Request.Query["supplier"].ToString();
            if (!string.IsNullOrEmpty(supplier))
            {
                InstagramSupplierDAO InstagramSupplierDAO = DataContext.InstagramSupplier
               .Where(r => r.Name.Equals(supplier))
               .FirstOrDefault();
                if (InstagramSupplierDAO != null)
                {
                    InstagramSupplierDAO.CountView++;
                    DataContext.SaveChanges();
                }
                return Redirect("https://www.instagram.com/elodie.chandelle/");
            }
            return Redirect("https://www.instagram.com/elodie.chandelle/");
        }
    }
}