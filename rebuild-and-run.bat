@echo off
ECHO Rebuilding Docker images and starting services...
docker-compose up -d --build
ECHO.
ECHO Operation completed. Press any key to exit.
pause 