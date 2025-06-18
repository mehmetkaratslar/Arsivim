@echo off
echo ==========================================
echo    Arsivim PC Versiyonu - Build ve Run
echo ==========================================
echo.

echo [1/4] Eski build dosyalarinin temizleniyor...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo Temizlik tamamlandi.
echo.

echo [2/4] Proje restore ediliyor...
dotnet restore
if %errorlevel% neq 0 (
    echo HATA: Restore basarisiz!
    pause
    exit /b 1
)
echo Restore tamamlandi.
echo.

echo [3/4] Proje build ediliyor...
dotnet build -c Debug -f net8.0-windows10.0.19041.0 --verbosity normal
if %errorlevel% neq 0 (
    echo HATA: Build basarisiz!
    echo.
    echo Alternatif cozum deneniyor...
    dotnet build -c Debug -f net8.0-windows10.0.19041.0 --verbosity minimal --no-restore
    if %errorlevel% neq 0 (
        echo HATA: Alternatif build de basarisiz!
        pause
        exit /b 1
    )
)
echo Build tamamlandi.
echo.

echo [4/4] Uygulama baslatiliyor...
echo Uygulama acilirken lutfen bekleyin...
dotnet run --launch-profile Arsivim --framework net8.0-windows10.0.19041.0 --no-build
if %errorlevel% neq 0 (
    echo HATA: Uygulama baslamiyor!
    echo.
    echo Debug bilgileri:
    echo - Framework: net8.0-windows10.0.19041.0
    echo - Profile: Arsivim
    echo - Config: Debug
    echo.
    pause
    exit /b 1
)

echo.
echo Uygulama basariyla kapatildi.
pause 