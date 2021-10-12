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
using ELODIE.Services.MCustomer;
using ELODIE.Services.MAppUser;
using ELODIE.Services.MCodeGeneratorRule;
using ELODIE.Services.MCustomerSource;
using ELODIE.Services.MDistrict;
using ELODIE.Services.MNation;
using ELODIE.Services.MOrganization;
using ELODIE.Services.MProfession;
using ELODIE.Services.MProvince;
using ELODIE.Services.MSex;
using ELODIE.Services.MStatus;
using ELODIE.Services.MWard;

namespace ELODIE.Rpc.customer
{
    public partial class CustomerController : RpcController
    {
    }
}

