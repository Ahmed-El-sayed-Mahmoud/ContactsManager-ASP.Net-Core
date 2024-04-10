using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.ValidationHelpers;
using System;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Services
{
    public class PersonServices : IPersonServices
    {
        private readonly ICountryServices _countriesService;
        private readonly ApplicationDbContext _db;
        public PersonServices(ApplicationDbContext personsDbContext , ICountryServices countryServices)
        {
            _countriesService =countryServices;
            _db = personsDbContext;
        }

        public async Task<PersonResponse> AddPerson(AddPersonRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            int duplicate = await _db.Persons.CountAsync(temp => (temp.PersonName == request.PersonName!.Trim())&&(temp.Email == request.Email!.Trim()) );

            if (duplicate>0)
            {
                throw new ArgumentException("This Contact already exist with the same name and Email");
            }
            Person person = request.ToPerson();
            person.Country =( await _countriesService.GetCountryById(request.CountryId))?.CountryName;
            
             CountryResponse? c = (await _countriesService.GetAllCountries()).FirstOrDefault(t => t.CountryName == request.Country);
            person.CountryID=c?.CountryId;
            person.Country = c?.CountryName;
            _db.Persons.Add(person);
           await _db.SaveChangesAsync();
            //_db.sp_AddPerson(person);
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = (await _countriesService.GetCountryById(personResponse.CountryID))?.CountryName;
            return personResponse;

        }

        public async Task<bool> DeletePerson(Guid? ID)
        {
            if (ID == null) throw new ArgumentNullException();
            if (ID == Guid.Empty) throw new ArgumentNullException(nameof(ID));
            Person? person =await  _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == ID);
            if (person == null)
            {
                return false;
            }
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PersonResponse>> GetAllPeople()
        {
            // return _db.sp_GetAllPersons().Select(temp=>temp.ToPersonResponse()).ToList();
            var persons = await _db.Persons.Include("country").ToListAsync();
           return persons.Select(t=>t.ToPersonResponse()).ToList();
        }
        public async Task<List<PersonResponse>> GetFiltered(string? SearchString, string? SearchBy)
        {
            SearchString = SearchString?.Trim();
            List<PersonResponse> All = await GetAllPeople();
            List<PersonResponse> Filtered =await  GetAllPeople();
            switch (SearchBy)
            {
                case nameof(PersonResponse.PersonName):
                    Filtered = All.Where((temp) => temp.PersonName != null &&
                    temp.PersonName.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    Filtered = All.Where((temp) => temp.Email != null &&
                   temp.Email.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Country):
                    Filtered = All.Where((temp) => temp.Country != null &&
                   temp.Country.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    Filtered = All.Where((temp) => temp.Address != null &&
                   temp.Address.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    Filtered = All.Where((temp) => temp.Gender?.ToString() != null &&
                   temp.Gender.ToString().ToLower() == SearchString.ToLower()).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    Filtered = All.Where((temp) => temp.DateOfBirth != null &&
                   temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default: Filtered = All; break;
            }
            return Filtered;
        }

        public async Task<PersonResponse?> GetPersonById(Guid? ID)
        {
            if (ID == null || ID == Guid.Empty) throw new ArgumentNullException("Id is Null");
            return (await _db.Persons.FirstOrDefaultAsync((temp) => temp.PersonID == ID))?.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(UpdatePersonRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));
            ValidationHelper.ModelValidator(personUpdateRequest);
            Person? person = await _db.Persons.Where(temp => temp.PersonID == personUpdateRequest.PersonId).FirstOrDefaultAsync();
            if (person == null)
                throw new ArgumentException("This ID does not exist in your Contacts");
            person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            person.Gender = personUpdateRequest.Gender;
            person.Email = personUpdateRequest.Email;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Address = personUpdateRequest.Address;
            person.CountryID = personUpdateRequest.CountryId;

            return person.ToPersonResponse();

        }
        public async Task<MemoryStream> GetPersonsExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream memoryStream = new MemoryStream();
            using (var package = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Persons Sheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Receive News Letters";
                workSheet.Cells["A1:H1"].Style.Fill.SetBackground(System.Drawing.Color.Gray);
                workSheet.Cells.Style.HorizontalAlignment=OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                List<PersonResponse>persons = await _db.Persons.Include("country").Select(t=>t.ToPersonResponse()).ToListAsync();
                int cur_row = 2;
                foreach(PersonResponse personResponse in persons)
                {
                    workSheet.Cells[cur_row, 1].Value = personResponse.PersonName;
                    workSheet.Cells[cur_row, 2].Value = personResponse.Email;
                    if (personResponse.DateOfBirth.HasValue)
                        workSheet.Cells[cur_row, 3].Value = personResponse.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    workSheet.Cells[cur_row, 4].Value = personResponse.Age;
                    workSheet.Cells[cur_row, 5].Value = personResponse.Gender;
                    workSheet.Cells[cur_row, 6].Value = personResponse.Country;
                    workSheet.Cells[cur_row, 7].Value = personResponse.Address;
                    workSheet.Cells[cur_row, 8].Value = personResponse.ReceiveNewsLetters;
                    cur_row++;
                }
                workSheet.Cells["A1:H1"].AutoFitColumns();
                await package.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<int> UploadExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int PersonsAdded = 0;
            using(var package=new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["persons"];
                if(worksheet==null)
                {
                    return 0;
                }
                int rowsCount = worksheet.Rows.Count();
                if (rowsCount == 0)
                    return 0;
                
                for(int cur_row = 2; cur_row<=rowsCount; cur_row++)
                {
                    AddPersonRequest person=new AddPersonRequest();
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 1].Value?.ToString()))
                        person.PersonName = worksheet.Cells[cur_row, 1].Value?.ToString();
                    else
                        continue;
                    
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 2].Value?.ToString()))
                        person.Email = worksheet.Cells[cur_row, 2].Value?.ToString();
                    else 
                        continue;
                    
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 3].Value?.ToString()))
                        person.DateOfBirth =Convert.ToDateTime( worksheet.Cells[cur_row, 3].Value);
                   
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 4].Value?.ToString()))
                        person.Gender = worksheet.Cells[cur_row, 4].Value?.ToString();
                    
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 5].Value?.ToString()))
                        person.Country = worksheet.Cells[cur_row, 5].Value?.ToString();
                   
                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 6].Value?.ToString()))
                        person.Address = worksheet.Cells[cur_row, 6].Value?.ToString();

                    if (!string.IsNullOrEmpty(worksheet.Cells[cur_row, 7].Value?.ToString()))
                        person.ReceiveNewsLetters = Convert.ToBoolean(worksheet.Cells[cur_row, 7].Value);


                    if(!_db.Persons.Where(t=>t.Email==person.Email&&t.PersonName==person.PersonName).Any())
                    {
                        await AddPerson(person);
                        PersonsAdded++;
                    }
                }
            }
            return PersonsAdded;
        }
    }
}
