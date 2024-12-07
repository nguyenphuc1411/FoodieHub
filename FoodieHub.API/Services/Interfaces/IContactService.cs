﻿using FoodieHub.API.Models.DTOs.Contact;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Services.Interfaces
{
    public interface IContactService
    {
        Task<ServiceResponse> AddContact(ContactDTO contact);
        Task<ServiceResponse> Get();
    }
}