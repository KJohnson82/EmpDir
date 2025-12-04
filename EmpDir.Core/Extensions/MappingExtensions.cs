using EmpDir.Core.DTOs;
using EmpDir.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                JobTitle = employee.JobTitle,
                IsManager = (bool)employee.IsManager,
                PhoneNumber = employee.PhoneNumber,
                CellNumber = employee.CellNumber,
                Extension = employee.Extension,
                Email = employee.Email,
                NetworkId = employee.NetworkId,
                EmpAvatar = employee.EmpAvatar,
                Location = (int)employee.Location,
                Department = (int)employee.Department,
                RecordAdd = employee.RecordAdd,
                Active = (bool)employee.Active,
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
                // Note: Navigation properties not set from DTO
            };
        }

        // ========== DEPARTMENT MAPPING ==========

        public static DepartmentDto ToDto(this Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                DeptName = department.DeptName,
                Location = (int)department.Location,
                DeptManager = department.DeptManager,
                DeptPhone = department.DeptPhone,
                DeptEmail = department.DeptEmail,
                DeptFax = department.DeptFax,
                RecordAdd = department.RecordAdd,
                Active = (bool)department.Active,
                LocationName = department.DeptLocation?.LocName
            };
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

        public static LocationDto ToDto(this Location location)
        {
            return new LocationDto
            {
                Id = location.Id,
                LocName = location.LocName,
                LocNum = location.LocNum,
                Address = location.Address,
                City = location.City,
                State = location.State,
                Zipcode = location.Zipcode,
                PhoneNumber = location.PhoneNumber,
                FaxNumber = location.FaxNumber,
                Email = location.Email,
                Hours = location.Hours,
                Loctype = (int)location.Loctype,
                AreaManager = location.AreaManager,
                StoreManager = location.StoreManager,
                RecordAdd = location.RecordAdd,
                Active = (bool)location.Active,
                LoctypeName = location.LocationType?.LoctypeName
            };
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
                LoctypeName = loctype.LoctypeName
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

