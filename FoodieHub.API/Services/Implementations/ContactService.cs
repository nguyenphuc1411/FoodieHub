﻿using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Contact;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ContactService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> AddContact(ContactDTO contact)
        {
            var obj = _mapper.Map<Contact>(contact);
            _appDbContext.Contacts.Add(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Thank you for reaching out to us. We will contact you soon.",
                    Data = _mapper.Map<ContactDTO>(obj),
                    StatusCode = 201

                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add new category.",
                    StatusCode = 400

                };
            }
        }

        public async Task<ServiceResponse> Get()
        {
            var contacts = await _appDbContext.Contacts
                .Include(c => c.Product)  // Đảm bảo lấy thông tin Product liên quan
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            var mappedContacts = _mapper.Map<List<GetContact>>(contacts);
            return new ServiceResponse
            {
                Success = true,
                Message = "Get Contact successfully",
                Data = mappedContacts,  // Trả về dữ liệu đã ánh xạ
                StatusCode = 200
            };
        }
    }
}
