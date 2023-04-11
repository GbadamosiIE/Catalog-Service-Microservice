using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using play.Catalog.Contracts;
using Play.Catalog.Service.Contracts;
using Play.Catalog.Service.DTOS;
using Play.Catalog.Service.Entitties;
using Play.Catalog.Service.Extensions;
using Play.Catalog.Service.Repositories;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _itemsRepository;
        // For rabbitMQ configurations
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IItemsRepository itemsRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> Get()
        {
            
            var items = (await _itemsRepository.GetAllAsync())
            .Select(item => item.AsDto());
            return Ok(items);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid Id)
        {
            var item = (await _itemsRepository.GetByIdAsync(x => x.Id == Id)).AsDto();
            if (item is null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> post(CreateItemDto model)
        {
            var item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CreateDate = DateTimeOffset.UtcNow
            };
            await _itemsRepository.CreateAsync(item);
            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id,item.Name,item.Description));
            return Ok(item);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(Guid Id, UpdateItemDto model)
        {
            var existingItem = await _itemsRepository.GetByIdAsync(x => x.Id == Id);
            if (existingItem is null)
            {
                return NotFound();
            }
            existingItem.Name = model.Name;
            existingItem.Description = model.Description;
            existingItem.Price = model.Price;
            await _itemsRepository.UpdateAsync((Item)existingItem, existingItem);
            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id,existingItem.Name,existingItem.Description));
            return NoContent();

        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var existingItem = await _itemsRepository.GetByIdAsync(x => x.Id == Id);
            if (existingItem is null)
            {
                return NotFound();
            }
            await _itemsRepository.RemoiveAsync(existingItem.Id);
            await _publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));
            return NoContent();
        }
    }

}