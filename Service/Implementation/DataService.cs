using Models.Data;
using Models.EF;
using Models.ViewModels.Auth;
using Models.ViewModels.Data;
using Models.ViewModels.Level;
using Models.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Helpers;
using Service.Interface;
using Microsoft.Extensions.Configuration;

namespace Service.Implementation
{
   
    public class DataService: IDataService
    {
        private readonly DataContext _dbContext;
        private readonly ILevelService _levelService;
        private readonly IConfiguration _configuration;

        public DataService(DataContext dbContext, ILevelService levelService,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _levelService = levelService;
            _configuration = configuration;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public async Task<List<ManagerOwnerUpdaterSponsorParticipantViewModel>> GetAllManagerOwnerUpdaterSponsorParticipant(int categoryID)
        {
            var managers = _dbContext.Managers;
            var owners = _dbContext.Owners;
            var updaters = _dbContext.Uploaders;
            var sponsors = _dbContext.Sponsors;
            var participant = _dbContext.Participants;
            var KPIs = _dbContext.KPIs;
            var users = _dbContext.Users;

            var data = await _dbContext.CategoryKPILevels.Where(x => x.CategoryID == categoryID)
                .Join(_dbContext.KPILevels,
                categoryKPILevel => categoryKPILevel.KPILevelID,
                kpilevel => kpilevel.ID,
                (categoryKPILevel, kpilevel) => new
                {
                    categoryKPILevel.KPILevelID,
                    categoryKPILevel.CategoryID,
                    kpilevel.KPIID,
                    kpilevel.KPILevelCode
                })
                 .Join(KPIs,
                categoryKPILevelKPI => categoryKPILevelKPI.KPIID,
                kpi => kpi.ID,
                (categoryKPILevelKPI, kpi) => new
                {
                    categoryKPILevelKPI.KPILevelID,
                    categoryKPILevelKPI.KPILevelCode,
                    categoryKPILevelKPI.CategoryID,
                    KPIName = kpi.Name
                })
                .Select(x => new
                {
                    x.CategoryID,
                    x.KPILevelID,
                    x.KPIName,
                    x.KPILevelCode
                }).ToListAsync();

            var model = data
                .Select(x => new ManagerOwnerUpdaterSponsorParticipantViewModel
                {
                    CategoryID = x.CategoryID,
                    KPILevelID = x.KPILevelID,
                    KPIName = x.KPIName,
                    KPILevelCode = x.KPILevelCode
                }).ToList();

            return model;
        }
        public async Task<object> GetAllDataByCategory(int categoryid, string period, int? start, int? end, int? year)
        {
            var currentYear = DateTime.Now.Year;
            var currentWeek = DateTime.Now.GetIso8601WeekOfYear();
            var currentMonth = DateTime.Now.Month;
            var currentQuarter = DateTime.Now.GetQuarter();
            //labels của chartjs mặc định có 53 phần tử
            List<DatasetViewModel> listDatasetViewModel = new List<DatasetViewModel>();
            if (!period.IsNullOrEmpty())
            {
                var datasets = new List<object>();
                //labels của chartjs mặc định có 53 phần tử
                List<string> listLabels = new List<string>();

                var dataremarks = new List<Dataremark>();

                //var tbldata = _dbContext.Datas;


                var listKPILevelID = await GetAllManagerOwnerUpdaterSponsorParticipant(categoryid);

                if (year == 0)
                    year = currentYear;

                if (period.ToLower() == "w")
                {
                    foreach (var item in listKPILevelID)
                    {
                        //var kpi = tblKPI.Find(item.KPIID).Name;
                        var tblCategory = await _dbContext.Categories.FindAsync(item.CategoryID);
                        var categorycode = tblCategory.Code;

                        var obj = await GetAllOwner(categoryid, item.KPILevelID);
                        var tbldata = await _dbContext.Datas
                       .Where(x => x.KPILevelCode == item.KPILevelCode && x.Period == "W" && x.Yearly == year)
                        .OrderBy(x => x.Week)
                        .Select(x => new
                        {
                            ID = x.ID,
                            Value = x.Value,
                            Remark = x.Remark,
                            x.Target,
                            Week = x.Week
                        })
                        .ToListAsync();
                        dataremarks = tbldata
                                     .Where(a => a.Value.ToDouble() > 0)
                     .Select(a => new Dataremark
                     {
                         ID = a.ID,
                         Value = a.Value,
                         Remark = a.Remark,
                         Week = a.Week,
                         ValueArray = new string[3] { a.Value, (a.Target.ToDouble() >= a.Value.ToDouble() ? false : true).ToString(), a.Target },
                     }).ToList();

                        if (start > 0 && end > 0)
                        {
                            dataremarks = dataremarks.Where(x => x.Week >= start && x.Week <= end).ToList();
                        }

                        var datasetsvm = new DatasetViewModel();
                        datasetsvm.KPIName = item.KPIName;
                        datasetsvm.Manager = obj.Manager;
                        datasetsvm.Owner = obj.Owner;
                        datasetsvm.Updater = obj.Updater;
                        datasetsvm.Sponsor = obj.Sponsor;
                        datasetsvm.Participant = obj.Participant;
                        datasetsvm.CategoryName = tblCategory.Name;
                        datasetsvm.CategoryCode = categorycode;
                        datasetsvm.KPILevelCode = item.KPILevelCode;

                        datasetsvm.Datasets = dataremarks;
                        datasetsvm.Period = "Weekly";

                        listDatasetViewModel.Add(datasetsvm);

                    }
                }
                else if (period.ToLower() == "m")
                {
                    foreach (var item in listKPILevelID)
                    {
                        var tblCategory = await _dbContext.Categories.FindAsync(item.CategoryID);
                        var categorycode = tblCategory.Code;

                        var obj = await GetAllOwner(categoryid, item.KPILevelID);
                        var tbldata = await _dbContext.Datas
                            .Where(x => x.KPILevelCode == item.KPILevelCode && x.Period == "M" && x.Yearly == year)
                          .OrderBy(x => x.Month)
                          .Select(x => new
                          {
                              ID = x.ID,
                              Value = x.Value,
                              Remark = x.Remark,
                              x.Target,
                              Month = x.Month,

                          }).ToListAsync();
                        dataremarks = tbldata
                          .Where(a => a.Value.ToDouble() > 0)
                         .Select(a => new Dataremark
                         {
                             ID = a.ID,
                             Value = a.Value,
                             Remark = a.Remark,
                             Week = a.Month,
                             ValueArray = new string[3] { a.Value, (a.Target.ToDouble() >= a.Value.ToDouble() ? false : true).ToString(), a.Target },
                         }).ToList();

                        if (start > 0 && end > 0)
                        {
                            dataremarks = dataremarks.Where(x => x.Week >= start && x.Week <= end).ToList();
                        }
                        var datasetsvm = new DatasetViewModel();
                        datasetsvm.KPIName = item.KPIName;
                        datasetsvm.Manager = obj.Manager;
                        datasetsvm.Owner = obj.Owner;
                        datasetsvm.Updater = obj.Updater;
                        datasetsvm.Sponsor = obj.Sponsor;
                        datasetsvm.Participant = obj.Participant;
                        datasetsvm.CategoryName = tblCategory.Name;
                        datasetsvm.CategoryCode = categorycode;
                        datasetsvm.KPILevelCode = item.KPILevelCode;

                        datasetsvm.Datasets = dataremarks;
                        datasetsvm.Period = "Monthly";

                        listDatasetViewModel.Add(datasetsvm);

                    }
                }
                else if (period.ToLower() == "q")
                {
                    foreach (var item in listKPILevelID)
                    {
                        var tblCategory = await _dbContext.Categories.FindAsync(item.CategoryID);
                        var categorycode = tblCategory.Code;

                        var obj = await GetAllOwner(categoryid, item.KPILevelID);
                        var tbldata = await _dbContext.Datas
                            .Where(x => x.KPILevelCode == item.KPILevelCode && x.Period == "Q" && x.Yearly == year)
                          .OrderBy(x => x.Quarter)
                         .Select(x => new
                         {
                             ID = x.ID,
                             Value = x.Value,
                             Remark = x.Remark,
                             x.Target,
                             Quarter = x.Quarter,

                         }).ToListAsync();
                        dataremarks = tbldata
                        .Where(a => a.Value.ToDouble() > 0)
                       .Select(a => new Dataremark
                       {
                           ID = a.ID,
                           Value = a.Value,
                           Remark = a.Remark,
                           Week = a.Quarter,
                           ValueArray = new string[3] { a.Value, (a.Target.ToDouble() >= a.Value.ToDouble() ? false : true).ToString(), a.Target },
                       }).ToList();
                        if (start > 0 && end > 0)
                        {
                            dataremarks = dataremarks.Where(x => x.Week >= start && x.Week <= end).ToList();
                        }
                        var datasetsvm = new DatasetViewModel();
                        datasetsvm.KPIName = item.KPIName;
                        datasetsvm.Manager = obj.Manager;
                        datasetsvm.Owner = obj.Owner;
                        datasetsvm.Updater = obj.Updater;
                        datasetsvm.Sponsor = obj.Sponsor;
                        datasetsvm.Participant = obj.Participant;
                        datasetsvm.CategoryName = tblCategory.Name;
                        datasetsvm.CategoryCode = categorycode;
                        datasetsvm.KPILevelCode = item.KPILevelCode;

                        datasetsvm.Datasets = dataremarks;
                        datasetsvm.Period = "Quarterly";

                        listDatasetViewModel.Add(datasetsvm);

                    }
                }
                else if (period.ToLower() == "y")
                {
                    foreach (var item in listKPILevelID)
                    {
                        var tblCategory = await _dbContext.Categories.FindAsync(item.CategoryID);
                        var categorycode = tblCategory.Code;

                        var obj = await GetAllOwner(categoryid, item.KPILevelID);
                        var tbldata = await _dbContext.Datas
                          .Where(x => x.KPILevelCode == item.KPILevelCode && x.Period == "Y" && x.Yearly == year)
                          .OrderBy(x => x.Year)
                          .Select(x => new
                          {
                              ID = x.ID,
                              Value = x.Value,
                              Remark = x.Remark,
                              x.Target,
                              Year = x.Year,

                          }).ToListAsync();
                        dataremarks = tbldata
                          .Where(a => a.Value.ToDouble() > 0)
                         .Select(a => new Dataremark
                         {
                             ID = a.ID,
                             Value = a.Value,
                             Remark = a.Remark,
                             Week = a.Year,
                             ValueArray = new string[3] { a.Value, (a.Target.ToDouble() >= a.Value.ToDouble() ? false : true).ToString(), a.Target },
                         }).ToList();
                        if (start > 0 && end > 0)
                        {
                            dataremarks = dataremarks.Where(x => x.Week >= start && x.Week <= end).ToList();
                        }
                        var datasetsvm = new DatasetViewModel();
                        datasetsvm.KPIName = item.KPIName;
                        datasetsvm.Manager = obj.Manager;
                        datasetsvm.Owner = obj.Owner;
                        datasetsvm.Updater = obj.Updater;
                        datasetsvm.Sponsor = obj.Sponsor;
                        datasetsvm.Participant = obj.Participant;
                        datasetsvm.CategoryName = tblCategory.Name;
                        datasetsvm.CategoryCode = categorycode;
                        datasetsvm.KPILevelCode = item.KPILevelCode;

                        datasetsvm.Datasets = dataremarks;
                        datasetsvm.Period = "Yearly";

                        listDatasetViewModel.Add(datasetsvm);
                    }
                }
            }
            return listDatasetViewModel;
        }

        public async Task<DataUserViewModel> GetAllOwner(int categoryID, int kpilevelID)
        {

            var manager = await _dbContext.Managers
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.FullName
                        }).Select(x => x.FullName).ToArrayAsync();


            var owner = await _dbContext.Owners
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.FullName
                        }).Select(x => x.FullName).ToArrayAsync();
            var updater = await _dbContext.Uploaders
                         .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                        .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.FullName
                        }).Select(x => x.FullName).ToArrayAsync();
            var sponsor = await _dbContext.Sponsors
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                       .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.FullName
                        }).Select(x => x.FullName).ToArrayAsync();
            var participant = await _dbContext.Participants
                        .Where(x => x.KPILevelID == kpilevelID && x.CategoryID == categoryID)
                       .Join(_dbContext.Users,
                        cat => cat.UserID,
                        user => user.ID,
                        (cat, user) => new
                        {
                            user.FullName
                        }).Select(x => x.FullName).ToArrayAsync();

            return new DataUserViewModel
            {
                Owner = owner.Count() != 0 ? string.Join(",", owner) : "N/A",
                Manager = manager.Count() != 0 ? string.Join(",", manager) : "N/A",
                Updater = updater.Count() != 0 ? string.Join(",", updater) : "N/A",
                Sponsor = sponsor.Count() != 0 ? string.Join(",", sponsor) : "N/A",
                Participant = participant.Count() != 0 ? string.Join(",", participant) : "N/A",
            };
        }
        public object Updaters(int kpilevelid, int categoryid)
        {
            //var user = _dbContext.Users;
            var list = _dbContext.Uploaders
                .Where(x => x.CategoryID == categoryid && x.KPILevelID == kpilevelid)
                .Join(_dbContext.Users,
                cat => cat.UserID,
                user => user.ID,
                (cat, user) => new
                {
                    user.FullName
                }).Select(x => x.FullName).ToArray();
            var count = list.Length;
            if (count == 0)
                return "N/A";
            else if (list == null)
                return "N/A";
            else
                return string.Join(",", list);

        }
        public object Owners(int kpilevelid, int categoryid)
        {
            //var user = _dbContext.Users;
            var list = _dbContext.Owners
                .Where(x => x.CategoryID == categoryid && x.KPILevelID == kpilevelid)
                .Join(_dbContext.Users,
                cat => cat.UserID,
                user => user.ID,
                (cat, user) => new
                {
                    user.FullName
                }).Select(x => x.FullName).ToArray();
            var count = list.Length;
            if (count == 0)
                return "N/A";
            else if (list == null)
                return "N/A";
            else
                return string.Join(",", list);
        }
        public object Managers(int kpilevelid, int categoryid)
        {
            //var user = _dbContext.Users;
            var list = _dbContext.Managers
                .Where(x => x.CategoryID == categoryid && x.KPILevelID == kpilevelid)
                .Join(_dbContext.Users,
                cat => cat.UserID,
                user => user.ID,
                (cat, user) => new
                {
                    user.FullName
                }).Select(x => x.FullName).ToArray();
            var count = list.Length;
            if (count == 0)
                return "N/A";
            else if (list == null)
                return "N/A";
            else
                return string.Join(",", list);
        }
        public object Sponsors(int kpilevelid, int categoryid)
        {
            //var user = _dbContext.Users;
            var list = _dbContext.Sponsors
                .Where(x => x.CategoryID == categoryid && x.KPILevelID == kpilevelid)
                .Join(_dbContext.Users,
                cat => cat.UserID,
                user => user.ID,
                (cat, user) => new
                {
                    user.FullName
                }).Select(x => x.FullName).ToArray();
            var count = list.Length;
            if (count == 0)
                return "N/A";
            else if (list == null)
                return "N/A";
            else
                return string.Join(",", list);
        }
        public object Participants(int kpilevelid, int categoryid)
        {
            //var user = _dbContext.Users;
            var list = _dbContext.Participants
                .Where(x => x.CategoryID == categoryid && x.KPILevelID == kpilevelid)
                .Join(_dbContext.Users,
                cat => cat.UserID,
                user => user.ID,
                (cat, user) => new
                {
                    user.FullName
                }).Select(x => x.FullName).ToArray();
            var count = list.Length;
            if (count == 0)
                return "N/A";
            else if (list == null)
                return "N/A";
            else
                return string.Join(",", list);
        }
        public ChartViewModel ListDatas(string kpilevelcode, string period, int? year, int? start, int? end, int? catid)
        {
            var currentYear = DateTime.Now.Year;
            var currentWeek = DateTime.Now.GetIso8601WeekOfYear();
            var currentMonth = DateTime.Now.Month;
            var currentQuarter = DateTime.Now.GetQuarter();

            string url = string.Empty;
            var yearly = year ?? 0;
            var categoryid = catid ?? 0;

            if (!string.IsNullOrEmpty(kpilevelcode) && !string.IsNullOrEmpty(period))
            {
                //label chartjs
                var item = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode);

                var PIC = Updaters(item.ID, categoryid);
                var Owner = Owners(item.ID, categoryid);
                var OwnerManagerment = Managers(item.ID, categoryid);
                var Sponsor = Sponsors(item.ID, categoryid);
                var Participant = Participants(item.ID, categoryid);

                var kpi = _dbContext.KPIs.FirstOrDefault(x => x.ID == item.KPIID);
                var kpiname = string.Empty;
                if (kpi != null)
                    kpiname = kpi.Name;
                var label = _dbContext.Levels.FirstOrDefault(x => x.ID == item.LevelID).Name.ToSafetyString();
                //datasets chartjs
                var model = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Yearly == yearly);

                var unit = _dbContext.KPIs.FirstOrDefault(x => x.ID == item.KPIID);
                if (unit == null) return new ChartViewModel();
                var unitName = _dbContext.Units.FirstOrDefault(x => x.ID == unit.Unit).Name.ToSafetyString();

                if (period == "W".ToUpper())
                {
                    var standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.WeeklyChecked == true).WeeklyStandard;
                    var statusFavourites = _dbContext.Favourites.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period) == null ? false : true;

                    //Tạo ra 1 mảng tuần mặc định bằng 0
                    List<string> listDatasets = new List<string>();

                    //labels của chartjs mặc định có 53 phần tử
                    List<string> listLabels = new List<string>();

                    //labels của chartjs mặc định có 53 phần tử
                    List<string> listTargets = new List<string>();

                    //labels của chartjs mặc định có 53 phần tử
                    List<int> listStandards = new List<int>();

                    var Dataremarks = new List<Dataremark>();
                    //Search range
                    if (start > 0 && end > 0)
                    {
                        model = model.Where(x => x.Yearly == year && x.Week >= start && x.Week <= end);

                        var listValues = model.Where(x => x.Period == "W").OrderBy(x => x.Week).Select(x => x.Value).ToArray();
                        var listLabelsW = model.Where(x => x.Period == "W").OrderBy(x => x.Week).Select(x => x.Week).ToArray();
                        var listtargetsW = model.Where(x => x.Period == "W").OrderBy(x => x.Week).Select(x => x.Target).ToArray();
                        for (int i = 0; i < listValues.Length; i++)
                        {
                            listStandards.Add(standard);
                        }
                        //Convert sang list string
                        var listStringLabels = Array.ConvertAll(listLabelsW, x => x.ToSafetyString());

                        //Convert sang list string
                        var listStringTargets = Array.ConvertAll(listtargetsW, x => x.ToSafetyString());

                        listDatasets.AddRange(listValues);
                        listLabels.AddRange(listStringLabels);
                        listTargets.AddRange(listStringTargets);

                        Dataremarks = model
                           .Where(x => x.Period == "W")
                           .OrderBy(x => x.Week)
                           .Select(x => new Dataremark
                           {
                               ID = x.ID,
                               Value = x.Value,
                               Remark = x.Remark,
                               Week = x.Week
                           }).ToList();

                    }
                    return new ChartViewModel
                    {
                        Unit = unitName,
                        Standard = standard,
                        Dataremarks = Dataremarks,
                        datasets = listDatasets.ToArray(),
                        labels = listLabels.ToArray(),
                        label = label,
                        targets = listTargets.ToArray(),
                        standards = listStandards.ToArray(),
                        kpiname = kpiname,
                        period = "W",
                        kpilevelcode = kpilevelcode,
                        statusfavorite = statusFavourites,
                        PIC = PIC,
                        Owner = Owner,
                        OwnerManagerment = OwnerManagerment,
                        Sponsor = Sponsor,
                        Participant = Participant
                    };

                }
                else if (period == "M".ToUpper())
                {
                    var standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.MonthlyChecked == true).MonthlyStandard;
                    var statusFavourites = _dbContext.Favourites.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period) == null ? false : true;

                    //Tạo ra 1 mảng tuần mặc định bằng 0
                    List<string> listDatasets = new List<string>();

                    //labels của chartjs mặc định có 12 phần tử = 0
                    List<string> listLabels = new List<string>();

                    //labels của chartjs mặc định có 12 phần tử
                    List<string> listTargets = new List<string>();
                    //Tạo ra 1 mảng tuần mặc định bằng 0
                    List<int> listStandards = new List<int>();
                    var Dataremarks = new List<Dataremark>();


                    //Search range
                    if (start > 0 && end > 0)
                    {
                        model = model.Where(x => x.Yearly == year && x.Month >= start && x.Month <= end);

                        var listValues = model.Where(x => x.Period == "M").OrderBy(x => x.Month).Select(x => x.Value).ToArray();
                        var listLabelsW = model.Where(x => x.Period == "M").OrderBy(x => x.Month).Select(x => x.Month).ToArray();
                        var listtargetsW = model.Where(x => x.Period == "M").OrderBy(x => x.Month).Select(x => x.Target).ToArray();
                        //Convert sang list string
                        var listStringTargets = Array.ConvertAll(listtargetsW, x => x.ToSafetyString());
                        listTargets.AddRange(listStringTargets);

                        for (int i = 0; i < listValues.Length; i++)
                        {
                            listStandards.Add(standard);
                        }
                        foreach (var a in listLabelsW)
                        {
                            switch (a)
                            {
                                case 1:
                                    listLabels.Add("Jan");
                                    break;
                                case 2:
                                    listLabels.Add("Feb"); break;
                                case 3:
                                    listLabels.Add("Mar"); break;
                                case 4:
                                    listLabels.Add("Apr"); break;
                                case 5:
                                    listLabels.Add("May");
                                    break;
                                case 6:
                                    listLabels.Add("Jun"); break;
                                case 7:
                                    listLabels.Add("Jul"); break;
                                case 8:
                                    listLabels.Add("Aug"); break;
                                case 9:
                                    listLabels.Add("Sep");
                                    break;
                                case 10:
                                    listLabels.Add("Oct"); break;
                                case 11:
                                    listLabels.Add("Nov"); break;
                                case 12:
                                    listLabels.Add("Dec"); break;
                            }
                        }

                        listDatasets.AddRange(listValues);

                        Dataremarks = model
                           .Where(x => x.Period == "M")
                           .OrderBy(x => x.Month)
                           .Select(x => new Dataremark
                           {
                               ID = x.ID,
                               Value = x.Value,
                               Remark = x.Remark,
                               Month = x.Month
                           }).ToList();
                    }

                    return new ChartViewModel
                    {
                        Unit = unitName,
                        Standard = standard,
                        Dataremarks = Dataremarks,
                        datasets = listDatasets.ToArray(),
                        labels = listLabels.ToArray(),
                        targets = listTargets.ToArray(),
                        standards = listStandards.ToArray(),
                        label = label,
                        kpiname = kpiname,
                        period = "M",
                        kpilevelcode = kpilevelcode,
                        statusfavorite = statusFavourites,
                        PIC = PIC,
                        Owner = Owner,
                        OwnerManagerment = OwnerManagerment,
                        Sponsor = Sponsor,
                        Participant = Participant
                    };
                }
                else if (period == "Q".ToUpper())
                {
                    var standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.QuarterlyChecked == true).QuarterlyStandard;
                    var statusFavourites = _dbContext.Favourites.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period) == null ? false : true;

                    //Tạo ra 1 mảng tuần mặc định bằng 0
                    List<string> listDatasets = new List<string>();

                    //labels của chartjs mặc định có 53 phần tử = 0
                    List<string> listLabels = new List<string>();

                    //labels của chartjs mặc định có 12 phần tử
                    List<string> listTargets = new List<string>();
                    //labels của chartjs mặc định có 12 phần tử
                    List<int> listStandards = new List<int>();
                    var Dataremarks = new List<Dataremark>();


                    if (year > 0 && start > 0 && end > 0)
                    {
                        model = model.Where(x => x.Yearly == year && x.Quarter >= start && x.Quarter <= end);
                        var listValues = model.Where(x => x.Period == "Q").OrderBy(x => x.Quarter).Select(x => x.Value).ToArray();
                        var listLabelsW = model.Where(x => x.Period == "Q").OrderBy(x => x.Quarter).Select(x => x.Quarter).ToArray();
                        listDatasets.AddRange(listValues);
                        var listtargetsW = model.Where(x => x.Period == "Q").OrderBy(x => x.Quarter).Select(x => x.Target).ToArray();

                        //Convert sang list string
                        var listStringTargets = Array.ConvertAll(listtargetsW, x => x.ToSafetyString());
                        listTargets.AddRange(listStringTargets);
                        for (int i = 0; i < listValues.Length; i++)
                        {
                            listStandards.Add(standard);
                        }
                        foreach (var i in listLabelsW)
                        {
                            switch (i)
                            {
                                case 1:
                                    listLabels.Add("Quarter 1"); break;
                                case 2:
                                    listLabels.Add("Quarter 2"); break;
                                case 3:
                                    listLabels.Add("Quarter 3"); break;
                                case 4:
                                    listLabels.Add("Quarter 4"); break;
                            }
                        }
                        Dataremarks = model
                         .Where(x => x.Period == "Q")
                         .OrderBy(x => x.Quarter)
                         .Select(x => new Dataremark
                         {
                             ID = x.ID,
                             Value = x.Value,
                             Remark = x.Remark,
                             Quater = x.Quarter
                         }).ToList();
                    }

                    return new ChartViewModel
                    {
                        Unit = unitName,
                        Standard = standard,
                        Dataremarks = Dataremarks,
                        datasets = listDatasets.ToArray(),
                        labels = listLabels.ToArray(),
                        targets = listTargets.ToArray(),
                        standards = listStandards.ToArray(),
                        label = label,
                        kpiname = kpiname,
                        period = "Q",
                        kpilevelcode = kpilevelcode,
                        statusfavorite = statusFavourites,
                        PIC = PIC,
                        Owner = Owner,
                        OwnerManagerment = OwnerManagerment,
                        Sponsor = Sponsor,
                        Participant = Participant
                    };
                }
                else if (period == "Y".ToUpper())
                {
                    if (start > 0 && end > 0)
                    {
                        model = model.Where(x => x.Year >= start && x.Year <= end);
                    }
                    var datasets = model.Where(x => x.Yearly == year && x.Period == "Y").OrderBy(x => x.Year).Select(x => x.Value).ToArray();
                    var Dataremarks = model
                      .Where(x => x.Period == "Y")
                      .OrderBy(x => x.Year)
                      .Select(x => new Dataremark
                      {
                          ID = x.ID,
                          Value = x.Value,
                          Remark = x.Remark,
                          Year = x.Year
                      }).ToList();
                    //data: labels chartjs
                    var listlabels = model.Where(x => x.Period == "Y").OrderBy(x => x.Year).Select(x => x.Year).ToArray();
                    var labels = Array.ConvertAll(listlabels, x => x.ToSafetyString());
                    var listtargetsW = model.Where(x => x.Period == "Y").OrderBy(x => x.Year).Select(x => x.Target).ToArray();
                    //labels của chartjs mặc định có 12 phần tử
                    List<string> listTargets = new List<string>();
                    //Convert sang list string
                    var listStringTargets = Array.ConvertAll(listtargetsW, x => x.ToSafetyString());
                    listTargets.AddRange(listStringTargets);
                    return new ChartViewModel
                    {
                        Unit = unitName,
                        Standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.YearlyChecked == true).YearlyStandard,
                        Dataremarks = Dataremarks,
                        datasets = datasets,
                        labels = labels,
                        label = label,
                        targets = listTargets.ToArray(),
                        kpiname = kpiname,
                        period = "Y",
                        kpilevelcode = kpilevelcode,
                        statusfavorite = _dbContext.Favourites.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period) == null ? false : true,
                        PIC = PIC,
                        Owner = Owner,
                        OwnerManagerment = OwnerManagerment,
                        Sponsor = Sponsor,
                        Participant = Participant
                    };
                }
                else
                {
                    return new ChartViewModel();
                }
            }
            else
            {
                return new ChartViewModel();
            }
        }

        public ChartViewModel Compare(string kpilevelcode, string period)
        {

            var model2 = new DataCompareViewModel();

            var model = new ChartViewModel();

            var item = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode);
            model.kpiname = _dbContext.KPIs.Find(item.KPIID).Name;
            model.label = _dbContext.Levels.FirstOrDefault(x => x.ID == item.LevelID).Name;
            model.kpilevelcode = kpilevelcode;

            var unit = _dbContext.KPIs.FirstOrDefault(x => x.ID == item.KPIID).Unit;
            var unitName = _dbContext.Units.FirstOrDefault(x => x.ID == unit).Name;

            if (period == "W")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();

                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Week).Select(x => new { x.Value, x.Week }).ToList();
                foreach (var valueWeek in datas)
                {
                    listDatasets.Add(valueWeek.Value);
                    listLabels.Add(valueWeek.Week.ToString());
                }

                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.period = period;

            }
            if (period == "M")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();


                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Month).Select(x => new { x.Month, x.Value }).ToList();
                foreach (var monthly in datas)
                {
                    listDatasets.Add(monthly.Value);
                    switch (monthly.Month)
                    {
                        case 1:
                            listLabels.Add("Jan"); break;
                        case 2:
                            listLabels.Add("Feb"); break;
                        case 3:
                            listLabels.Add("Mar"); break;
                        case 4:
                            listLabels.Add("Apr"); break;
                        case 5:
                            listLabels.Add("May"); break;
                        case 6:
                            listLabels.Add("Jun"); break;
                        case 7:
                            listLabels.Add("Jul"); break;
                        case 8:
                            listLabels.Add("Aug"); break;
                        case 9:
                            listLabels.Add("Sep");
                            break;
                        case 10:
                            listLabels.Add("Oct"); break;
                        case 11:
                            listLabels.Add("Nov"); break;
                        case 12:
                            listLabels.Add("Dec"); break;
                    }
                }
                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.period = period;
            }
            if (period == "Q")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();
                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Quarter).Select(x => new { x.Quarter, x.Value }).ToList();
                foreach (var quarterly in datas)
                {
                    listDatasets.Add(quarterly.Value);
                    switch (quarterly.Quarter)
                    {
                        case 1:
                            listLabels.Add("Quarter 1"); break;
                        case 2:
                            listLabels.Add("Quarter 2"); break;
                        case 3:
                            listLabels.Add("Quarter 3"); break;
                        case 4:
                            listLabels.Add("Quarter 4"); break;
                    }
                }
                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.period = period;
                model.Unit = unitName;
                model.Standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.QuarterlyChecked == true).QuarterlyStandard;
            }
            if (period == "Y")
            {
                var datasetsKPILevel1 = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Year).Select(x => x.Value).ToArray();
                var labelsKPILevel1 = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Year).Select(x => x.Year).ToArray();
                var labels1 = Array.ConvertAll(labelsKPILevel1, x => x.ToSafetyString());
                model.datasets = datasetsKPILevel1;
                model.labels = labels1;
                model.period = period;
            }
            return model;
        }
       
        public ChartViewModel2 Compare2(string kpilevelcode, string period)
        {

            var model2 = new DataCompareViewModel();

            var model = new ChartViewModel2();

            var item = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode);
            model.kpiname = _dbContext.KPIs.Find(item.KPIID).Name;
            model.label = _dbContext.Levels.FirstOrDefault(x => x.ID == item.LevelID).Name;
            model.kpilevelcode = kpilevelcode;

            var unit = _dbContext.KPIs.FirstOrDefault(x => x.ID == item.KPIID).Unit;
            var unitName = _dbContext.Units.FirstOrDefault(x => x.ID == unit).Name;

            if (period == "W")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> targets = new List<string>();

                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Week).Select(x => new { x.Value, x.Week, x.Target }).ToList();
                foreach (var valueWeek in datas)
                {
                    listDatasets.Add(valueWeek.Value);
                    listLabels.Add(valueWeek.Week.ToString());
                    targets.Add(valueWeek.Target.ToString());
                }

                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.targets = targets.ToArray();
                model.period = period;

            }
            if (period == "M")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();
                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> targets = new List<string>();

                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Month).Select(x => new { x.Month, x.Value, x.Target }).ToList();
                foreach (var monthly in datas)
                {
                    listDatasets.Add(monthly.Value);
                    switch (monthly.Month)
                    {
                        case 1:
                            listLabels.Add("Jan"); break;
                        case 2:
                            listLabels.Add("Feb"); break;
                        case 3:
                            listLabels.Add("Mar"); break;
                        case 4:
                            listLabels.Add("Apr"); break;
                        case 5:
                            listLabels.Add("May"); break;
                        case 6:
                            listLabels.Add("Jun"); break;
                        case 7:
                            listLabels.Add("Jul"); break;
                        case 8:
                            listLabels.Add("Aug"); break;
                        case 9:
                            listLabels.Add("Sep");
                            break;
                        case 10:
                            listLabels.Add("Oct"); break;
                        case 11:
                            listLabels.Add("Nov"); break;
                        case 12:
                            listLabels.Add("Dec"); break;
                    }
                }
                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.period = period;
                model.targets = targets.ToArray();
            }
            if (period == "Q")
            {
                //Tạo ra 1 mảng tuần mặc định bằng 0
                List<string> listDatasets = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> listLabels = new List<string>();

                //labels của chartjs mặc định có 53 phần tử = 0
                List<string> targets = new List<string>();
                var datas = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Quarter).Select(x => new { x.Quarter, x.Value, x.Target }).ToList();
                foreach (var quarterly in datas)
                {
                    listDatasets.Add(quarterly.Value);
                    switch (quarterly.Quarter)
                    {
                        case 1:
                            listLabels.Add("Quarter 1"); break;
                        case 2:
                            listLabels.Add("Quarter 2"); break;
                        case 3:
                            listLabels.Add("Quarter 3"); break;
                        case 4:
                            listLabels.Add("Quarter 4"); break;
                    }
                }
                model.datasets = listDatasets.ToArray();
                model.labels = listLabels.ToArray();
                model.period = period;
                model.Unit = unitName;
                model.targets = targets.ToArray();
                model.Standard = _dbContext.KPILevels.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.QuarterlyChecked == true).QuarterlyStandard;
            }
            if (period == "Y")
            {
                var datasetsKPILevel1 = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Year).Select(x => x.Value).ToArray();
                var labelsKPILevel1 = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Year).Select(x => x.Year).ToArray();
                var labels1 = Array.ConvertAll(labelsKPILevel1, x => x.ToSafetyString());
                var targets = _dbContext.Datas.Where(x => x.KPILevelCode == kpilevelcode && x.Period == period).OrderBy(x => x.Year).Select(x => x.Target).ToArray();

                model.datasets = datasetsKPILevel1;
                model.labels = labels1;
                model.period = period;
                model.targets = Array.ConvertAll(targets, x => x.ToSafetyString());
            }
            return model;
        }
        public List<ChartViewModel2> Compare2(string obj)
        {
            obj = obj.ToSafetyString();
            var listChartVM = new List<ChartViewModel2>();
            var value = obj.Split('-');

            var size = value.Length;
            foreach (var item in value)
            {
                var kpilevelcode = item.Split(',')[0];
                var period = item.Split(',')[1];
                listChartVM.Add(Compare2(kpilevelcode, period));
            }
            return listChartVM;
        }
        public DataCompareViewModel Compare(string obj)
        {
            var listChartVM = new List<ChartViewModel>();
            var model = new DataCompareViewModel();
            obj = obj.ToSafetyString();

            var value = obj.Split('-');
            model.Period = value[1].Split(',')[1];
            var size = value.Length;
            foreach (var item in value)
            {
                var kpilevelcode = item.Split(',')[0];
                var period = item.Split(',')[1];
                listChartVM.Add(Compare(kpilevelcode, period));
                model.list1 = Compare(kpilevelcode, period);
            }

            if (size == 2)
            {
                var kpilevelcode1 = value[0].Split(',')[0];
                var period1 = value[1].Split(',')[1];
                var kpilevelcode2 = value[1].Split(',')[0];
                var period2 = value[1].Split(',')[1];
                model.list1 = Compare(kpilevelcode1, period1);
                model.list2 = Compare(kpilevelcode2, period2);

                return model;
            }
            else if (size == 3)
            {
                var kpilevelcode1 = value[0].Split(',')[0];
                var period1 = value[1].Split(',')[1];

                var kpilevelcode2 = value[1].Split(',')[0];
                var period2 = value[1].Split(',')[1];

                var kpilevelcode3 = value[2].Split(',')[0];
                var period3 = value[2].Split(',')[1];
                model.list1 = Compare(kpilevelcode1, period1);
                model.list2 = Compare(kpilevelcode2, period2);
                model.list3 = Compare(kpilevelcode3, period3);
                return model;

            }
            else if (size == 4)
            {
                var kpilevelcode1 = value[0].Split(',')[0];
                var period1 = value[1].Split(',')[1];

                var kpilevelcode2 = value[1].Split(',')[0];
                var period2 = value[1].Split(',')[1];

                var kpilevelcode3 = value[2].Split(',')[0];
                var period3 = value[2].Split(',')[1];

                var kpilevelcode4 = value[3].Split(',')[0];
                var period4 = value[3].Split(',')[1];
                model.list1 = Compare(kpilevelcode1, period1);
                model.list2 = Compare(kpilevelcode2, period2);
                model.list3 = Compare(kpilevelcode3, period3);
                model.list4 = Compare(kpilevelcode4, period4);
                return model;
            }
            else
            {
                return new DataCompareViewModel();
            }
        }
       
        public async Task<object> Remark(int dataid)
        {
            var model = await _dbContext.Datas.FirstOrDefaultAsync(x => x.ID == dataid);
            return new
            {
                model = model,
                users = await _dbContext.Users.Where(x => x.Permission > 1).ToListAsync()
            };
        }
        public async Task<bool> UpdateRemark(int dataid, string remark)
        {
            var model = await _dbContext.Datas.FirstOrDefaultAsync(x => x.ID == dataid);
            try
            {
                model.Remark = remark.ToSafetyString();
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<object> ListKPIUpload(int updaterid, int page, int pageSize)
        {
            var datas = await _dbContext.Datas.ToListAsync();
            var model = (await (from u in _dbContext.Uploaders.Where(x => x.UserID == updaterid)
                                join item in _dbContext.KPILevels on u.KPILevelID equals item.ID
                                join cat in _dbContext.CategoryKPILevels.Where(x => x.Status == true) on u.KPILevelID equals cat.KPILevelID
                                join kpi in _dbContext.KPIs on item.KPIID equals kpi.ID
                                select new
                                {
                                    KPILevelID = u.KPILevelID,
                                    KPIName = kpi.Name,
                                    StateDataW = item.WeeklyChecked ?? false,
                                    StateDataM = item.MonthlyChecked ?? false,
                                    StateDataQ = item.QuarterlyChecked ?? false,
                                    StateDataY = item.YearlyChecked ?? false,
                                }).ToListAsync())
                         .Select(x => new ListKPIUploadViewModel
                         {
                             KPILevelID = x.KPILevelID,
                             KPIName = x.KPIName,
                             StateW = datas.Max(x => x.Week) > 0 ? true : false,
                             StateM = datas.Max(x => x.Month) > 0 ? true : false,
                             StateQ = datas.Max(x => x.Quarter) > 0 ? true : false,
                             StateY = datas.Max(x => x.Year) > 0 ? true : false,

                             StateDataW = x.StateDataW,
                             StateDataM = x.StateDataM,
                             StateDataQ = x.StateDataQ,
                             StateDataY = x.StateDataY
                         }).DistinctBy(p => p.KPIName).ToList();
            //bảng uploader có nhiều KPILevel trùng nhau vì 1 KPILevel thuộc nhiều Category khác nhau 
            //nên ta phải distinctBy KPILevelID để lấy ra danh sách KPI không bị trùng nhau vì yêu cầu chỉ cần lấy ra KPI để upload dữ liệu
            ////Mỗi KPILevel ứng với 1 KPI khác nhau
            int totalRow = model.Count();

            model = model.OrderByDescending(x => x.KPIName)
             .Skip((page - 1) * pageSize)
             .Take(pageSize).ToList();

            return new
            {
                data = model,
                page,
                pageSize,
                status = true,
                total = totalRow,
                isUpdater = true

            };
        }

        public async Task<object> UpLoadKPILevel(int userid, int page, int pageSize)
        {
            var datas = _dbContext.Datas;
            var model = await (from u in _dbContext.Users
                               join l in _dbContext.Levels on u.LevelID equals l.ID
                               join item in _dbContext.KPILevels on l.ID equals item.LevelID
                               join kpi in _dbContext.KPIs on item.KPIID equals kpi.ID
                               where u.ID == userid && item.Checked == true
                               select new KPIUploadViewModel
                               {
                                   KPIName = kpi.Name,
                                   StateW = item.WeeklyChecked == true && datas.Where(x => x.KPILevelCode == item.KPILevelCode).Max(x => x.Week) > 0 ? true : false,
                                   StateM = item.MonthlyChecked == true && datas.Where(x => x.KPILevelCode == item.KPILevelCode).Max(x => x.Month) > 0 ? true : false,
                                   StateQ = item.QuarterlyChecked == true && datas.Where(x => x.KPILevelCode == item.KPILevelCode).Max(x => x.Quarter) > 0 ? true : false,
                                   StateY = item.YearlyChecked == true && datas.Where(x => x.KPILevelCode == item.KPILevelCode).Max(x => x.Year) > 0 ? true : false,

                                   StateDataW = item.WeeklyChecked ?? false,
                                   StateDataM = item.MonthlyChecked ?? false,
                                   StateDataQ = item.QuarterlyChecked ?? false,
                                   StateDataY = item.YearlyChecked ?? false,

                               }).ToListAsync();
            int totalRow = model.Count();
            model = model.OrderByDescending(x => x.KPIName)
              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();
            var vm = new WorkplaceViewModel()
            {
                KPIUpLoads = model,
                total = totalRow,
                page = page,
                pageSize = pageSize
            };
            return vm;
        }

     
        public string GetValueData(string KPILevelCode, string CharacterPeriod, int period)
        {
            var value = CharacterPeriod.ToSafetyString();
            string obj = "0";
            switch (value)
            {
                case "W":
                    var item = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "W" && x.Week == period);
                    if (item != null)
                        obj = item.Value;
                    break;
                case "M":
                    var item1 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "M" && x.Month == period);
                    if (item1 != null)
                        obj = item1.Value;
                    break;
                case "Q":
                    var item2 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "Q" && x.Quarter == period);
                    if (item2 != null)
                        obj = item2.Value;
                    break;
                case "Y":
                    var item3 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "Y" && x.Year == period);
                    if (item3 != null)
                        obj = item3.Value;
                    break;
            }
            return obj;
        }
        public string GetTargetData(string KPILevelCode, string CharacterPeriod, int period)
        {
            var value = CharacterPeriod.ToSafetyString();
            string obj = "0";
            switch (value)
            {
                case "W":
                    var item = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "W" && x.Week == period);
                    if (item != null)
                        obj = item.Target;
                    break;
                case "M":
                    var item1 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "M" && x.Month == period);
                    if (item1 != null)
                        obj = item1.Target;
                    break;
                case "Q":
                    var item2 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "Q" && x.Quarter == period);
                    if (item2 != null)
                        obj = item2.Target;
                    break;
                case "Y":
                    var item3 = _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == KPILevelCode && x.Period == "Y" && x.Year == period);
                    if (item3 != null)
                        obj = item3.Target;
                    break;
            }
            return obj;
        }
        public List<DataExportViewModel> DataExport(int userid)
        {
            var datas = _dbContext.Datas;
            var kpis = _dbContext.KPIs;
            var model = (from u in _dbContext.Uploaders.Where(x => x.UserID == userid).DistinctBy(x => x.KPILevelID)
                         join kpiLevel in _dbContext.KPILevels on u.KPILevelID equals kpiLevel.ID
                         join cat in _dbContext.CategoryKPILevels.Where(x => x.Status == true) on u.KPILevelID equals cat.KPILevelID
                         join kpi in _dbContext.KPIs on kpiLevel.KPIID equals kpi.ID
                         join l in _dbContext.Levels on kpiLevel.LevelID equals l.ID
                         where kpiLevel.Checked == true
                         select new DataExportViewModel
                         {
                             Area = l.Name,
                             KPILevelCode = kpiLevel.KPILevelCode,
                             KPIName = kpi.Name,
                             StateW = kpiLevel.WeeklyChecked ?? false,
                             StateM = kpiLevel.MonthlyChecked ?? false,
                             StateQ = kpiLevel.QuarterlyChecked ?? false,
                             StateY = kpiLevel.YearlyChecked ?? false,

                             PeriodValueW = datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).FirstOrDefault() != null ? (int?)datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).Max(x => x.Week) ?? 0 : 0,
                             PeriodValueM = datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).FirstOrDefault() != null ? (int?)datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).Max(x => x.Month) ?? 0 : 0,
                             PeriodValueQ = datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).FirstOrDefault() != null ? (int?)datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).Max(x => x.Quarter) ?? 0 : 0,
                             PeriodValueY = datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).FirstOrDefault() != null ? (int?)datas.Where(x => x.KPILevelCode == kpiLevel.KPILevelCode).Max(x => x.Year) ?? 0 : 0,

                             UploadTimeW = kpiLevel.Weekly,
                             UploadTimeM = kpiLevel.Monthly,
                             UploadTimeQ = kpiLevel.Quarterly,
                             UploadTimeY = kpiLevel.Yearly,
                             //TargetValueW = kpi.Unit == 1 ? "not require" : "require"
                         }).ToList();

            return model;
        }
        /// <summary>
        /// Kiểm tra tồn tại Data
        /// </summary>
        /// <param name="kpilevelcode"></param>
        /// <param name="period"></param>
        /// <param name="periodValue"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public Models.EF.Data IsExistKPILevelData(string kpilevelcode, string period, int periodValue, int year)
        {
            switch (period.ToSafetyString().ToUpper())
            {
                case "W":
                    return _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period && x.Week == periodValue && x.Yearly == year);
                case "M":
                    return _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period && x.Month == periodValue && x.Yearly == year);
                case "Q":
                    return _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period && x.Quarter == periodValue && x.Yearly == year);
                case "Y":
                    return _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == kpilevelcode && x.Period == period && x.Year == periodValue && x.Yearly == year);

                default:
                    return null;
            }
        }
        /// <summary>
        /// Lọc dữ liệu "Tạo mới" và "Cập nhật" đọc từ file Excel.
        /// </summary>
        /// <param name="entity">Danh sách đọc từ excel</param>
        /// <returns>Trả về 2 danh sách "Tạo mới" và "Cập nhật" đọc từ file Excel</returns>
        public Tuple<List<Models.EF.Data>, List<Models.EF.Data>> CreateOrUpdateData(List<UploadDataViewModel> entity)
        {
            List<Models.EF.Data> listCreateData = new List<Models.EF.Data>();
            List<Models.EF.Data> listUpdateData = new List<Models.EF.Data>();
            List<UploadDataViewModel> list = new List<UploadDataViewModel>();
            foreach (var item in entity)
            {
                var value = item.KPILevelCode;
                var kpilevelcode = value.Substring(0, value.Length - 1);
                var period = value.Substring(value.Length - 1, 1);
                var year = item.Year; //dữ liệu trong năm vd: năm 2019
                var valuePeriod = item.Value;
                var target = item.TargetValue;
                //query trong bảng data nếu updated thì update lại db
                var isExistData = IsExistKPILevelData(kpilevelcode, period, item.PeriodValue, year);
                switch (period)
                {
                    case "W":
                        var dataW = new Models.EF.Data();
                        dataW.KPILevelCode = kpilevelcode;
                        dataW.Value = item.Value;
                        dataW.Week = item.PeriodValue;
                        dataW.Yearly = year;
                        dataW.CreateTime = item.CreateTime;
                        dataW.Period = period;
                        if (item.TargetValue.ToDouble() > 0)
                            dataW.Target = item.TargetValue.ToString();
                        else dataW.Target = "0";
                        if (isExistData == null)
                            listCreateData.Add(dataW);
                        else if (isExistData != null)
                        {
                            if (dataW.Value != valuePeriod || dataW.Target != target)
                            {
                                dataW.ID = isExistData.ID;
                                listUpdateData.Add(dataW);
                            }
                        }
                        else
                            list.Add(item);
                        break;
                    case "M":
                        var dataM = new Models.EF.Data();
                        dataM.KPILevelCode = kpilevelcode;
                        dataM.Value = item.Value;
                        dataM.Month = item.PeriodValue;
                        dataM.Yearly = year;
                        dataM.CreateTime = item.CreateTime;
                        dataM.Period = period;

                        if (item.TargetValue.ToDouble() > 0)
                            dataM.Target = item.TargetValue.ToString();
                        else dataM.Target = "0";
                        if (isExistData == null)
                            listCreateData.Add(dataM);
                        else if (isExistData != null)
                        {
                            if (isExistData.Value != valuePeriod || isExistData.Target != target)
                            {
                                dataM.ID = isExistData.ID;
                                listUpdateData.Add(dataM);
                            }
                        }
                        else
                            list.Add(item);
                        break;
                    case "Q":
                        var dataQ = new Models.EF.Data();
                        dataQ.KPILevelCode = kpilevelcode;
                        dataQ.Value = item.Value;
                        dataQ.Quarter = item.PeriodValue;
                        dataQ.Yearly = year;
                        dataQ.CreateTime = item.CreateTime;
                        dataQ.Period = period;

                        if (item.TargetValue.ToDouble() > 0)
                            dataQ.Target = item.TargetValue.ToString();
                        else dataQ.Target = "0";
                        if (isExistData == null)
                            listCreateData.Add(dataQ);
                        else if (isExistData != null)
                        {
                            if (isExistData.Value != valuePeriod || isExistData.Target != target)
                            {
                                dataQ.ID = isExistData.ID;
                                listUpdateData.Add(dataQ);
                            }
                        }
                        else
                            list.Add(item);
                        break;
                    case "Y":
                        var dataY = new Models.EF.Data();
                        dataY.KPILevelCode = kpilevelcode;
                        dataY.Value = item.Value;
                        dataY.Year = item.PeriodValue;
                        dataY.Yearly = year;
                        dataY.CreateTime = item.CreateTime;
                        dataY.Period = period;

                        if (item.TargetValue.ToDouble() > 0)
                            dataY.Target = item.TargetValue.ToString();
                        else dataY.Target = "0";
                        if (isExistData == null)
                            listCreateData.Add(dataY);
                        else if (isExistData != null)
                        {
                            if (isExistData.Value != valuePeriod || isExistData.Target != target)
                            {
                                dataY.ID = isExistData.ID;
                                listUpdateData.Add(dataY);
                            }
                        }
                        else
                            list.Add(item);
                        break;
                    default:

                        break;
                }
            }

            return Tuple.Create(listCreateData, listUpdateData);
        }
        public async Task<string> GetKPIName(string code)
        {
            var item = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == code && x.Checked == true);
            var kpilevelID = item.KPIID;
            var listCategory = await _dbContext.KPIs.Where(x => x.ID == kpilevelID).FirstOrDefaultAsync();
            return listCategory.Name;
        }
        /// <summary>
        /// Hàm này dùng để tìm CÁC category của mỗi kpilevelcode
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<List<int>> GetAllCategoryByKPILevel(string code)
        {
            var item = await _dbContext.KPILevels.FirstOrDefaultAsync(x => x.KPILevelCode == code && x.Checked == true);
            var kpilevelID = item.ID;
            var listCategory = await _dbContext.CategoryKPILevels.Where(x => x.KPILevelID == kpilevelID && x.Status == true).Select(x => x.CategoryID).ToListAsync();
            return listCategory;
        }
        /// <summary>
        /// Hàm này dùng để tạo url chuyển tới trang ChartPriod của từng data khi update hoặc create
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private async Task<List<string[]>> ListURLToChartPriodAsync(List<Data> datas)
        {
            var listURLToChartPeriod = new List<string[]>();
            string url = string.Empty;
            var http = _configuration.GetSection("AppSettings:URL").Value.ToSafetyString();

            foreach (var item in datas.DistinctBy(x => x.KPILevelCode))
            {
                var oc = _levelService.GetNode(item.KPILevelCode);
                var kpiname = await GetKPIName(item.KPILevelCode);
                var listCategories = await GetAllCategoryByKPILevel(item.KPILevelCode);
                if (item.Period == "W")
                {

                    foreach (var cat in listCategories)
                    {
                        url = http + $"/ChartPeriod/?kpilevelcode={item.KPILevelCode}&catid={cat}&period={item.Period}&year={DateTime.Now.Year}&start=1&end=53";
                        listURLToChartPeriod.Add(new string[3]
                                           {
                                url,kpiname,oc
                                           });
                    }

                }
                if (item.Period == "M")
                {
                    foreach (var cat in listCategories)
                    {
                        url = http + $"/ChartPeriod/?kpilevelcode={item.KPILevelCode}&catid={cat}&period={item.Period}&year={DateTime.Now.Year}&start=1&end=12";
                        listURLToChartPeriod.Add(new string[3]
                        {
                                url,kpiname,oc
                        });
                    }
                }
                if (item.Period == "Q")
                {
                    foreach (var cat in listCategories)
                    {
                        url = http + $"/ChartPeriod/?kpilevelcode={item.KPILevelCode}&catid={cat}&period={item.Period}&year={DateTime.Now.Year}&start=1&end=4";
                        listURLToChartPeriod.Add(new string[3]
                        {
                                url,kpiname,oc
                        });
                    }
                }
                if (item.Period == "Y")
                {
                    foreach (var cat in listCategories)
                    {
                        url = http + $"/ChartPeriod/?kpilevelcode={item.KPILevelCode}&catid={cat}&period={item.Period}&year={DateTime.Now.Year}&start={DateTime.Now.Year}&end={DateTime.Now.Year}";
                        listURLToChartPeriod.Add(new string[3]
                        {
                                    url,kpiname,oc
                        });
                    }
                }
            }
            return listURLToChartPeriod;

        }
        /// <summary>
        /// Hàm này dùng để xem chi tiết cụ thể của thông báo
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="users"></param>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task CreateNotificationDetails(List<string[]> datas, IEnumerable<int> users, int notificationId)
        {
            var listNotification = new List<NotificationDetail>();
            foreach (var item in users)
            {
                foreach (var item2 in datas)
                {
                    listNotification.Add(new NotificationDetail
                    {
                        Content = item2[1] + " ( " + item2[2] + " ) ",
                        URL = item2[0],
                        NotificationID = notificationId,
                        UserID = item
                    });
                }
            }

            _dbContext.NotificationDetails.AddRange(listNotification);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ImportDataViewModel> ImportData(List<UploadDataViewModel> entity, string userUpdate, int userId)
        {
            #region *) Biến toàn cục
            var URL = _configuration.GetSection("AppSettings:URL").Value.ToSafetyString();

            var listAdd = new List<Data>();
            var listTag = new List<Tag>();
            var listSendMail = new List<string>();
            var listUploadKPIVMs = new List<UploadKPIViewModel>();
            var listDataSuccess = new List<UploadKPIViewModel>();


            var dataModel = _dbContext.Datas;
            var kpiLevelModel = _dbContext.KPILevels;
            var kpiModel = _dbContext.KPIs;
            var levelModel = _dbContext.Levels;
            #endregion

            #region *) Lọc dữ liệu làm 2 loại là tạo mới và cập nhật
            var tuple = CreateOrUpdateData(entity);
            var listCreate = tuple.Item1;
            var listUpdate = tuple.Item2;
            #endregion
            try
            {
                #region *) Tạo mới
                if (listCreate.Count() > 0)
                {
                    _dbContext.Datas.AddRange(listCreate);
                    await _dbContext.SaveChangesAsync();
                    //Gui mail list nay khi update
                    //Tạo mới xong rồi thì thêm vào list gửi mail 
                    foreach (var item in listCreate)
                    {

                        var tblKPILevelByKPILevelCode = await kpiLevelModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode);
                        if (item.Value.ToDouble() > 0)
                        {
                            var dataSuccess = new UploadKPIViewModel()
                            {
                                KPILevelCode = item.KPILevelCode,
                                Area = levelModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.LevelID).Name,
                                KPIName = kpiModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.KPIID).Name,
                                Week = item.Week,
                                Month = item.Month,
                                Quarter = item.Quarter,
                                Year = item.Year
                            };
                            listDataSuccess.Add(dataSuccess);
                        }
                        if (item.Value.ToDouble() < item.Target.ToDouble())
                        {
                            var dataUploadKPIVM = new UploadKPIViewModel()
                            {
                                KPILevelCode = item.KPILevelCode,
                                Area = levelModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.LevelID).Name,
                                KPIName = kpiModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.KPIID).Name,
                                Week = item.Week,
                                Month = item.Month,
                                Quarter = item.Quarter,
                                Year = item.Year
                            };
                            listUploadKPIVMs.Add(dataUploadKPIVM);
                        }
                    }
                    //Tìm ID theo KPILevelCode trong bản KPILevel
                    var listKPILevel = listCreate.Select(x => x.KPILevelCode).Distinct().ToArray();
                    var listKPILevelID = _dbContext.KPILevels.Where(a => listKPILevel.Contains(a.KPILevelCode)).Select(a => a.ID).ToArray();

                    #region *) Lưu vào bảng thông báo
                    var notify = new Notification();
                    notify.Content = "You have just uploaded some KPIs.";
                    notify.Action = "Upload";
                    notify.TaskName = "Upload KPI Data";
                    notify.Link = URL + "/Home/ListSubNotificationDetail/";
                    notify.UserID = userId;
                    _dbContext.Notifications.Add(notify);
                    await _dbContext.SaveChangesAsync();
                    #endregion
                    #region *) Thông báo với các manager, owner, sponsor, updater khi upload xong
                    var listManager = (await _dbContext.Managers.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listOwner = (await _dbContext.Owners.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listSponsor = (await _dbContext.Sponsors.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listUpdater = (await _dbContext.Uploaders.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listAll = listManager.Union(listOwner).Union(listOwner).Union(listSponsor).Union(listUpdater);
                    #endregion


                    #region *) Thêm vào bảng chi tiết thông báo
                    var listUrls = await ListURLToChartPriodAsync(listCreate);
                    await CreateNotificationDetails(listUrls, listAll, notify.ID);
                    #endregion
                }
                #endregion
                #region *) Cập nhật
                if (listUpdate.Count() > 0)
                {
                    foreach (var item in listUpdate)
                    {
                        switch (item.Period)
                        {
                            case "W":
                                var dataW = await dataModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Period == item.Period && x.Week == item.Week && x.Yearly == item.Yearly);
                                dataW.Value = item.Value;
                                dataW.Target = item.Target;
                                _dbContext.SaveChanges();
                                break;
                            case "M":
                                var dataM = await dataModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Period == item.Period && x.Month == item.Month && x.Yearly == item.Yearly);
                                dataM.Value = item.Value;
                                dataM.Target = item.Target;
                                _dbContext.SaveChanges();
                                break;
                            case "Q":

                                var dataQ = await dataModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Period == item.Period && x.Quarter == item.Quarter && x.Yearly == item.Yearly);
                                dataQ.Value = item.Value;
                                dataQ.Target = item.Target;
                                _dbContext.SaveChanges();
                                break;
                            case "Y":
                                var dataY = await dataModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Period == item.Period && x.Year == item.Year && x.Yearly == item.Yearly);
                                dataY.Value = item.Value;
                                dataY.Target = item.Target;
                                _dbContext.SaveChanges();
                                break;
                            default:
                                break;
                        }
                        var tblKPILevelByKPILevelCode = await kpiLevelModel.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode);
                        if (item.Value.ToDouble() > 0)
                        {
                            var dataSuccess = new UploadKPIViewModel()
                            {
                                KPILevelCode = item.KPILevelCode,
                                Area = levelModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.LevelID).Name,
                                KPIName = kpiModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.KPIID).Name,
                                Week = item.Week,
                                Month = item.Month,
                                Quarter = item.Quarter,
                                Year = item.Year
                            };
                            listDataSuccess.Add(dataSuccess);
                        }
                        //Nếu dữ liệu mà nhỏ hơn mục tiêu thì sẽ gửi mail
                        if (item.Value.ToDouble() < item.Target.ToDouble())
                        {
                            var dataUploadKPIVM = new UploadKPIViewModel()
                            {
                                KPILevelCode = item.KPILevelCode,
                                Area = levelModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.LevelID).Name,
                                KPIName = kpiModel.FirstOrDefault(x => x.ID == tblKPILevelByKPILevelCode.KPIID).Name,
                                Week = item.Week,
                                Month = item.Month,
                                Quarter = item.Quarter,
                                Year = item.Year
                            };
                            listUploadKPIVMs.Add(dataUploadKPIVM);
                        }
                    }
                    //Tìm ID theo KPILevelCode trong bản KPILevel
                    var listKPILevel = listUpdate.Select(x => x.KPILevelCode).Distinct().ToArray();
                    var listKPILevelID = _dbContext.KPILevels.Where(a => listKPILevel.Contains(a.KPILevelCode)).Select(a => a.ID).ToArray();

                    #region *) Lưu vào bảng thông báo
                    var notify = new Notification();
                    notify.Content = "You have just uploaded some KPIs.";
                    notify.Action = "Upload";
                    notify.TaskName = "Upload KPI Data";
                    notify.UserID = userId;
                    notify.Link = URL + "/Home/ListSubNotificationDetail/";
                    _dbContext.Notifications.Add(notify);
                    await _dbContext.SaveChangesAsync();
                    #endregion
                    #region *) Thông báo với các manager, owner, sponsor, updater khi upload xong
                    var listManager = (await _dbContext.Managers.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listOwner = (await _dbContext.Owners.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listSponsor = (await _dbContext.Sponsors.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listUpdater = (await _dbContext.Uploaders.Where(x => listKPILevelID.Contains(x.KPILevelID)).ToListAsync()).DistinctBy(x => x.KPILevelID).Select(x => x.UserID).ToList();
                    var listAll = listManager.Union(listOwner).Union(listOwner).Union(listSponsor).Union(listUpdater);
                    #endregion
                    #region *) Thêm vào bảng chi tiết thông báo
                    var listUrls = await ListURLToChartPriodAsync(listUpdate);
                    await CreateNotificationDetails(listUrls, listAll, notify.ID);
                    #endregion
                }

                #endregion
                if (listUploadKPIVMs.Count > 0 || listDataSuccess.Count > 0)
                {
                    return new ImportDataViewModel
                    {
                        ListUploadKPIVMs = listUploadKPIVMs,
                        ListDataSuccess = listDataSuccess,
                        ListSendMail = listSendMail,
                        Status = true,
                    };
                }
                else
                {
                    return new ImportDataViewModel
                    {
                        ListUploadKPIVMs = listUploadKPIVMs,
                        ListSendMail = listSendMail,
                        Status = true,
                    };
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return new ImportDataViewModel
                {
                    ListUploadKPIVMs = listUploadKPIVMs,
                    Status = false,
                };
            }
        }
        public async Task<bool> IsUpdater(int id)
        {
            if (await _dbContext.Uploaders.FindAsync(id) == null)
                return false;
            return true;
        }
        public async Task<object> KPIRelated(int levelId, int page, int pageSize)
        {
            var obj = await _dbContext.KPILevels.Where(x => x.LevelID == levelId && x.Checked == true).ToListAsync();
            var kpiName = _dbContext.KPIs;
            var datas = _dbContext.Datas;
            var list = new List<KPIUploadViewModel>();
            if (obj != null)
            {
                foreach (var item in obj)
                {
                    var data = new KPIUploadViewModel()
                    {
                        KPIName = kpiName.FirstOrDefault(x => x.ID == item.KPIID).Name,
                        StateW = item.WeeklyChecked == true && datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Week > 0) != null ? true : false,
                        StateM = item.MonthlyChecked == true && datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Month > 0) != null ? true : false,
                        StateQ = item.QuarterlyChecked == true && datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Quarter > 0) != null ? true : false,
                        StateY = item.YearlyChecked == true && datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Year > 0) != null ? true : false,

                        StateDataW = item.WeeklyChecked ?? false,
                        StateDataM = item.MonthlyChecked ?? false,
                        StateDataQ = item.QuarterlyChecked ?? false,
                        StateDataY = item.YearlyChecked ?? false,
                    };
                    list.Add(data);
                }
                var total = list.Count();
                list = list.OrderBy(x => x.KPIName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return new
                {
                    model = list,
                    total,
                    page,
                    pageSize,
                    status = true
                };
            }
            return new
            {
                status = false
            };
        }
        /// <summary>
        /// Convert the nested hierarchical object to flatten object
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<TreeView> ConvertHierarchicalToFlattenObject(TreeView parent)
        {
            yield return parent;
            foreach (TreeView child in parent.children) // check null if you must
                foreach (TreeView relative in ConvertHierarchicalToFlattenObject(child))
                    yield return relative;
        }
        public IEnumerable<LevelViewModel> ConvertHierarchicalToFlattenObject2(LevelViewModel parent)
        {
            if (parent == null)
                parent = new LevelViewModel();
            if (parent.Levels == null)
                parent.Levels = new List<LevelViewModel>();
            yield return parent;
            foreach (LevelViewModel child in parent.Levels) // check null if you must
                foreach (LevelViewModel relative in ConvertHierarchicalToFlattenObject2(child))
                    yield return relative;
        }
        public List<LevelViewModel> GetTree(List<LevelViewModel> list, int parent)
        {
            return list.Where(x => x.ParentID == parent).Select(x => new LevelViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Levels = GetTree(list, x.ID)
            }).ToList();
        }
        public async Task<object> UpLoadKPILevelTrack(int userid, int page, int pageSize)
        {
            var model1 = await _levelService.GetListTreeForWorkplace(userid);
            var relative = ConvertHierarchicalToFlattenObject(model1);
            var itemuser = _dbContext.Users.FirstOrDefault(x => x.ID == userid).LevelID;
            var level = _dbContext.Levels.Select(
                x => new LevelViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Code = x.Code,
                    ParentID = x.ParentID,
                    ParentCode = x.ParentCode,
                    LevelNumber = x.LevelNumber,
                    State = x.State,
                    CreateTime = x.CreateTime
                }).ToList();
            // here you get your list
            var itemlevel = _dbContext.Levels.FirstOrDefault(x => x.ID == itemuser);
            var tree = GetTree(level, itemuser).FirstOrDefault();

            var relative2 = ConvertHierarchicalToFlattenObject2(tree);
            //var KPILevels = _dbContext.KPILevels.Where(x => x.Checked == true).ToList();
            var list = new List<KPIUploadViewModel>();


            var userKPIlevel = _dbContext.KPILevels.Where(x => x.Checked == true && x.LevelID == itemuser).ToList();
            foreach (var item in userKPIlevel)
            {
                var data = new KPIUploadViewModel()
                {
                    KPIName = _dbContext.KPIs.FirstOrDefault(x => x.ID == item.KPIID).Name,
                    Area = _dbContext.Levels.FirstOrDefault(x => x.ID == item.LevelID).Name,
                    StateW = item.WeeklyChecked == true && _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Week > 0) != null ? true : false,
                    StateM = item.MonthlyChecked == true && _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Month > 0) != null ? true : false,
                    StateQ = item.QuarterlyChecked == true && _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Quarter > 0) != null ? true : false,
                    StateY = item.YearlyChecked == true && _dbContext.Datas.FirstOrDefault(x => x.KPILevelCode == item.KPILevelCode && x.Year > 0) != null ? true : false,

                    StateDataW = (bool?)item.WeeklyChecked ?? false,
                    StateDataM = (bool?)item.MonthlyChecked ?? false,
                    StateDataQ = (bool?)item.QuarterlyChecked ?? false,
                    StateDataY = (bool?)item.YearlyChecked ?? false,

                };
                list.Add(data);
            }
            var total = 0;
            if (relative2 != null)
            {
                var KPILevels = new List<KPILevel>();
                foreach (var aa in relative2)
                {
                    if (aa != null)
                    {
                        KPILevels = (await _dbContext.KPILevels.Where(x => x.Checked == true && x.LevelID == aa.ID)
                       .Select(a => new
                       {
                           a.KPIID,
                           a.LevelID,
                           a.WeeklyChecked,
                           a.MonthlyChecked,
                           a.QuarterlyChecked,
                           a.YearlyChecked,
                           a.KPILevelCode
                       }).ToListAsync())
                       .Select(x => new KPILevel
                       {
                           KPIID = x.KPIID,
                           LevelID = x.LevelID,
                           WeeklyChecked = x.WeeklyChecked,
                           MonthlyChecked = x.MonthlyChecked,
                           QuarterlyChecked = x.QuarterlyChecked,
                           YearlyChecked = x.YearlyChecked,
                           KPILevelCode = x.KPILevelCode
                       }).ToList();
                    }

                    if (KPILevels != null)
                    {
                        foreach (var item in KPILevels)
                        {
                            var data = new KPIUploadViewModel()
                            {
                                KPIName = (await _dbContext.KPIs.FirstOrDefaultAsync(x => x.ID == item.KPIID)).Name,
                                Area = (await _dbContext.Levels.FirstOrDefaultAsync(x => x.ID == item.LevelID)).Name,
                                StateW = item.WeeklyChecked == true && (await _dbContext.Datas.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Week > 0)) != null ? true : false,
                                StateM = item.MonthlyChecked == true && (await _dbContext.Datas.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Month > 0)) != null ? true : false,
                                StateQ = item.QuarterlyChecked == true && (await _dbContext.Datas.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Quarter > 0)) != null ? true : false,
                                StateY = item.YearlyChecked == true && (await _dbContext.Datas.FirstOrDefaultAsync(x => x.KPILevelCode == item.KPILevelCode && x.Year > 0)) != null ? true : false
                            };
                            list.Add(data);
                        }
                    }

                }
                total = list.Count();
                list = list.OrderBy(x => x.KPIName).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            }

            return new
            {
                model = list,
                total,
                page,
                pageSize
            };
        }
    }
}
