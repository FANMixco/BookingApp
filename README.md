# BookingApp

[![Actions Status](https://github.com/FANMixco/BookingApp/workflows/.NET%20Core/badge.svg)](https://github.com/FANMixco/BookingApp)

A simple app for reserving books in a Library. Powered by:

- ASP .NET Core MVC 7+
- Entity Framework Core for SQLite
- SQLite
- BootStrap
- DataTable
- jQuery 3+
- JavaScript
- xUnit

[![preview1][1]][1]


[![preview2][2]][2]


  [1]: https://mir-cdn.behance.net/v1/rendition/project_modules/fs/c8bae393836207.5e6f59f3edf60.png
  [2]: https://mir-cdn.behance.net/v1/rendition/project_modules/fs/8731a293836207.5e6f59f3eeb22.png

To generate the docker image and run it:

```
docker build -t bookingapp .
docker run --name bookingapp_test --rm -it -p 8000:80 bookingapp
```

If you don't have access to Visual Studio, you can create the publish folder with this command:

```
dotnet publish -f net7.0 -r linux-x64 -c Release -o ./publish
```

## Note:
The default user and password are **admin**.
