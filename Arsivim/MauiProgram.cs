using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Arsivim.Data.Context;
using Arsivim.Data.Repositories;
using Arsivim.Services.Core;
using Arsivim.ViewModels;
using Arsivim.Views;
using CommunityToolkit.Maui;

namespace Arsivim;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Veritabanı konfigürasyonu
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "arsivim.db");
		builder.Services.AddDbContext<ArsivimContext>(options =>
			options.UseSqlite($"Data Source={dbPath}"));

		// Repository'leri kaydet
		builder.Services.AddScoped<BelgeRepository>();
		builder.Services.AddScoped<KisiRepository>();

		// Servisleri kaydet
		builder.Services.AddScoped<BelgeYonetimi>();

		// ViewModels'leri kaydet
		builder.Services.AddTransient<AnaSayfaVM>();
		builder.Services.AddTransient<BelgeListeVM>();
		builder.Services.AddTransient<BelgeDetayVM>();
		builder.Services.AddTransient<KisiListeVM>();
		builder.Services.AddTransient<AyarlarVM>();
		builder.Services.AddTransient<EtiketYonetimVM>();
		builder.Services.AddTransient<GecmisVM>();
		builder.Services.AddTransient<IstatistiklerVM>();
		builder.Services.AddTransient<KullaniciYonetimVM>();

		// Pages'leri kaydet
		builder.Services.AddTransient<AnaSayfa>();
		builder.Services.AddTransient<BelgeListesi>();
		builder.Services.AddTransient<BelgeDetay>();
		builder.Services.AddTransient<KisiListesi>();
		builder.Services.AddTransient<Ayarlar>();
		builder.Services.AddTransient<EtiketYonetimi>();
		builder.Services.AddTransient<Gecmis>();
		builder.Services.AddTransient<Istatistikler>();
		builder.Services.AddTransient<KullaniciYonetimi>();
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Veritabanını başlat
		using (var scope = app.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ArsivimContext>();
			context.Database.EnsureCreated();
		}

		return app;
	}
}
