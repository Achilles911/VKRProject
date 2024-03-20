using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Pages.Main
{
    public class MenuModel : PageModel
    {
        private readonly ApplicationContext _context;

        public MenuModel(ApplicationContext context)
        {
            _context = context;
        }

        public IList<Assets> Assets { get; set; }
        public string SearchMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Assets = await _context.Assets.ToListAsync();
            return Page();
        }

        public IActionResult OnPostCreateExcel()//Создание Excel таблицы
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            var assets = _context.Assets.ToList();// Получаем все записи из таблицы БД
            using (var package = new ExcelPackage())// Создаем новый пакет Excel
            {
               
                var worksheet = package.Workbook.Worksheets.Add("Assets"); // Добавляем новый лист

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Наименование";
                worksheet.Cells[1, 2].Value = "Инвентарный номер";
                worksheet.Cells[1, 3].Value = "Количество";
                worksheet.Cells[1, 4].Value = "Год ввода";
                worksheet.Cells[1, 5].Value = "Первоначальная стоимость";
                worksheet.Cells[1, 6].Value = "Остаточная стоимость";
                worksheet.Cells[1, 7].Value = "Срок полезного использования";
                worksheet.Cells[1, 8].Value = "Техническое состояние";

                // Данные из БД
                int row = 2;
                foreach (var asset in assets)
                {
                    worksheet.Cells[row, 1].Value = asset.object_name;
                    worksheet.Cells[row, 2].Value = asset.inventory_number;
                    worksheet.Cells[row, 3].Value = asset.quantity;
                    worksheet.Cells[row, 4].Value = asset.year_introduction;
                    worksheet.Cells[row, 5].Value = asset.initial_cost;
                    worksheet.Cells[row, 6].Value = asset.residual_value;
                    worksheet.Cells[row, 7].Value = asset.useful_life;
                    worksheet.Cells[row, 8].Value = asset.technical_condition;
                    row++;
                }

               
                var stream = new MemoryStream(); // Сохраняем Excel-файл
                package.SaveAs(stream);
                stream.Position = 0;
                // Возвращаем Excel-файл как результат
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Assets.xlsx");
            }
        }

        public async Task<IActionResult> OnPostSearchAsync(string searchString)//поиск предметов
        {
            Assets = await _context.Assets.ToListAsync();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                // Если строка поиска пустая, выводим сообщение
                SearchMessage = "Заполните эту строку";
                return Page();
            }

            if (int.TryParse(searchString, out int inventoryNumber) && inventoryNumber != 0)
            {
                // Если введенное значение является числом и не равно нулю
                Assets = await _context.Assets
                    .Where(a => a.inventory_number == inventoryNumber)
                    .ToListAsync();

                if (Assets.Any())
                {
                    // Если найдены соответствующие активы, возвращаем страницу с этими активами
                    return Page();
                }
                else
                {
                    // Если активы не найдены, выводим сообщение
                    Assets = await _context.Assets.ToListAsync();
                    SearchMessage = "Не найдено предметов";
                    return Page();
                }
            }
            else
            {
                // Если введенное значение не является числом или равно нулю, ничего не делаем
                return Page();
            }
        }
        
        






    }
}
