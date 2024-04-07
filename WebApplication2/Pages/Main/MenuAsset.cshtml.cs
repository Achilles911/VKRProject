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

            // ��������� � ��������� ������������
            if (string.IsNullOrWhiteSpace(objectName))
            {
                ModelState.AddModelError("ObjectName", "�������� ��������.");
            }
            else
            {
                Asset.object_name = objectName;
            }

            // ��������� � ��������� ����������� �����
            if (string.IsNullOrWhiteSpace(inventoryNumber) || inventoryNumber == "0")
            {
                ModelState.AddModelError("InventoryNumber", "�������� ��������.");
            }
            else if (inventoryNumber != Asset.inventory_number.ToString())
            {
                // ���������, ���������� �� ��� ������ � ����� ����������� �������
                var existingAssetWithSameInventoryNumber = await _context.Assets.FirstOrDefaultAsync(a => a.inventory_number == int.Parse(inventoryNumber) && a.id != id);

                if (existingAssetWithSameInventoryNumber != null)
                {
                    // ���� ������ ��� ����������, ���������� ��������� �� ������
                    ModelState.AddModelError("InventoryNumber", "����� ����������� ����� ��� ������������.");
                    return Page(); 
                }
                else
                {
                    Asset.inventory_number = int.Parse(inventoryNumber);
                }
            }
            if (quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "�������� ��������.");
            }
            else
            {
                Asset.quantity = quantity;
            }

            if (yearIntroduction <= 0)
            {
                ModelState.AddModelError("YearIntroduction", "�������� ��������.");
            }
            else
            {
                Asset.year_introduction = yearIntroduction;
            }

            if (initialCost <= 0)
            {
                ModelState.AddModelError("InitialCost", "�������� ��������.");
            }
            else
            {
                Asset.initial_cost = initialCost;
            }

            if (residualValue <= 0)
            {
                ModelState.AddModelError("ResidualValue", "�������� ��������.");
            }
            else
            {
                Asset.residual_value = residualValue;
            }

            if (usefulLife <= 0)
            {
                ModelState.AddModelError("UsefulLife", "�������� ��������.");
            }
            else
            {
                Asset.useful_life = usefulLife;
            }

            if (string.IsNullOrWhiteSpace(technicalCondition))
            {
                ModelState.AddModelError("TechnicalCondition", "�������� ��������.");
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
            // ���������� QR-��� �� ������ ������������ ������
            Bitmap qrCodeImage = GenerateQRCode(inventoryNumberStr);

            // ��������� ����������� � ������ ��� ������ ������
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, ImageFormat.Png);
                byte[] qrCodeBytes = memoryStream.ToArray();

                // ������������ ������ ������ � ������ Base64 ��� ����������� � HTML
                string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                string qrCodeImageSrc = $"data:image/png;base64,{qrCodeBase64}";

                // ��������� URL ����������� � �������� ������
                QRCodeImageUrl = qrCodeImageSrc;

                // ���������� ��������� � ���� JSON
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
            // �������������� ������������ ������ � ������
            string inventoryNumberStr = inventoryNumber.ToString();

            // ��������� QR-���� �� ������ ������������ ������
            Bitmap qrCodeImage = GenerateQRCode(inventoryNumberStr);

            // �������� ����������� � ������� ������������ ������
            Bitmap finalImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height + 20); // ����������� ������ ��� ������
            using (Graphics g = Graphics.FromImage(finalImage))
            {
                // ��������� ���� ����� ����� ������
                g.Clear(Color.White);

                // ������ QR-��� � ������� ����� �����������
                g.DrawImage(qrCodeImage, 0, 0);

                // ��������� ����� ������������ ������ ��� QR-�����
                using (Font font = new Font("Times New Roman", 16))
                {
                    // ���������� ����� �� �����������
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

                // ���������� ���� ����������� ��� ������
                return File(imageBytes, "image/png");
            }
        }
    }
}
