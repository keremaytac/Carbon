﻿using Carbon.Redis.Abstractions;
using Carbon.WebApplication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.Demo.WebApplication.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : CarbonController
    {
        private readonly IRedisRepository _redis;
        public RedisController(IRedisRepository redis)
        {
            _redis = redis;
        }


        [HttpGet]
        [Route("Cache")]
        public async Task<string> Cache()
        {
            var home = new Home()
            {
                Address = "Koç Digital"
            };
            var homeForSummer = new Home()
            {
                Address = "Hawaii"
            };

            var customer = new Customer()
            {
                Age = 21,
                Name = "Bilgehan",
                Home = new List<Home>() { home, homeForSummer }
            };

            var (setStringIsSuccess, setStringError) = await _redis.StringSetAsync(string.Format(CacheKey.HomeAddressById, home.Id), home.Address);

            var (getStringData, getStringerror) = await _redis.StringGetAsync(string.Format(CacheKey.HomeAddressById, home.Id));



            var errorSetSimple = await _redis.SetSimpleObjectAsync(string.Format(CacheKey.CustomerHome, customer.Id, home.Id), home);
            var errorSetSimple1 = await _redis.SetSimpleObjectAsync(string.Format(CacheKey.CustomerHome, customer.Id, homeForSummer.Id), homeForSummer);

            var (setComplexIsSuccess, setComplexError) = await _redis.SetComplexObject(string.Format(CacheKey.CustomerById, customer.Id), customer);

            var (homeData, homeError) = _redis.GetSimpleObject<Home>(string.Format(CacheKey.CustomerHome, customer.Id, home.Id));
            var (summerHomeData, summerHomeError) = _redis.GetSimpleObject<Home>(string.Format(CacheKey.CustomerHome, customer.Id, homeForSummer.Id));
            var (customerData, customerError) = _redis.GetComplexObject<Customer>(string.Format(CacheKey.CustomerById, customer.Id));

            var customer2 = new Customer()
            {
                Age = 21,
                Name = "Bilgehan",
                Home = new List<Home>() { home, homeForSummer }
            };
            var (setComplexIsSuccess1, setComplexError1) = await _redis.SetComplexObject(string.Format(CacheKey.CustomerById, customer2.Id), customer);
            var (isDeleted, errorRemove) = await _redis.RemoveKey(string.Format(CacheKey.CustomerById, customer2.Id));

            var (removedList, couldNotBeRemoved) = _redis.RemoveKeysByPattern(string.Format(CacheKey.CustomerHome, customer.Id, "*"));
            return null;
        }


        public class Customer
        {
            public Customer()
            {
                Id = Guid.NewGuid();
            }
            public Guid Id { get; set; }
            public int Age { get; set; }
            public string Name { get; set; }
            public List<Home> Home { get; set; }
        }
        public class Home
        {
            public Home()
            {
                Id = Guid.NewGuid();
            }
            public Guid Id { get; set; }
            public string Address { get; set; }
        }

        public static class CacheKey
        {
            public const string HomeAddressById = "Home:{0}:Address";

            public const string CustomerById = "Customer:{0}";
            public const string CustomerHome = "Customer:{0}:Home:{1}";
        }
    }
}