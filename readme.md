# Product Management Service

**Repository:** Reapit.Platform.Products

**Namespace:** Reapit.Platform.Products

**Description:** Products management service used to create and manage products and authorisation clients.

---

## Service

- This services uses packages exclusive to the [Reapit GitHub NuGet](https://docs.docker.com/desktop/install/windows-install/) 
  feed.  Ensure your NuGet configuration includes this source. 

---

## Testing

### Code Coverage

- Open a terminal window in the root directory

- Get rid of any existing TestResults directories:
```sh
# PowerShell:
Get-Childitem -Include TestResults -Recurse -Force | Remove-Item -Force -Recurse
```

- Run the following command to run the tests with code coverage analysis:
```sh
dotnet test .\src\Reapit.Platform.Products.sln --collect:"XPlat Code Coverage"
```

- Run the following command to create the code coverage report in `./coverage`
```sh
reportgenerator -reports:./src/**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html -filefilters:-*Migrations*
```

- Open the report (`./coverage/index.html`) in a browser
```sh
# Bash:
open coverage/index.html

# Command Shell:
start "" "coverage/index.html"

# PowerShell:
ii ./coverage/index.html
```

---

## Migrations
