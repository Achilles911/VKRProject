using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;

namespace WebApplication2.Pages.Main
{
    public class MenuAssetModel : PageModel
    {
        private readonly ApplicationContext _context;

        public MenuAssetModel(ApplicationContext context)
        {
            _context = context;
        }

        public Assets Asset { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Asset = await _context.Assets.FindAsync(id);

            if (Asset == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string objectName, string inventoryNumber, string quantity, int yearIntroduction, decimal initialCost, decimal residualValue, int usefulLife, string technicalCondition)
        {
            var assetToUpdate = await _context.Assets.FindAsync(id);

            if (assetToUpdate == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(objectName))
            {
                assetToUpdate.object_name = objectName;
            }

            // ѕреобразуем строку в int перед сохранением в базе данных
            if (int.TryParse(inventoryNumber, out int parsedInventoryNumber))
            {
                assetToUpdate.inventory_number = parsedInventoryNumber;
            }
            else
            {
                // ќбработка ошибки неправильного формата числа
                // «десь можно вывести сообщение об ошибке или выполнить другие действи€ по вашему усмотрению
            }

            // ѕреобразуем строку в int перед сохранением в базе данных
            if (!string.IsNullOrEmpty(quantity) && int.TryParse(quantity, out int parsedQuantity))
            {
                assetToUpdate.quantity = parsedQuantity;
            }
            else
            {
                // ќбработка ошибки неправильного формата числа
                // «десь можно вывести сообщение об ошибке или выполнить другие действи€ по вашему усмотрению
            }

            assetToUpdate.year_introduction = yearIntroduction;
            assetToUpdate.initial_cost = initialCost;
            assetToUpdate.residual_value = residualValue;
            assetToUpdate.useful_life = usefulLife;
            assetToUpdate.technical_condition = technicalCondition;

            _context.Assets.Update(assetToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Main/menu");
        }

    }
}
