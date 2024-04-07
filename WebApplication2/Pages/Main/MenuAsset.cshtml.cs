using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;
using System;
using System.Drawing.Imaging;

namespace WebApplication2.Pages.Main
{
    public class MenuAssetModel : PageModel
    {
        private readonly IApplicationContext _context;
        public string QRCodeImageUrl { get; set; }

        public MenuAssetModel(IApplicationContext context)
        {
            _context = context;
        }

        public Assets Asset { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Asset = await _context.FindAssetAsync(id);

            if (Asset == null)
            {
                return NotFound();
            }
            await OnPostGenerateQRCodeAsync(Asset.inventory_number);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, string objectName, string inventoryNumber, int quantity, int yearIntroduction, decimal initialCost, decimal residualValue, int usefulLife, string technicalCondition)
        {
            var Asset = await _context.FindAssetAsync(id);

            if (Asset == null)
            {
                return NotFound();
            }

            // Проверяем и обновляем наименование
            if (string.IsNullOrWhiteSpace(objectName))
            {
                ModelState.AddModelError("ObjectName", "Неверное значение.");
            }
            else
            {
                Asset.object_name = objectName;
            }

            // Проверяем и обновляем инвентарный номер
            if (string.IsNullOrWhiteSpace(inventoryNumber) || inventoryNumber == "0")
            {
                ModelState.AddModelError("InventoryNumber", "Неверное значение.");
            }
            else if (inventoryNumber != Asset.inventory_number.ToString())
            {
                // Проверяем, существует ли уже запись с таким инвентарным номером
                var existingAssetWithSameInventoryNumber = await _context.Assets.FirstOrDefaultAsync(a => a.inventory_number == int.Parse(inventoryNumber) && a.id != id);

                if (existingAssetWithSameInventoryNumber != null)
                {
                    // Если запись уже существует, генерируем сообщение об ошибке
                    ModelState.AddModelError("InventoryNumber", "Такой инвентарный номер уже используется.");
                    return Page(); 
                }
                else
                {
                    Asset.inventory_number = int.Parse(inventoryNumber);
                }
            }
            if (quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Неверное значение.");
            }
            else
            {
                Asset.quantity = quantity;
            }

            if (yearIntroduction <= 0)
            {
                ModelState.AddModelError("YearIntroduction", "Неверное значение.");
            }
            else
            {
                Asset.year_introduction = yearIntroduction;
            }

            if (initialCost <= 0)
            {
                ModelState.AddModelError("InitialCost", "Неверное значение.");
            }
            else
            {
                Asset.initial_cost = initialCost;
            }

            if (residualValue <= 0)
            {
                ModelState.AddModelError("ResidualValue", "Неверное значение.");
            }
            else
            {
                Asset.residual_value = residualValue;
            }

            if (usefulLife <= 0)
            {
                ModelState.AddModelError("UsefulLife", "Неверное значение.");
            }
            else
            {
                Asset.useful_life = usefulLife;
            }

            if (string.IsNullOrWhiteSpace(technicalCondition))
            {
                ModelState.AddModelError("TechnicalCondition", "Неверное значение.");
            }
            else
            {
                Asset.technical_condition = technicalCondition;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Main/menu");
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var assetToDelete = await _context.FindAssetAsync(id);

            if (assetToDelete == null)
            {
                return NotFound();
            }

            _context.Assets.Remove(assetToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Main/menu");
        }
        public async Task<IActionResult> OnPostGenerateQRCodeAsync(int inventoryNumber)
        {
            string inventoryNumberStr = inventoryNumber.ToString();
            // Генерируем QR-код на основе инвентарного номера
            Bitmap qrCodeImage = GenerateQRCode(inventoryNumberStr);

            // Сохраняем изображение в памяти как массив байтов
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, ImageFormat.Png);
                byte[] qrCodeBytes = memoryStream.ToArray();

                // Конвертируем массив байтов в строку Base64 для отображения в HTML
                string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                string qrCodeImageSrc = $"data:image/png;base64,{qrCodeBase64}";

                // Сохраняем URL изображения в свойстве модели
                QRCodeImageUrl = qrCodeImageSrc;

                // Возвращаем результат в виде JSON
                return new JsonResult(new { QRCodeImage = qrCodeImageSrc });
            }
        }


        private Bitmap GenerateQRCode(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(5); 
            return qrCodeImage;
        }

        public async Task<IActionResult> OnPostPrintAsync(int inventoryNumber)
        {
            // Преобразование инвентарного номера в строку
            string inventoryNumberStr = inventoryNumber.ToString();

            // Генерация QR-кода на основе инвентарного номера
            Bitmap qrCodeImage = GenerateQRCode(inventoryNumberStr);

            // Создание изображения с текстом инвентарного номера
            Bitmap finalImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height + 20); // Увеличиваем высоту для текста
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                // Заполняем весь холст белым цветом
                g.Clear(Color.White);

                // Рисуем QR-код в верхней части изображения
                g.DrawImage(qrCodeImage, 0, 0);

                // Добавляем текст инвентарного номера под QR-кодом
                using (Font font = new Font("Times New Roman", 16))
                {
                    // Центрируем текст по горизонтали
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    RectangleF rect = new RectangleF(0, qrCodeImage.Height, finalImage.Width, 20);
                    g.DrawString(inventoryNumberStr, font, Brushes.Black, rect, format);
                }
            }

            await using (MemoryStream stream = new MemoryStream())
            {
                finalImage.Save(stream, ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();

                // Возвращаем файл изображения для печати
                return File(imageBytes, "image/png");
            }
        }
    }
}
