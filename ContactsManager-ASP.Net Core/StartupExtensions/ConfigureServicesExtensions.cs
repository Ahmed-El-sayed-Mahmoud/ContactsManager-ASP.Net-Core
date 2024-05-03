﻿using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace ContactsManager_ASP.Net_Core
{
	public static class ConfigureServicesExtensions
	{
		public static void ConfigureServices(this IServiceCollection services,ConfigurationManager configurationManager)
		{
			services.AddHttpLogging(options =>
			{
				options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
			});
			//Adding global filters
			services.AddControllersWithViews(options =>
			{
				// options.Filters.Add<ResponseHeadersActionFilter>(1);if it receives no args
				//ILogger<ResponseHeadersActionFilter>? logger = services.BuildServiceProvider()
				//.GetRequiredService<ILogger<ResponseHeadersActionFilter>>();

				//options.Filters.Add(new ResponseHeadersActionFilter(logger, "KeyFromGlobal", "valfromglobal",2));
			});
			services.AddScoped<IPersonRepository, PersonRepository>();
			services.AddScoped<ICountryRepository, CountryRepository>();
			services.AddScoped<ICountryServices, CountryServices>();
			services.AddScoped<IPersonGetterServices, PersonGetterServices>();
			services.AddScoped<IPersonAdderServices, PersonAdderServices>();
			services.AddScoped<IPersonUploaderServices, PersonUploaderServices>();
			services.AddScoped<IPersonDeleterServices, PersonDeleterServices>();
			services.AddScoped<IPersonSorterServices, PersonSorterServices>();
			services.AddScoped<IPersonUpdaterServices, PersonUpdaterServices>();
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configurationManager.GetConnectionString("DefaultConnection")));
		}
	}
}
