using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MVCApp.Models;
using MVCApp.ViewModels;

namespace MVCApp
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {               
                cfg.CreateMap<Recognition, BadgeViewModel>()
                    .ForMember(c => c.Id, r => r.MapFrom(rcg => rcg.Badge.Id))
                    .ForMember(c => c.Title, r => r.MapFrom(rcg => rcg.Badge.Title))
                    .ForMember(c => c.Description, r => r.MapFrom(rcg => rcg.Badge.Description))
                    .ForMember(c => c.ImageUrl, r => r.MapFrom(rcg => rcg.Badge.ImageUrl));
                cfg.CreateMap<Person, PersonViewModel>();
                cfg.CreateMap<Person, PersonEditViewModel>();
                cfg.CreateMap<PersonEditViewModel, Person>();
                cfg.CreateMap<Badge, BadgeViewModel>();
                cfg.CreateMap<BadgeCreateViewModel, Badge>();
                cfg.CreateMap<Badge, BadgeEditViewModel>();
                cfg.CreateMap<BadgeEditViewModel, Badge>();
                cfg.CreateMap<ApplicationUser, ExpandedUserViewModel>();
            });

        }
    }
}