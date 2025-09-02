using AutoMapper;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.MappingProfiles
{
    public class GradeMappingProfile : Profile
    {
        public GradeMappingProfile()
        {
            CreateMap<GradeRequestDTO, Grade>();
            CreateMap<Grade, GradeResponseDTO>();
        }
    }
}
