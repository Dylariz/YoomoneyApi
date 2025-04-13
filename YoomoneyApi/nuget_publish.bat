@echo off
setlocal enabledelayedexpansion

:: Указываем путь к файлу с API-ключом
set "secretFilePath=%~dp0nuget_secret.txt"

:: Проверяем, существует ли файл с API-ключом
if not exist "%secretFilePath%" (
    echo Файл %secretFilePath% не найден.
    exit /b 1
)

:: Читаем первую строку из файла
set /p "apiKey="<"%secretFilePath%"

:: Указываем путь к файлу YoomoneyApi.csproj относительно расположения .bat-файла
set "csprojPath=%~dp0YoomoneyApi.csproj"

:: Проверяем, существует ли файл
if not exist "%csprojPath%" (
    echo Файл %csprojPath% не найден.
    exit /b 1
)

:: Извлекаем строку с версией
for /f "tokens=*" %%A in ('findstr "<Version>" "%csprojPath%"') do (
    set "versionLine=%%A"
)

:: Удаляем лишние теги, оставляя только значение версии
for /f "tokens=2 delims=<>" %%A in ("!versionLine!") do (
    set "version=%%A"
)

:: Выводим версию
echo Публикуемая версия: %version%

:: Указываем путь к nupkg-файлу
set "nupkgPath=%~dp0bin\Release\YoomoneyApi.%version%.nupkg"

:: Проверяем, существует ли nupkg-файл
if not exist "%nupkgPath%" (
    echo Пакет %nupkgPath% не найден.
    exit /b 1
)

:: Выполняем push в NuGet
dotnet nuget push "%nupkgPath%" --source https://api.nuget.org/v3/index.json --api-key %apiKey%

:: Проверяем результат
if %errorlevel% neq 0 (
    echo Ошибка при публикации пакета.
    exit /b 1
)

echo Пакет успешно опубликован.