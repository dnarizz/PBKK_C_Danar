@echo off
title Cek Database - Student Registration
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0cek-database.ps1"
echo.
pause
