using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Services.MRole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELODIE.Rpc
{
    public class PermissionController : SimpleController
    {
        private IPermissionService PermissionService;
        private ICurrentContext CurrentContext;
        public PermissionController(IPermissionService PermissionService, ICurrentContext CurrentContext)
        {
            this.PermissionService = PermissionService;
            this.CurrentContext = CurrentContext;
        }

        [HttpPost, Route("rpc/elodie/permission/list-path")]
        public async Task<List<string>> ListPath()
        {
            return await PermissionService.ListPath(CurrentContext.UserId);
        }

       
    }
}
