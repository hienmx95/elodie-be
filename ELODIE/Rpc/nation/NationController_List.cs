using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using ELODIE.Entities;
using ELODIE.Services.MNation;
using ELODIE.Services.MStatus;

namespace ELODIE.Rpc.nation
{
    public partial class NationController : RpcController
    {
        [Route(NationRoute.FilterListStatus), HttpPost]
        public async Task<List<Nation_StatusDTO>> FilterListStatus([FromBody] Nation_StatusFilterDTO Nation_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Nation_StatusDTO> Nation_StatusDTOs = Statuses
                .Select(x => new Nation_StatusDTO(x)).ToList();
            return Nation_StatusDTOs;
        }

        [Route(NationRoute.SingleListStatus), HttpPost]
        public async Task<List<Nation_StatusDTO>> SingleListStatus([FromBody] Nation_StatusFilterDTO Nation_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Nation_StatusDTO> Nation_StatusDTOs = Statuses
                .Select(x => new Nation_StatusDTO(x)).ToList();
            return Nation_StatusDTOs;
        }

    }
}

