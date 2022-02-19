using Application.Interfaces.Services;
using Core.Constants.Data;
using Core.Constants.Permission;
using Core.Constants.Role;
using Core.Constants.User;
using Core.Models.Dto.Data;
using Domain.Entities.Data;
using Domain.Entities.Setting;
using Domain.Entities.User;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Verifier.Infrastructure.Services
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<TheTerminalUser> _userManager;
        private readonly RoleManager<TheTerminalRole> _roleManager;

        public DatabaseSeeder(
            UserManager<TheTerminalUser> userManager,
            RoleManager<TheTerminalRole> roleManager,
            ApplicationDbContext applicationDbContext,
            ILogger<DatabaseSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public void Initialize(bool shouldSeedDefaultData)
        {
            if (shouldSeedDefaultData)
            {
                AddAdministrator();
                AddBasicUser();
                AddDataAsset();
            }

            AddSetting();
            _applicationDbContext.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new TheTerminalRole(RoleConstants.AdministratorRole, "Administrator role with full permissions");
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    _logger.LogInformation("Seeded Administrator Role.");
                }

                //Check if User Exists
                var superUser = new TheTerminalUser
                {
                    FirstName = User.AdminFirstName,
                    LastName = User.AdminLastName,
                    Email = User.AdminEmail,
                    UserName = User.AdminUsername,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };

                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);

                    if (!result.Succeeded)
                        foreach (var error in result.Errors)
                            _logger.LogError(error.Description);
                    else
                        _logger.LogInformation("Seeded Default SuperAdmin User.");
                }

                foreach (var permission in Permissions.GetRegisteredPermissions())
                    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);

            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var basicRole = new TheTerminalRole(RoleConstants.BasicRole, "Basic role with default permissions");
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                    _logger.LogInformation("Seeded Basic Role.");
                }

                //Check if User Exists
                var basicUser = new TheTerminalUser
                {
                    FirstName = User.BasicUserFirstName,
                    LastName = User.AdminLastName,
                    Email = User.BasicUserEmail,
                    UserName = User.BasicUsername,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };

                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                    _logger.LogInformation("Seeded User with Basic Role.");
                }

            }).GetAwaiter().GetResult();
        }

        private void AddSetting()
        {
            var settingsLookup = _applicationDbContext.Settings.ToList();

            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.AppVersion)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.AppVersion,
                    Value = "1.0",
                };

                _applicationDbContext.Settings.Add(setting);
            }

            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.CachedDuration)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.CachedDuration,
                    Value = "10",
                };

                _applicationDbContext.Settings.Add(setting);
            }

            #region EmailTemplates
            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.SendGridAccountActivationEmailTemplate)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.SendGridAccountActivationEmailTemplate,
                    Value = "d-76a1baf2819444a9aea3fb0414488006",
                };

                _applicationDbContext.Settings.Add(setting);
            }

            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.SendGridForgotPasswordEmailTemplate)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.SendGridForgotPasswordEmailTemplate,
                    Value = "d-bcc7b92734c64e82a6f2323665de114a",
                };

                _applicationDbContext.Settings.Add(setting);
            }

            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.CommunityInvitationTemplate)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.CommunityInvitationTemplate,
                    Value = "d-d4c70712f6074a3b96da8a4010cbcddf",
                };

                _applicationDbContext.Settings.Add(setting);
            }
            #endregion

            #region Chat
            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.ChatGroupConnectedUsersDurationIn)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.ChatGroupConnectedUsersDurationIn,
                    Value = "20",
                };

                _applicationDbContext.Settings.Add(setting);
            }
            #endregion

            #region DataSets
            var availableDataSets = JsonConvert.SerializeObject(new List<TheTerminalDataSetDto>() {
                        new TheTerminalDataSetDto {Name = DataSetConstants.DailyBars },
                        new TheTerminalDataSetDto {Name = DataSetConstants.DailyBarsAdjusted },
                        new TheTerminalDataSetDto {Name = DataSetConstants.MinuteBars },
                        new TheTerminalDataSetDto {Name = DataSetConstants.MinuteBarsRaw },
                        new TheTerminalDataSetDto {Name = DataSetConstants.CiqFinancialsAnnual },
                        new TheTerminalDataSetDto {Name = DataSetConstants.CiqFinancialsQuarterly },
                        new TheTerminalDataSetDto {Name = DataSetConstants.CiqAdrs },
                        new TheTerminalDataSetDto {Name = DataSetConstants.CiqSecurityMaster },
                        new TheTerminalDataSetDto {Name = DataSetConstants.CiqEarningEstimate },
                        });

            if (!settingsLookup.Any(x => x.Key.Equals(AppSettings.AvailableDataSet)))
            {
                var setting = new Setting
                {
                    Key = AppSettings.AvailableDataSet,
                    Value = availableDataSets,
                };

                _applicationDbContext.Settings.Add(setting);
            }
            else
            {
                var dataSetSettings = _applicationDbContext.Settings.FirstOrDefault(u => u.Key.Equals(AppSettings.AvailableDataSet));
                dataSetSettings.Value = availableDataSets;
            }
            #endregion

            _logger.LogInformation("Seeded Settings.");
        }

        private void AddDataAsset()
        {
            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Email.Equals(User.AdminEmail));
            var dataSetOneName = "Test Data Asset 1";

            if (!_applicationDbContext.DataAsset.Any(u => u.Name == dataSetOneName))
            {
                var dataAsset = new DataAsset
                {
                    UserId = user.Id,
                    Name = dataSetOneName,
                    JoinType = Core.Enums.DataAssetJoinType.Inner,
                    DataQuery = new List<QueryParams>() {
                new QueryParams { Symbols = new List<string>() { "AAPL", "GOOG" },
                Name = DataSetConstants.DailyBars, DateParams = new DateParams{ DateType = Core.Enums.DataQueryDateType.Interval, FromDate = DateTimeOffset.Now.AddDays(-5), ToDate =  DateTimeOffset.Now  } },
                new QueryParams { Symbols = new List<string>() { "AAPL" },
                Name = DataSetConstants.DailyBars, DateParams = new DateParams{ Periods = 3 , PeriodType =  Core.Enums.TimePeriodType.Days,  DateType = Core.Enums.DataQueryDateType.PeriodsToNow  } } }
                };

                _applicationDbContext.DataAsset.Add(dataAsset);
            }

            var dataSetTwoName = "Test Data Asset 2";

            if (!_applicationDbContext.DataAsset.Any(u => u.Name == dataSetTwoName))
            {
                var dataAsset = new DataAsset
                {
                    UserId = user.Id,
                    Name = dataSetTwoName,
                    JoinType = Core.Enums.DataAssetJoinType.Inner,
                    DataQuery = new List<QueryParams>() {
                new QueryParams { Symbols = new List<string>() { "AAPL", "GOOG" },
                Name = DataSetConstants.DailyBars, DateParams = new DateParams{ Periods = 3 , PeriodType = Core.Enums.TimePeriodType.Hours, DateType = Core.Enums.DataQueryDateType.PointInTime, ToDate =  DateTimeOffset.Now  } }}
                };

                _applicationDbContext.DataAsset.Add(dataAsset);
            }


            var dataSetThree = "Test Data Asset 3";

            if (!_applicationDbContext.DataAsset.Any(u => u.Name == dataSetThree))
            {
                var dataAsset = new DataAsset
                {
                    UserId = user.Id,
                    Name = dataSetThree,
                    JoinType = Core.Enums.DataAssetJoinType.Inner,
                    DataQuery = new List<QueryParams>() {
                new QueryParams { Symbols = new List<string>() { "AAPL", "GOOG" },
                Name = DataSetConstants.DailyBarsAdjusted, DateParams = new DateParams{ DateType = Core.Enums.DataQueryDateType.Interval, FromDate = DateTimeOffset.Now.AddDays(-5), ToDate =  DateTimeOffset.Now  } },
                new QueryParams { Symbols = new List<string>() { "AAPL" },
                Name = DataSetConstants.MinuteBars, DateParams = new DateParams{ Periods = 3 , PeriodType =  Core.Enums.TimePeriodType.Days,  DateType = Core.Enums.DataQueryDateType.PeriodsToNow  } } }
                };

                _applicationDbContext.DataAsset.Add(dataAsset);
            }
        }
    }
}