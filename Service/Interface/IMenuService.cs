﻿using Models.Data;
using Models.EF;
using Microsoft.EntityFrameworkCore;

using Service.Helpers;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
   public interface IMenuService: IDisposable, ICommonService<Menu>
    {
        Task<List<Permission>> GetPermissions();
    }
     
}
