using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkiaSharp;



namespace Employees.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Dictionary<string, int> map;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            map = new Dictionary<string, int>();
        }

        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await GetEmployeesFromApi();
            

            foreach (Employee e in employees)
            {
                int totalHoursWorked = (int)(e.EndTime - e.StartTime).TotalHours;

            if (!string.IsNullOrEmpty(e.Name))
                {
                    
                    if (map.ContainsKey(e.Name))
                    {
                        map[e.Name] += totalHoursWorked;
                    }
                    else
                    {
                        map.Add(e.Name, totalHoursWorked);
                    }
                }
            }
            var sortedList = map.ToList();
            sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            map = new Dictionary<string, int>(sortedList);
            GeneratePieChart(map);
            return View(map);

        }


        private async Task<List<Employee>> GetEmployeesFromApi()
        {
            List<Employee> employees = new List<Employee>();
            string uri = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(data);
                }
            }

            return employees;
        }
        public static void GeneratePieChart(Dictionary<string, int> data)
        {
            using (var bitmap = new SKBitmap(700, 700))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(SKColors.White);
                    float total = data.Values.Sum();
                    SKPoint center = new SKPoint(350, 350);
                    float radius = 200;
                    float startAngle = 0;
                    SKPaint textPaint = new SKPaint
                    {
                        TextSize = 20,
                        IsAntialias = true,
                        Color = SKColors.Black,
                        TextAlign = SKTextAlign.Center
                    };
                    foreach (var entry in data)
                    {
                        float sweepAngle = (entry.Value / total) * 360;
                        SKColor sliceColor = GenerateRandomColor();

                        using (var paint = new SKPaint
                        {
                            Style = SKPaintStyle.Fill,
                            Color = sliceColor
                        })
                        {
                        
                            canvas.DrawArc(new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius),
                                        startAngle,
                                        sweepAngle,
                                        true,
                                        paint);

                            float labelAngle = startAngle + (sweepAngle / 2);
                            float labelX = center.X + (float)(radius * Math.Cos(Math.PI * labelAngle / 180));
                            float labelY = center.Y + (float)(radius * Math.Sin(Math.PI * labelAngle / 180));
                            canvas.DrawText($"{entry.Key}: {(entry.Value / total) * 100:F2}%", labelX, labelY, textPaint);
                        }

                        
                        startAngle += sweepAngle;
                    }
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data2 = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite("pie_chart.png"))
                {
                    data2.SaveTo(stream);
                }
            }
        }

    private static SKColor GenerateRandomColor()
    {
        Random random = new Random();
        return new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
    }


       
    }
}