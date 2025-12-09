using EmpDir.Core.DTOs;
using EmpDir.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmpDir.Core.Extensions
{
    public static class MappingExtensions
    {
        // ========== EMPLOYEE MAPPING ==========

        public static EmployeeDto ToDto(this Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName ?? string.Empty,
                LastName = employee.LastName ?? string.Empty,
                JobTitle = employee.JobTitle ?? string.Empty,
                IsManager = employee.IsManager ?? false,
                PhoneNumber = employee.PhoneNumber ?? string.Empty,
                CellNumber = employee.CellNumber,
                Extension = employee.Extension,
                Email = employee.Email ?? string.Empty,
                NetworkId = employee.NetworkId,
                EmpAvatar = employee.EmpAvatar,
                Location = employee.Location ?? 0,
                Department = employee.Department ?? 0,
                RecordAdd = employee.RecordAdd,
                Active = employee.Active ?? true,
                LocationName = employee.EmpLocation?.LocName,
                DepartmentName = employee.EmpDepartment?.DeptName
            };
        }

        public static Employee ToModel(this EmployeeDto dto)
        {
            return new Employee
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                JobTitle = dto.JobTitle,
                IsManager = dto.IsManager,
                PhoneNumber = dto.PhoneNumber,
                CellNumber = dto.CellNumber,
                Extension = dto.Extension,
                Email = dto.Email,
                NetworkId = dto.NetworkId,
                EmpAvatar = dto.EmpAvatar,
                Location = dto.Location,
                Department = dto.Department,
                RecordAdd = dto.RecordAdd,
                Active = dto.Active
            };
        }

        // ========== DEPARTMENT MAPPING ==========

        public static DepartmentDto ToDto(this Department department, bool includeEmployees = true)
        {
            var dto = new DepartmentDto
            {
                Id = department.Id,
                DeptName = department.DeptName ?? string.Empty,
                Location = department.Location ?? 0,
                DeptManager = department.DeptManager,
                DeptPhone = department.DeptPhone,
                DeptEmail = department.DeptEmail,
                DeptFax = department.DeptFax,
                RecordAdd = department.RecordAdd,
                Active = department.Active ?? true,
                LocationName = department.DeptLocation?.LocName
            };

            // Populate child collection if requested and available
            if (includeEmployees && department.Employees != null)
            {
                dto.Employees = department.Employees.Select(e => e.ToDto()).ToList();
            }

            return dto;
        }

        public static Department ToModel(this DepartmentDto dto)
        {
            return new Department
            {
                Id = dto.Id,
                DeptName = dto.DeptName,
                Location = dto.Location,
                DeptManager = dto.DeptManager,
                DeptPhone = dto.DeptPhone,
                DeptEmail = dto.DeptEmail,
                DeptFax = dto.DeptFax,
                RecordAdd = dto.RecordAdd,
                Active = dto.Active
            };
        }

        // ========== LOCATION MAPPING ==========

        public static LocationDto ToDto(this Location location, bool includeRelated = true)
        {
            var dto = new LocationDto
            {
                Id = location.Id,
                LocName = location.LocName ?? string.Empty,
                LocNum = location.LocNum ?? 0,
                Address = location.Address ?? string.Empty,
                City = location.City ?? string.Empty,
                State = location.State ?? string.Empty,
                Zipcode = location.Zipcode ?? string.Empty,
                PhoneNumber = location.PhoneNumber,
                FaxNumber = location.FaxNumber,
                Email = location.Email,
                Hours = location.Hours,
                Loctype = location.Loctype ?? 0,
                AreaManager = location.AreaManager,
                StoreManager = location.StoreManager,
                RecordAdd = location.RecordAdd,
                Active = location.Active ?? true,
                LoctypeName = location.LocationType?.LoctypeName
            };

            // Populate child collections if requested and available
            if (includeRelated)
            {
                if (location.Departments != null)
                {
                    dto.Departments = location.Departments.Select(d => d.ToDto(includeEmployees: false)).ToList();
                }

                if (location.Employees != null)
                {
                    dto.Employees = location.Employees.Select(e => e.ToDto()).ToList();
                }
            }

            return dto;
        }

        public static Location ToModel(this LocationDto dto)
        {
            return new Location
            {
                Id = dto.Id,
                LocName = dto.LocName,
                LocNum = dto.LocNum,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Zipcode = dto.Zipcode,
                PhoneNumber = dto.PhoneNumber,
                FaxNumber = dto.FaxNumber,
                Email = dto.Email,
                Hours = dto.Hours,
                Loctype = dto.Loctype,
                AreaManager = dto.AreaManager,
                StoreManager = dto.StoreManager,
                RecordAdd = dto.RecordAdd,
                Active = dto.Active
            };
        }

        // ========== LOCTYPE MAPPING ==========

        public static LoctypeDto ToDto(this Loctype loctype)
        {
            return new LoctypeDto
            {
                Id = loctype.Id,
                LoctypeName = loctype.LoctypeName ?? string.Empty
            };
        }

        public static Loctype ToModel(this LoctypeDto dto)
        {
            return new Loctype
            {
                Id = dto.Id,
                LoctypeName = dto.LoctypeName
            };
        }
    }
}