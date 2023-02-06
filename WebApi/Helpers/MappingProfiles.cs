using AutoMapper;
using Data.Mongo.Collections;
using Identity.Models;
using Identity.Services.Interfaces;
using Models.DbEntities;
using Models.DTOs.Account;
using Models.DTOs.Category;
using Models.DTOs.Client;
using Models.DTOs.Employee;
using Models.DTOs.Log;
using Models.DTOs.PurchaseItems;
using Models.DTOs.Purchases;
using Models.DTOs.Recycled;
using Models.DTOs.RecycledItems;
using Models.DTOs.Sale;
using Models.DTOs.SaleItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email));

            CreateMap<LoginLog, LogDto>()
               .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
               .ForMember(d => d.LoginTime, o => o.MapFrom(s => s.LoginTime));

            CreateMap<SubCategory, SubCategoryDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.MainCategoryId, o => o.MapFrom(s => s.MainCategoryId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<SubCategoryDto, SubCategory>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<MainCategory, MainCategoryDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.SubCategories, o => o.MapFrom(s => s.SubCategories));

            CreateMap<MainCategoryDto, MainCategory>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<MainCategoryRequest, MainCategory>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<SubCategoryRequest, SubCategory>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.MainCategoryId, o => o.MapFrom(s => s.MainCategoryId));

            CreateMap<Purchase, PurchaseDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => "PZ/" + s.Id.ToString() + "/" + s.Date.Month.ToString() + "/" + s.Date.Year.ToString()))
                .ForMember(d => d.Approved, o => o.MapFrom(s => s.Approved))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => s.DateToApproval))
                .ForMember(d => d.PurchaseItems, o => o.MapFrom(s => s.PurchaseItems))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy == "0"? "Brak danych": s.CreatedBy));

            CreateMap<PurchaseRequest, Purchase>()
                .ForMember(d => d.Date, o => o.MapFrom(s => DateTime.Now))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => DateTime.Now.AddDays(2)))
                .ForMember(d => d.PurchaseItems, o => o.MapFrom(s => s.PurchaseItems))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.Approved, o => o.MapFrom(s => false));

            CreateMap<PurchaseUpdateRequest, Purchase>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.PurchaseItems, o => o.MapFrom(s => s.PurchaseItems));

            CreateMap<PurchaseItem, PurchaseItemDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.Contamination, o => o.MapFrom(s => s.Contamination))
                .ForMember(d => d.SubCategory, o => o.MapFrom(s => s.SubCategory));

            CreateMap<PurchaseItemRequest, PurchaseItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.Contamination, o => o.MapFrom(s => s.Contamination))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId));

            CreateMap<PurchaseItemInsertRequest, PurchaseItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.Contamination, o => o.MapFrom(s => s.Contamination))
                .ForMember(d => d.PurchaseId, o => o.MapFrom(s => s.PurchaseId));

            CreateMap<PurchaseItemUpdateRequest, PurchaseItem>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.Contamination, o => o.MapFrom(s => s.Contamination))
                .ForMember(d => d.PurchaseId, o => o.MapFrom(s => s.PurchaseId));

            CreateMap<Sale, SaleDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => "WZ/" + s.Id.ToString() + "/" + s.Date.Month.ToString() + "/" + s.Date.Year.ToString()))
                .ForMember(d => d.Approved, o => o.MapFrom(s => s.Approved))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => s.DateToApproval))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.SaleItems, o => o.MapFrom(s => s.SaleItems))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy == "0" ? "Brak danych" : s.CreatedBy));

            CreateMap<SaleRequest, Sale>()
                .ForMember(d => d.Date, o => o.MapFrom(s => DateTime.Now))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => DateTime.Now.AddDays(2)))
                .ForMember(d => d.SaleItems, o => o.MapFrom(s => s.SaleItems))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.Approved, o => o.MapFrom(s => false));

            CreateMap<SaleUpdateRequest, Sale>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Client))
                .ForMember(d => d.SaleItems, o => o.MapFrom(s => s.SaleItems));

            CreateMap<SaleItem, SaleItemDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategory, o => o.MapFrom(s => s.SubCategory));

            CreateMap<SaleItemRequest, SaleItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId));

            CreateMap<SaleItemInsertRequest, SaleItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.SaleId, o => o.MapFrom(s => s.SaleId));

            CreateMap<SaleItemUpdateRequest, SaleItem>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.SaleId, o => o.MapFrom(s => s.SaleId));

            CreateMap<Client, ClientDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.CompanyName != null ? s.CompanyName : s.FirstName + " " + s.LastName));

            CreateMap<Client, CompanyClientDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.CompanyName));

            CreateMap<Client, IndividualClientDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<ClientRequest, Client>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.CompanyName))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<ClientUpdateRequest, Client>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.CompanyName))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<EmployeeRequest, Employee>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<EmployeeUpdateRequest, Employee>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

            CreateMap<RecycledItem, RecycledItemDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategory, o => o.MapFrom(s => s.SubCategory));

            CreateMap<RecycledItemRequest, RecycledItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId));

            CreateMap<RecycledItemInsertRequest, RecycledItem>()
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.RecycledId, o => o.MapFrom(s => s.RecycledId));

            CreateMap<RecycledItemUpdateRequest, RecycledItem>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(d => d.SubCategoryId, o => o.MapFrom(s => s.SubCategoryId))
                .ForMember(d => d.RecycledId, o => o.MapFrom(s => s.RecycledId));

            CreateMap<Recycled, RecycledDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => "WS/" + s.Id.ToString() + "/" + s.Date.Month.ToString() + "/" + s.Date.Year.ToString()))
                .ForMember(d => d.Approved, o => o.MapFrom(s => s.Approved))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => s.DateToApproval))
                .ForMember(d => d.Client, o => o.MapFrom(s => s.Employee))
                .ForMember(d => d.RecycledItems, o => o.MapFrom(s => s.RecycledItems))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy == "0" ? "Brak danych" : s.CreatedBy));

            CreateMap<RecycledRequest, Recycled>()
                .ForMember(d => d.Date, o => o.MapFrom(s => DateTime.Now))
                .ForMember(d => d.DateToApproval, o => o.MapFrom(s => DateTime.Now.AddDays(2)))
                .ForMember(d => d.RecycledItems, o => o.MapFrom(s => s.RecycledItems))
                .ForMember(d => d.Employee, o => o.MapFrom(s => s.Employee))
                .ForMember(d => d.Approved, o => o.MapFrom(s => false));

            CreateMap<RecycledUpdateRequest, Recycled>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Employee, o => o.MapFrom(s => s.Employee))
                .ForMember(d => d.RecycledItems, o => o.MapFrom(s => s.RecycledItems));

        }
    }
}
