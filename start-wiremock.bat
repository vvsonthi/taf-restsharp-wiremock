@echo off
REM WireMock Startup Script for Local Testing (Windows)
REM Usage: start-wiremock.bat

SET WIREMOCK_VERSION=3.3.1
SET WIREMOCK_JAR=wiremock-standalone-%WIREMOCK_VERSION%.jar
SET WIREMOCK_PORT=9091

echo ========================================
echo   WireMock Local Server Startup
echo ========================================

REM Check if JAR exists
IF NOT EXIST "%WIREMOCK_JAR%" (
    echo.
    echo [91mWireMock JAR not found![0m
    echo.
    echo Please download WireMock JAR manually:
    echo.
    echo   Download URL:
    echo   https://repo1.maven.org/maven2/org/wiremock/wiremock-standalone/%WIREMOCK_VERSION%/%WIREMOCK_JAR%
    echo.
    echo   Save it to: %CD%
    echo.
    echo Or use Git Bash to run: ./start-wiremock.sh
    echo.
    pause
    exit /b 1
)

echo.
echo [92mStarting WireMock on port %WIREMOCK_PORT%...[0m
echo [93mFramework will auto-upload files from WireMock/ folder[0m
echo [93mPress Ctrl+C to stop[0m
echo ========================================
echo.

java -jar "%WIREMOCK_JAR%" --port %WIREMOCK_PORT% --verbose --global-response-templating

echo.
echo WireMock stopped.
pause
