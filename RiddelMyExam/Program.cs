using DinkToPdf.Contracts;
using DinkToPdf;
using RiddelMyExam.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var pdfGlobalSettings = new GlobalSettings
{
    ColorMode = ColorMode.Color,
    Orientation = Orientation.Portrait,
    PaperSize = PaperKind.A4,
    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
};

var pdfObjectSettings = new ObjectSettings
{
    PagesCount = true,
    HtmlContent = "<h1>Hello World!</h1>",
    WebSettings = { DefaultEncoding = "utf-8" },
    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
    FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]" },
};


builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddTransient<candidateModel>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.Run();
