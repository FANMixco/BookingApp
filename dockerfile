# Use the official ASP.NET Core image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Copy the app files to the /app folder in the image
WORKDIR /app
COPY ./BookingApp/publish .
COPY ./BookingApp/booking.db .

# Expose port 80 for the web app
EXPOSE 80

# Run the app using dotnet command
ENTRYPOINT ["dotnet", "BookingApp.dll"]